﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OccuRec.FieldIdentification;

namespace OccuRec.FieldIdentification.UCAC2
{
    public class UCAC2Catalogue
    {
        public static bool IsValidCatalogLocation(ref string folderPath)
        {
            if (CheckSingleFolder(folderPath))
                return true;

            string u2Path = Path.GetFullPath(Path.Combine(folderPath, "u2"));
            if (CheckSingleFolder(u2Path))
            {
                folderPath = u2Path;
                return true;
            }

            return false;
        }

        private static bool CheckSingleFolder(string folderPath)
        {
            IEnumerator<string> ucac2Files = UCAC2FileIterator.UCAC2Files(folderPath);
            while (ucac2Files.MoveNext())
            {
                if (File.Exists(ucac2Files.Current))
                    // Some people may only have a partial version of the catalogue (e.g. Northern Hemisphere)
                    return true;
            }

            return false;
        }

        private static CatalogMagnitudeBand ModelFitMagnitudeBand = new CatalogMagnitudeBand(UCAC2Entry.BAND_ID_UNFILTERED, "Internal UCAC magnitude (U2 Mag)");
        private static CatalogMagnitudeBand JohnsonVFromModelFitMagnitudeBand = new CatalogMagnitudeBand(UCAC2Entry.BAND_ID_V, "Johnson V - Computed from U2 Mag");
        private static CatalogMagnitudeBand CousinsRFromMagnitudeBand = new CatalogMagnitudeBand(UCAC2Entry.BAND_ID_R, "Cousins R - Computed from U2 Mag");

        public static CatalogMagnitudeBand[] CatalogMagnitudeBands = new CatalogMagnitudeBand[] { ModelFitMagnitudeBand, JohnsonVFromModelFitMagnitudeBand, CousinsRFromMagnitudeBand };

        public static double ConvertMagnitude(double measuredMag, double vrColorIndex, Guid catalogMagBand, MagOutputBand magOutputBand)
        {
            if (catalogMagBand == UCAC2Entry.BAND_ID_R && magOutputBand == MagOutputBand.CousinsR) return measuredMag;
            if (catalogMagBand == UCAC2Entry.BAND_ID_R && magOutputBand == MagOutputBand.JohnsonV) return ColourIndexTables.GetVFromRAndVR(measuredMag, vrColorIndex);

            if (catalogMagBand == UCAC2Entry.BAND_ID_V && magOutputBand == MagOutputBand.CousinsR) return ColourIndexTables.GetRFromVAndVR(measuredMag, vrColorIndex);
            if (catalogMagBand == UCAC2Entry.BAND_ID_V && magOutputBand == MagOutputBand.JohnsonV) return measuredMag;

            if (catalogMagBand == UCAC2Entry.BAND_ID_UNFILTERED)
            {
                double jk = ColourIndexTables.GetJKFromVR(vrColorIndex);
                if (magOutputBand == MagOutputBand.CousinsR) return jk * -0.262157262293991 + measuredMag * 0.972995989114809 + measuredMag * measuredMag * 0.0294054519219995 +
                       measuredMag * measuredMag * measuredMag * -0.00152172319138341 + -1.27151807006964;
                if (magOutputBand == MagOutputBand.JohnsonV) return jk*0.482510826567376 + measuredMag*0.69853741307966 + measuredMag*measuredMag*0.053709249992501 +
                       measuredMag*measuredMag*measuredMag*-0.00221766445441987 + -0.186573576388743;
            }

            return double.NaN;
        }

        public static bool CheckAndWarnIfNoBSS(string folderPath, IWin32Window owner)
        {
            bool hasBinaryVersion = true;
            for (int i = 1; i <= 36; i++)
            {
                if (!File.Exists(Path.Combine(folderPath, string.Format("s{0}", i.ToString("00")))))
                {
                    hasBinaryVersion = false;
                    break;
                }
            }
            if (hasBinaryVersion)
                hasBinaryVersion = File.Exists(Path.Combine(folderPath, "bsindex.da"));

            bool hasTextVersion = File.Exists(Path.Combine(folderPath, "ucac2bss.dat"));

            if (hasBinaryVersion)
                return true;

            //if (hasTextVersion)
            //{
            //    if (MessageBox.Show(owner, 
            //        "The UCAC2 Bright Star Supplement is in ASCII format. Press 'OK' to generate the binary version from this ASCII version.\r\n" +
            //        "This will generate 36 files called S01 .. S36 and will require 18Mb of disk space.",
            //        "Action Required", 
            //        MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            //    {
            //        BSSBinaryFilesBuilder builder = new BSSBinaryFilesBuilder(folderPath);
            //        if (builder.Build(owner))
            //            return true;
            //    }

            //    return false;
            //}

            // TODO: If Sxx files are present but the index file is not!
            BSSBinaryFilesBuilder indexBuilder = new BSSBinaryFilesBuilder(folderPath);
            indexBuilder.BuildIndex(owner);

            if (owner == null)
                return false;

            Process.Start("ftp://ad.usno.navy.mil/users/nz/bss/");

	        MessageBox.Show(owner,
	                        "The UCAC2 Bright Star Supplement cannot be found. This part of the catalog is required by OccuRec. The total size \r\n" +
	                        "is about 18Mb - 36 's' files ('s01' ... 's36') and the file 'bsindex.da'",
	                        "Action Required",
	                        MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

            return false;
        }

        private UCAC2Index m_Index;
        private string m_CatalogLocation;

        public UCAC2Catalogue(string catalogLocation)
        {
            m_CatalogLocation = catalogLocation;
            m_Index = UCAC2Index.GetIndex(catalogLocation);
        }

        public List<IStar> GetStarsInRegion(double raDeg, double deDeg, double radiusDeg, double limitMag, float epoch)
        {
            List<SearchZone> searchZones = CatalogHelper.GetZones(raDeg, deDeg, radiusDeg);

            List<IStar> starsFromThisZone = new List<IStar>();

            foreach (SearchZone zone in searchZones)
            {
                 LoadStars(zone, limitMag, starsFromThisZone);
            }

            UCAC2Entry.TargetEpoch = epoch;
            return starsFromThisZone;
        }

        private void LoadStars(SearchZone zone, double limitMag, List<IStar> starsFromThisZone)
        {
            List<LoadPosition> searchIndexes = m_Index.GetLoadPositions(zone);

            IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UCAC2Entry)));
            try
            {
                foreach(LoadPosition pos in searchIndexes)
                {
                    string fileName;
                    if (pos.BSS)
                        fileName = Path.Combine(m_CatalogLocation, string.Format("s{0}", pos.ZoneId.ToString("00")));
                    else
                        fileName = Path.Combine(m_CatalogLocation, string.Format("z{0}", pos.ZoneId.ToString("000")));

                    long positionFrom = (pos.FromRecordId - 1)* UCAC2Entry.Size;
                    uint firstStarNoToRead = pos.FirstStarNoInBin + pos.FromRecordId;
                    uint numRecords = pos.ToRecordId - pos.FromRecordId;

                    using (FileStream str = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (BinaryReader rdr = new BinaryReader(str))
                        {
                            rdr.BaseStream.Position = positionFrom;

                            for (int i = 0; i <= numRecords; i++, firstStarNoToRead++)
                            {
                                UCAC2Entry entry = new UCAC2Entry();
                                byte[] rawData = rdr.ReadBytes(UCAC2Entry.Size);

                                Marshal.Copy(rawData, 0, buffer, Marshal.SizeOf(entry));
                                entry = (UCAC2Entry)Marshal.PtrToStructure(buffer, typeof(UCAC2Entry));

                                if (pos.BSS)
                                    entry.InitUCAC2Entry(firstStarNoToRead + UCAC2Entry.FIRST_BSS_STAR_NO);
                                else
                                    entry.InitUCAC2Entry(firstStarNoToRead);

                                if (entry.Mag > limitMag) continue;

                                if (entry.RAJ2000 < zone.RAFrom) continue;
                                if (entry.RAJ2000 > zone.RATo) continue;
                                if (entry.DEJ2000 < zone.DEFrom) continue;
                                if (entry.DEJ2000 > zone.DETo) continue;

                                starsFromThisZone.Add(entry);
                            }
                        }
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OccuRec.Astrometry.StarCatalogues;
using OccuRec.Astrometry.StarCatalogues.NOMAD;
using OccuRec.Astrometry.StarCatalogues;

namespace OccuRec.Astrometry.StarCatalogues.PPMXL
{
    public class PPMXLCatalogue
    {
        public static bool IsValidCatalogLocation(ref string folderPath)
        {
            if (CheckSingleFolder(folderPath))
                return true;

            return false;
        }

        private static bool CheckSingleFolder(string folderPath)
        {
            string indexFileName = Path.GetFullPath(folderPath + @"\zones.idx");

            if (!File.Exists(indexFileName))
                return false;

            IEnumerator<string> ppmxlFiles = PPMXLFileIterator.PPMXLFiles(folderPath);
            while (ppmxlFiles.MoveNext())
            {
                if (File.Exists(ppmxlFiles.Current))
                    // Some people may only have a partial version of the catalogue (e.g. Northern Hemisphere)
                    return true;
            }

            return false;
        }

        private static CatalogMagnitudeBand CousinsRMagnitudeBand = new CatalogMagnitudeBand(PPMXLEntry.BAND_ID_R, "Cousins R from USNOB");
        private static CatalogMagnitudeBand JohnsonBMagnitudeBand = new CatalogMagnitudeBand(PPMXLEntry.BAND_ID_B, "Johnson B from USNOB");

        public static CatalogMagnitudeBand[] CatalogMagnitudeBands = new CatalogMagnitudeBand[] { CousinsRMagnitudeBand, JohnsonBMagnitudeBand };

        public static double ConvertMagnitude(double measuredMag, double vrColorIndex, Guid catalogMagBand, MagOutputBand magOutputBand)
        {
            if (catalogMagBand == PPMXLEntry.BAND_ID_R && magOutputBand == MagOutputBand.CousinsR) return measuredMag;
            if (catalogMagBand == PPMXLEntry.BAND_ID_R && magOutputBand == MagOutputBand.JohnsonV) return ColourIndexTables.GetVFromRAndVR(measuredMag, vrColorIndex);

            if (catalogMagBand == PPMXLEntry.BAND_ID_B && magOutputBand == MagOutputBand.CousinsR) return ColourIndexTables.GetRFromBAndVR(measuredMag, vrColorIndex);
            if (catalogMagBand == PPMXLEntry.BAND_ID_B && magOutputBand == MagOutputBand.JohnsonV) return ColourIndexTables.GetVFromBAndVR(measuredMag, vrColorIndex);

            return double.NaN;
        }

        private string m_CatalogLocation;
        private PPMXLIndex m_Index;

        public PPMXLCatalogue(string catalogLocation)
        {
            m_CatalogLocation = catalogLocation;
            m_Index = PPMXLIndex.GetIndex(catalogLocation);            
        }

        public List<IStar> GetStarsInRegion(double raDeg, double deDeg, double radiusDeg, double limitMag)
        {
            return GetStarsInRegion(raDeg, deDeg, radiusDeg, limitMag, 2000.0f);
        }

        public List<IStar> GetStarsInRegion(double raDeg, double deDeg, double radiusDeg, double limitMag, float epoch)
        {
            List<SearchZone> searchZones = CatalogHelper.GetZones(raDeg, deDeg, radiusDeg);

            var starsFromThisZone = new List<IStar>();

            foreach (SearchZone zone in searchZones)
            {
                LoadStars(zone, limitMag, starsFromThisZone);
            }

            PPMXLEntry.TargetEpoch = epoch;
            return starsFromThisZone;
        }

        private void LoadStars(SearchZone zone, double limitMag, List<IStar> starsFromThisZone)
        {
            List<LoadPosition> searchIndexes = m_Index.GetLoadPositions(zone);

            foreach (LoadPosition pos in searchIndexes)
            {
                string fileName;
                fileName = Path.Combine(m_CatalogLocation, string.Format("{0}{1}{2}.dat", pos.Hemisphere, pos.ZoneId.ToString("00"), pos.SubZoneId));

                long positionFrom = (pos.FromRecordId + 1 /* for the header row */) * PPMXLEntry.Size;
                uint numRecords = pos.ToRecordId - pos.FromRecordId;

                using (FileStream fileStr = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                using (BinaryReader rdr = new BinaryReader(fileStr))
                {
                    rdr.BaseStream.Position = positionFrom;

                    for (int i = 0; i <= numRecords; i++)
                    {
                        byte[] data = rdr.ReadBytes(PPMXLEntry.Size);

                        PPMXLEntry entry = new PPMXLEntry(Encoding.ASCII.GetString(data));

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
}

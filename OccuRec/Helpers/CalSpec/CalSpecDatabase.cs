﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace OccuRec.Helpers.CalSpec
{
    public class CalSpecDatabase
    {
        public List<CalSpecStar> Stars = new List<CalSpecStar>();

        internal CalSpecDatabase()
        { }

        internal CalSpecDatabase(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            Stars.Clear();

            for (int i = 0; i < count; i++)
            {
                var star = new CalSpecStar(reader);
                Stars.Add(star);
            }
        }

        internal void Serialize(BinaryWriter writer)
        {
            writer.Write(Stars.Count);

            foreach (CalSpecStar star in Stars)
            {
                star.Serialize(writer);
            }
        }

        private static CalSpecDatabase s_Instance;
        private static object s_SyncRoot = new object();

        public static CalSpecDatabase Instance
        {
			get
			{
                if (s_Instance == null)
                {
                    lock (s_SyncRoot)
                    {
                        if (s_Instance == null)
                        {
                           Assembly assembly = Assembly.GetExecutingAssembly();
                           using (Stream compressedStream = assembly.GetManifestResourceStream("OccuRec.Helpers.CalSpec.CalSpec.db"))
                           using (var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress, true))
                           {
                               using (var reader = new BinaryReader(deflateStream))
                               {
                                   var db = new CalSpecDatabase(reader);
                                   Thread.MemoryBarrier();
                                   s_Instance = db;
                               }
                           }
                        }
                    }
                }

			    return s_Instance;
			}
        }
    }

    public class CalSpecStar
    {
        public string CalSpecStarId;
        public string AbsFluxStarId;
        public string TYC2;
        public string U4;
        public float RA_J2000_Hours;
        public float DE_J2000_Deg;
        public float pmRA;
        public float pmDE;
        public float MagV;
        public float MagBV;
        public string SpecType;
        public string STIS_Flag;
        public string FITS_File;

		public Dictionary<double, double> DataPoints = new Dictionary<double, double>();

        internal  CalSpecStar()
        { }

        internal CalSpecStar(BinaryReader reader)
        {
            CalSpecStarId = reader.ReadString();
            AbsFluxStarId = reader.ReadString();
            TYC2 = reader.ReadString();
            U4 = reader.ReadString();
            RA_J2000_Hours = reader.ReadSingle();
            DE_J2000_Deg = reader.ReadSingle();
            pmRA = reader.ReadSingle();
            pmDE = reader.ReadSingle();
            MagV = reader.ReadSingle();
            MagBV = reader.ReadSingle();
            SpecType = reader.ReadString();
            STIS_Flag = reader.ReadString();
            FITS_File = reader.ReadString();

            int cnt = reader.ReadInt32();
            DataPoints.Clear();
            for (int i = 0; i < cnt; i++)
            {
                float wavelength = reader.ReadSingle();
                float flux = reader.ReadSingle();
                DataPoints.Add(wavelength, flux);
            }
        }

        internal void Serialize(BinaryWriter writer)
        {
            writer.Write(CalSpecStarId);
            writer.Write(AbsFluxStarId);
            writer.Write(TYC2);
            writer.Write(U4);
            writer.Write(RA_J2000_Hours);
            writer.Write(DE_J2000_Deg);
            writer.Write(pmRA);
            writer.Write(pmDE);
            writer.Write(MagV);
            writer.Write(MagBV);
            writer.Write(SpecType);
            writer.Write(STIS_Flag);
            writer.Write(FITS_File);

            writer.Write(DataPoints.Count);
            foreach (double key in DataPoints.Keys)
            {
				writer.Write((float)key);
                writer.Write((float)DataPoints[key]);
            }
        }
    }
}

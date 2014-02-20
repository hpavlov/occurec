﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using OccuRec.FieldIdentification;

namespace OccuRec.FieldIdentification
{
    public interface IStar
    {
        ulong StarNo { get; }
        double RADeg { get;}
        double DEDeg { get; }
        double Mag { get; }
        double MagR { get; }
        double MagB { get; }
        double MagV { get; }
		double MagJ { get; }
		double MagK { get; }
        string GetStarDesignation(int alternativeId);
        double GetMagnitudeForBand(Guid magBandId);
    }

	[Serializable()]
    public class Star : IStar, ISerializable
    {
        private readonly ulong m_StarNo;
        private readonly double m_RADeg;
        private readonly double m_DEDeg;
        private readonly double m_Mag;

        public Star(ulong id, double raDeg, double deDeg, double mag)
        {
            m_StarNo = id;
            m_RADeg = raDeg;
            m_DEDeg = deDeg;
            m_Mag = mag;
        }

		public Star(IStar copyFrom)
			: this(copyFrom.StarNo, copyFrom.RADeg, copyFrom.DEDeg, copyFrom.Mag)
		{ }

        #region IStar Members

        public ulong StarNo
        {
            get { return m_StarNo; }
        }

        public string GetStarDesignation(int alternativeId)
        {
            return m_StarNo.ToString();
        }

        public double RADeg
        {
            get { return m_RADeg; }
        }

        public double DEDeg
        {
            get { return m_DEDeg; }
        }

        public double Mag
        {
            get { return m_Mag; }
        }

        public double MagR
        {
            get { return m_Mag; }
        }

        public double MagB
        {
            get { return m_Mag; }
        }

        public double MagV
        {
            get { return m_Mag; }
        }

		public double MagJ
		{
			get { return double.NaN; }
		}

		public double MagK
		{
			get { return double.NaN; }
		}

        public double GetMagnitudeForBand(Guid magBandId)
        {
            return m_Mag;
        }
        #endregion

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("m_StarNo", m_StarNo);
			info.AddValue("m_RADeg", m_RADeg);
			info.AddValue("m_DEDeg", m_DEDeg);
			info.AddValue("m_Mag", m_Mag);
		}

		public Star(SerializationInfo info, StreamingContext context)
        {
			m_StarNo = info.GetUInt32("m_StarNo");
			m_RADeg = info.GetDouble("m_RADeg");
			m_DEDeg = info.GetDouble("m_DEDeg");
			m_Mag = info.GetDouble("m_Mag");
        }
    }
}

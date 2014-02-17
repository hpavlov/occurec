using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace WindowsClock.Tester
{
	public class LogEntry
	{
		public float WinAccu;
		public float WinAccuNorm;
		public float OccuRecAccu;
		public float OccuRecErr;
		public float TimeDrift;
		public float TimeDriftErr;
		public float NTPAccu;
		public bool NTPTimeUpdate;
	}

	public class LogData
	{
		public List<LogEntry> Data = new List<LogEntry>(); 

		public LogData(string fileName)
		{
			ParseContent(File.ReadAllLines(fileName));
		}

		private void ParseContent(string[] content)
		{
			Data.Clear();

			//	WinAccu		WinAccuNorm	OccuRecAccu	Error	DriftCorr	NTPAccu
			//	87.0		92.4		8.0		16.0	0.0+/-0.00	17.6
			//	-13.5		-14.2		6.5		16.0	0.0+/-0.00	16.9
			//	-10.1		-11.8		4.5		16.0	0.0+/-0.00	17.0
			//	-18.9		-19.1		8.0		16.0	0.0+/-0.00	15.4*		

			for (int i = 1; i < content.Length; i++)
			{
				string[] tokens = content[i].Split(new char[] {'\t', ' '}, StringSplitOptions.RemoveEmptyEntries);
				if (tokens.Length == 6)
				{
					float winAccu = float.Parse(tokens[0], CultureInfo.InvariantCulture);
					float winAccuNorm = float.Parse(tokens[1], CultureInfo.InvariantCulture);
					float occuRecAccu = float.Parse(tokens[2], CultureInfo.InvariantCulture);
					float occuRecErr = float.Parse(tokens[3], CultureInfo.InvariantCulture);

					float diffCorr = 0;
					float diffCorrErr = 0;

					string[] diffCorrToks = tokens[4].Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					if (diffCorrToks.Length == 2)
					{
						diffCorr = float.Parse(diffCorrToks[0].TrimEnd('+'), CultureInfo.InvariantCulture);
						diffCorrErr = float.Parse(diffCorrToks[1].TrimStart('-'), CultureInfo.InvariantCulture);
					}

					float ntpAccu = float.Parse(tokens[5].TrimEnd('*'), CultureInfo.InvariantCulture);
					bool ntpRefChanged = tokens[5].IndexOf("*", StringComparison.InvariantCultureIgnoreCase) > -1;

					Data.Add(new LogEntry()
					{
						WinAccu = winAccu,
						WinAccuNorm = winAccuNorm,
						OccuRecAccu = occuRecAccu,
						OccuRecErr = occuRecErr,
						TimeDrift = diffCorr,
						TimeDriftErr = diffCorrErr,
						NTPAccu = ntpAccu,
						NTPTimeUpdate = ntpRefChanged
					});
				}
			}
		}

	}
}

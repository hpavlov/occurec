using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OccuRec.Helpers
{
    public class SlewPosition
    {
        public double RA { get; set; }
        public double DEC { get; set; }
    }

    public class LastSlewPositions
    {
        public List<SlewPosition> Positions = new List<SlewPosition>();

        public static LastSlewPositions Load()
        {
            try
            {
                var ser = new XmlSerializer(typeof(LastSlewPositions));
                return (LastSlewPositions)ser.Deserialize(new StringReader(Properties.Settings.Default.LastSlewPositions));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            return new LastSlewPositions(); 
        }

        public void RegisterLatest(double RAHours, double DEDeg)
        {
            var pos = new SlewPosition() {RA = RAHours, DEC = DEDeg };

            for (int i = Positions.Count - 1; i >= 0; i--)
            {
               if (xephem.Elongation(RAHours * 15, DEDeg, Positions[i].RA * 15, Positions[i].DEC) * 60 < 1)
               {
                   Positions.RemoveAt(i);
               }
            }

            if (Positions.Count == 0)
                Positions.Add(pos);
            else
                Positions.Insert(0, pos);

            while (Positions.Count > 10) Positions.RemoveAt(Positions.Count - 1);
        }

        public void Save()
        {
            try
            {
                var ser = new XmlSerializer(typeof(LastSlewPositions));
                var output = new StringBuilder();
                ser.Serialize(new StringWriter(output), this);

                Properties.Settings.Default.LastSlewPositions = output.ToString();
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
    }
}

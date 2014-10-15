/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using OccuRec.Utilities;

namespace OccuRec.OCR
{
    public class TimeStampArea
    {
        [XmlAttribute()]
        public int Top { get; set; }

        [XmlAttribute()]
        public int Left { get; set; }

        [XmlAttribute()]
        public int Width { get; set; }

        [XmlAttribute()]
        public int Height { get; set; }
    }

    public class AlignmentConfig
    {
        public int Width = 768;
        public int Height = 480;
        public int FrameTopOdd = 452;
        public int FrameTopEven = 453;

        public int CharWidth = 21;
        public int CharHeight = 12;

        [XmlArrayItem("Left")]
        public List<int> CharPositions = new List<int>();

        public static AlignmentConfig GetDefault()
        {
            var rv = new AlignmentConfig()
            {
                Width = 768,
                Height = 480,
                FrameTopOdd = 465,
                FrameTopEven = 466,
                CharWidth = 21,
                CharHeight = 12
            };

            rv.CharPositions = new List<int>(
            new int[]
				{
					9, 35, 59, 86, 112, 162, 188, 240, 266, 317, 342, 367, 393, 444, 470, 495, 521, 572, 598, 624, 649, 675, 700, 726
				});

            return rv;
        }

        // Digits: 3, 6, 9, 8, 0
        //
        // WHITE
        //
        //     10  11   12   13   14
        // 5   x   x    x    x    x 
        // 6   x   x    x    x    x
    }

    public class OcrZonePixel
    {
        [XmlAttribute()]
        public int X { get; set; }

        [XmlAttribute()]
        public int Y { get; set; }
    }

    public class OcrZone
    {
        public int ZoneId;
        public string ZoneName;

        [XmlArrayItem("Pixel")]
        public List<OcrZonePixel> Pixels = new List<OcrZonePixel>();

        public static List<OcrZone> GetDefaultZones()
        {
            var rv = new List<OcrZone>();

            var zone = new OcrZone() { ZoneId = 0, ZoneName = "Rounded-Id-1 (3,6,9,8,0)" };
            zone.Pixels.Add(new OcrZonePixel() { X = 10, Y = 5 });
            zone.Pixels.Add(new OcrZonePixel() { X = 11, Y = 5 });
            zone.Pixels.Add(new OcrZonePixel() { X = 12, Y = 5 });
            zone.Pixels.Add(new OcrZonePixel() { X = 13, Y = 5 });
            zone.Pixels.Add(new OcrZonePixel() { X = 14, Y = 5 });
            zone.Pixels.Add(new OcrZonePixel() { X = 10, Y = 6 });
            zone.Pixels.Add(new OcrZonePixel() { X = 11, Y = 6 });
            zone.Pixels.Add(new OcrZonePixel() { X = 12, Y = 6 });
            zone.Pixels.Add(new OcrZonePixel() { X = 13, Y = 6 });
            zone.Pixels.Add(new OcrZonePixel() { X = 14, Y = 6 });
            rv.Add(zone);

            return rv;
        }
    }


    public enum ZoneValue
    {
		On = 0,
		Off = 1,
		Gray = 2,
		NotOn = 3,
		NotOff = 4,
		OnOff = 5,
		OffOn = 6,
		NotOnOff = 7,
		NotOffOn = 8
    }

	public enum DefinitionMode
	{
		Standard = 0,
		SplitZones = 1
	}

    public class ZoneSignature
    {
        [XmlAttribute("Id")]
        public int ZoneId { get; set; }

        [XmlAttribute("Value")]
        public ZoneValue ZoneValue { get; set; }
    }

    public class CharDefinition
    {
        public CharDefinition()
        {
            ZoneSignatures = new List<ZoneSignature>();
        }

	    public string Character { get; set; }
        public int? FixedPosition { get; set; }

        [XmlArrayItem("Zone")]
        public List<ZoneSignature> ZoneSignatures { get; set; }

        public static List<CharDefinition> GetDefault()
        {
            var rv = new List<CharDefinition>();

            var charDef = new CharDefinition() { Character = "0" };
            charDef.ZoneSignatures.Add(new ZoneSignature() {ZoneId = 0, ZoneValue = ZoneValue.Off});
            rv.Add(charDef);

            return rv;
        }
    }
	public class OcrConfiguration
	{
		public OcrConfiguration(bool populateDefault)
        {
            Alignment = AlignmentConfig.GetDefault();
            Zones = OcrZone.GetDefaultZones();
            CharDefinitions = CharDefinition.GetDefault();
			SupportedDevices = new List<string>();
        }

		public OcrConfiguration()
        {
            Alignment = new AlignmentConfig();
            Zones = new List<OcrZone>();
            CharDefinitions = new List<CharDefinition>();
			SupportedDevices = new List<string>();
        }

		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public DefinitionMode Mode { get; set; }

		[XmlAttribute]
		public bool Hidden { get; set; }

		public AlignmentConfig Alignment { get; set; }

        public List<OcrZone> Zones { get; set; }

        public List<CharDefinition> CharDefinitions { get; set; }

		[XmlArrayItem("Device")]
		public List<string> SupportedDevices { get; set; }		
	}

	public class OcrSettings
    {
        private static OcrSettings s_OCRSettings = null;

        public static OcrSettings Instance
        {
            get
            {
                if (s_OCRSettings == null)
                {
					string xmlConfig = AssemblyHelper.GetEmbededResource("OccuRec", "OCR-Settings.xml");
					
                    var ser = new XmlSerializer(typeof(OcrSettings));
                    using (TextReader rdr = new StringReader(xmlConfig))
                    {
                        s_OCRSettings = (OcrSettings)ser.Deserialize(rdr);
                    }                                  
                }

                return s_OCRSettings;
            }            
        }

		public OcrConfiguration this[string configName]
		{
			get
			{
				OcrConfiguration rv = Configurations.SingleOrDefault(x => x.Name == configName);
				if (rv == null)
					rv = new OcrConfiguration(true);

				return rv;
			}
		}

        public string ToXml()
        {
            var ser = new XmlSerializer(typeof(OcrSettings));
            var output = new StringBuilder();

            using (TextWriter wrt = new StringWriter(output))
            {
                ser.Serialize(wrt, this);
                wrt.Flush();
            }

            return output.ToString();
        }

		public OcrSettings()
		{
			Configurations = new List<OcrConfiguration>();
		}

		[XmlElement("Configuration")]
		public List<OcrConfiguration> Configurations { get; set; }
    }
}

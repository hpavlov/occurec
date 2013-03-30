using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AAVRec.OCR
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

    public class OCRSettings
    {
        private static OCRSettings s_OCRSettings = null;

        public static OCRSettings Instance
        {
            get
            {
                if (s_OCRSettings == null)
                {
                    string configFileName = Path.GetFullPath(string.Format("{0}\\IOTA-VTI-OCR-Settings.xml", AppDomain.CurrentDomain.BaseDirectory));

                    if (!File.Exists(configFileName))
                    {
                        File.WriteAllText(configFileName, new OCRSettings().ToXml());
                    }

                    string xmlConfig = File.ReadAllText(configFileName);

                    var ser = new XmlSerializer(typeof(OCRSettings));
                    using (TextReader rdr = new StringReader(xmlConfig))
                    {
                        s_OCRSettings = (OCRSettings)ser.Deserialize(rdr);
                    }                                  
                }

                return s_OCRSettings;
            }            
        }

        public string ToXml()
        {
            var ser = new XmlSerializer(typeof(OCRSettings));
            var output = new StringBuilder();

            using (TextWriter wrt = new StringWriter(output))
            {
                ser.Serialize(wrt, this);
                wrt.Flush();
            }

            return output.ToString();
        }

        public TimeStampArea TimeStampArea1 { get; set; }
        public TimeStampArea TimeStampArea2 { get; set; }
    }
}

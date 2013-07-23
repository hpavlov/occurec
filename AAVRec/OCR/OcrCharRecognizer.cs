using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AAVRec.OCR
{
    public class OcrCharRecognizer
    {
        public static int MIN_ON_VALUE = 200;

        private List<OcrZone> zones = new List<OcrZone>();
        private List<CharDefinition> charDefinitions = new List<CharDefinition>();

        public OcrCharRecognizer(List<OcrZone> zones, List<CharDefinition> charDefinitions)
        {
            this.zones.AddRange(zones);
            this.charDefinitions.AddRange(charDefinitions);
        }

        public char RecognizeChar(double[] computedZones, int median, int charPosition)
        {
            int MAX_OFF_VALUE = median + (MIN_ON_VALUE - median) / 4;

            foreach (CharDefinition charDef in charDefinitions)
            {
                if (charDef.FixedPosition != null &&
                    charDef.FixedPosition.Value != charPosition)
                {
                    continue;
                }

                bool isMatch = true;

                foreach(ZoneSignature zoneSign in charDef.ZoneSignatures)
                {
                    if (zoneSign.ZoneValue == ZoneValue.On && computedZones[zoneSign.ZoneId] < MIN_ON_VALUE)
                    {
                        isMatch = false;
                        break;
                    }

                    if (zoneSign.ZoneValue == ZoneValue.Off && computedZones[zoneSign.ZoneId] >= MAX_OFF_VALUE)
                    {
                        isMatch = false;
                        break;                        
                    }

                    if (zoneSign.ZoneValue == ZoneValue.Gray && (computedZones[zoneSign.ZoneId] < MAX_OFF_VALUE || computedZones[zoneSign.ZoneId] > MIN_ON_VALUE))
                    {
                        isMatch = false;
                        break;
                    }

                    if (zoneSign.ZoneValue == ZoneValue.NotOn && computedZones[zoneSign.ZoneId] > MIN_ON_VALUE)
                    {
                        isMatch = false;
                        break;
                    }

                    if (zoneSign.ZoneValue == ZoneValue.NotOff && computedZones[zoneSign.ZoneId] < MAX_OFF_VALUE)
                    {
                        isMatch = false;
                        break;
                    }
                }   

                if (isMatch)
                    return charDef.Character[0];
            }

            return '\0';
        }
    }
}

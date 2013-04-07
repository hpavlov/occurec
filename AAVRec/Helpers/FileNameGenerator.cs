using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AAVRec.Properties;

namespace AAVRec.Helpers
{
    public static class FileNameGenerator
    {
        public static string GenerateFileName()
        {
            return Path.GetFullPath(string.Format("{0}\\video-{1}.avi", Settings.Default.OutputLocation, DateTime.Now.ToString("yyyy-MMM-dd HH-mm-ss")));
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AAVRec.Properties;

namespace AAVRec.Helpers
{
    public static class FileNameGenerator
    {
        private static Regex REGEX_FILEMASK = new Regex("\\d\\d\\d\\d\\-[a-z]{3}\\-\\d\\d \\d\\d\\-\\d\\d\\-\\d\\d \\((?<SeqNo>\\d+)\\).(avi|aav)", RegexOptions.IgnoreCase); 

        public static string GenerateFileName(bool isAAVFile)
        {
            IEnumerable<string> existingFiles = Directory.EnumerateFiles(Settings.Default.OutputLocation, "*.*", SearchOption.TopDirectoryOnly);
            List<int> existingSequenceIds = existingFiles
                .Select(x => REGEX_FILEMASK.Match(x).Groups["SeqNo"])
                .Where(g => g != null && !string.IsNullOrEmpty(g.Value))
                .Select(g => int.Parse(g.Value))
                .Distinct()
                .ToList();

            int nextNumber = existingSequenceIds.Any() ? existingSequenceIds.Max() + 1 : 1;

            return Path.GetFullPath(
                string.Format("{0}\\{1} ({2}).{3}", 
                    Settings.Default.OutputLocation, 
                    DateTime.Now.ToString("yyyy-MMM-dd HH-mm-ss"),
                    nextNumber,
                    isAAVFile ? "aav" : "avi"));
        }
    }
}

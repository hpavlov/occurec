using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OccuRec.Properties;

namespace OccuRec.Helpers
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

		public static void CheckAndWarnForFileSystemLimitation()
		{
			if (Directory.Exists(Settings.Default.OutputLocation))
			{
				CheckAndWarnForFileSystemLimitation(Settings.Default.OutputLocation, MessageBoxButtons.OK);
			}
		}


		public static DialogResult CheckAndWarnForFileSystemLimitation(string folderToCheck, MessageBoxButtons warningButtons)
		{
			string directoryRoot = Directory.GetDirectoryRoot(folderToCheck);

			DriveInfo[] allDrives = DriveInfo.GetDrives();

			try
			{
				DriveInfo outputDrive = allDrives.SingleOrDefault(x => x.RootDirectory.Name.Equals(directoryRoot));
				if (outputDrive != null)
				{
					string message = null;
					if (outputDrive.DriveFormat == "FAT" || outputDrive.DriveFormat == "FAT16")
					{
						// Limit of 2 Gb
						message = string.Format(
							"The file system on drive {0} is {1} and is limited to a single file size of 2 Gb. It is recommended to use an NTFS file system for the output video location.",
							directoryRoot, outputDrive.DriveFormat);
					}
					else if (outputDrive.DriveFormat == "FAT32")
					{
						// Limit of 4 Gb
						message = string.Format(
							"The file system on drive {0} is {1} and is limited to a single file size of 4 Gb. It is recommended to use an NTFS file system for the output video location.",
							directoryRoot, outputDrive.DriveFormat);
					}

					if (message != null)
						return MessageBox.Show(message, "OccuRec", warningButtons, MessageBoxIcon.Warning);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.GetFullStackTrace());
			}

			return DialogResult.OK;
		}
    }
}

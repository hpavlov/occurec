using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

namespace OccuRecUpdate.Schema
{
    class File
    {
        internal readonly string Path = null;
        internal readonly string LocalPath = null;
        internal readonly bool Archived = false;
        internal readonly string Action = null;

        internal Dictionary<int, string> LanguageSpecificFiles = new Dictionary<int, string>();

		//    <File Path="/OccuRec_3_0_0_0/OccuRec.zip" LocalPath="OccuRec.exe" Archived="true" />
		//    <File Path="/OccuRec_3_0_0_0/OccuRec.Core.zip" LocalPath="OccuRec.Core.dll" Archived="true" />
		//    <File Path="/OccuRec_3_0_0_0/OccuRec.SDK.dll" />
		//    <File Path="/OccuRec_3_0_0_0/OccuRec.EN.pdf" LocalPath="/Documentation/OccuRec.pdf" Action="ShellExecute">
		//      <Language Id="1" Path="/OccuRec_3_0_0_0/OccuRec.EN.pdf" />
		//      <Language Id="2" Path="/OccuRec_3_0_0_0/OccuRec.DE.pdf" />
        //    </File>
        internal File(XmlElement node)
        {
            Path = node.Attributes["Path"].Value;

            if (node.Attributes["LocalPath"] != null)
                LocalPath = node.Attributes["LocalPath"].Value;

            if (node.Attributes["Archived"] != null)
                Archived = Convert.ToBoolean(node.Attributes["Archived"].Value, CultureInfo.InvariantCulture);
            else
                Archived = false;

            if (node.Attributes["Action"] != null)
                Action = node.Attributes["Action"].Value;

            foreach (XmlNode langNode in node.SelectNodes("./Language"))
            {
                int langId = int.Parse(langNode.Attributes["Id"].Value, CultureInfo.InvariantCulture);
                string path = langNode.Attributes["Path"].Value;

                try
                {
                    LanguageSpecificFiles.Add(langId, path);
                }
                catch { }
            }
        }
    }
}

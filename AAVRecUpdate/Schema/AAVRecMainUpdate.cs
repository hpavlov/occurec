using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

namespace AAVRecUpdate.Schema
{
    class AAVRecMainUpdate : UpdateObject
    {
        //<Update File="AAVRec.exe" MustExist="true" Version="30000" ReleaseDate="21 Mar 2008" ModuleName="Occult Watcher">
        //    <File Path="/AAVRec_3_0_0_0/AAVRec.zip" LocalPath="AAVRec.exe" Archived="true" />
        //    <File Path="/AAVRec_3_0_0_0/AAVRec.Core.zip" LocalPath="AAVRec.Core.dll" Archived="true" />
        //    <File Path="/AAVRec_3_0_0_0/AAVRec.SDK.dll" />
        //</Update>
        public AAVRecMainUpdate(XmlElement node)
            : base(node)
        {
            if (node.Attributes["File"] == null)
                throw new InstallationAbortException("The update location points to an older version of AAVRec.");

            m_File = node.Attributes["File"].Value;
            m_Version = int.Parse(node.Attributes["Version"].Value, CultureInfo.InvariantCulture);
            if (node.Attributes["MustExist"] != null)
                m_MustExist = Convert.ToBoolean(node.Attributes["MustExist"].Value, CultureInfo.InvariantCulture);
            else
                m_MustExist = true;

            m_ReleaseDate = node.Attributes["ReleaseDate"].Value;

            if (node.Attributes["ModuleName"] != null)
                m_ModuleName = node.Attributes["ModuleName"].Value;
            else
                m_ModuleName = "AAVRec";
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OccuRec.FieldIdentification.UCAC2
{
    public class BSSBinaryFilesBuilder
    {
        private string m_Folder;

        public BSSBinaryFilesBuilder(string asciiFileLocation)
        {
            m_Folder = asciiFileLocation;
        }

        public bool Build(IWin32Window owner)
        {
            //// 36 zones, 5 deg each (-90, -85) (-85, -80)
            //string asciiFileName = Path.Combine(m_Folder, "ucac2bss.dat");
            //using(FileStream fs = new FileStream(asciiFileName, FileMode.Open, FileAccess.Read))
            //using (TextReader rdr = new StreamReader(fs))
            //{
            //    string line = rdr.ReadLine();

              
            //}

            return false;
        }

        public bool BuildIndex(IWin32Window owner)
        {
            // "bsindex.da"
            return false;
        }
    }
}

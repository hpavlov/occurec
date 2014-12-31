using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using OccuRec.Helpers;

namespace OccuRec
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();

            this.Text = String.Format("About {0}", AssemblyTitle);
            this.textBoxDescription.Text = AssemblyDescription;
            if (!string.IsNullOrEmpty(AssemblyReleaseDate))
            {
                this.lblProductName.Text = String.Format("{0} v{1}{2}, Released on {3}", AssemblyProduct, AssemblyFileVersion, IsBetaRelease ? " BETA" : "", AssemblyReleaseDate);
            }
            else
                this.lblProductName.Text = String.Format("{0} v{1}, Unreleased ALPHA Version", AssemblyProduct, AssemblyFileVersion);
        }

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }


        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }
        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public static string AssemblyFileVersion
        {
            get
            {
                object[] atts = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);
                if (atts != null && atts.Length == 1)
                    return ((AssemblyFileVersionAttribute)atts[0]).Version;
                else
                    return AssemblyVersion;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyReleaseDate
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(ReleaseDateAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((ReleaseDateAttribute)attributes[0]).ReleaseDate.ToString("dd MMM yyyy");
            }
        }

        public bool IsBetaRelease
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(BetaReleaseAttribute), false);
                return attributes.Length == 1;
            }
        }
    }
}

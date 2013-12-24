using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OccuRec.Config.Panels
{
	public class SettingsPanel : UserControl
	{
		public virtual void LoadSettings() { }
		public virtual void SaveSettings() { }
		public virtual void OnPostSaveSettings() { }
		public virtual void Reset() { }
		public virtual bool ValidateSettings()
		{
			return true;
		}
	}
}

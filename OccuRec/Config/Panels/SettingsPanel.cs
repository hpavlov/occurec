﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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

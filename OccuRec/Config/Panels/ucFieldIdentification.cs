/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.Astrometry.StarCatalogues;
using OccuRec.Helpers;
using OccuRec.Properties;
using OccuRec.Utilities;

namespace OccuRec.Config.Panels
{
	public partial class ucFieldIdentification : SettingsPanel
	{
		public ucFieldIdentification()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			cbxCatalogue.SelectedIndex = (int)Settings.Default.StarCatalog - 1;
			tbxCatalogueLocation.Text = Settings.Default.StarCatalogLocation;
			if (Guid.Empty != Settings.Default.StarCatalogMagnitudeBandId)
			{
				CatalogMagnitudeBand bnd = cbxCatalogPhotometryBand.Items.Cast<CatalogMagnitudeBand>().FirstOrDefault(mb => mb.Id == Settings.Default.StarCatalogMagnitudeBandId);
				if (bnd != null)
					cbxCatalogPhotometryBand.SelectedItem = bnd;
			}

			cbxFocalReducer.Checked = Settings.Default.FocalReducerUsed;
			nudFocalReducerValue.SetNUDValue(Settings.Default.FocalReducerValue);
		}

		public override void SaveSettings()
		{
			if (cbxCatalogue.SelectedIndex != -1)
			{
				Settings.Default.StarCatalog = (StarCatalog)(cbxCatalogue.SelectedIndex + 1);
				Settings.Default.StarCatalogLocation = tbxCatalogueLocation.Text;
				Settings.Default.StarCatalogMagnitudeBandId = ((CatalogMagnitudeBand)cbxCatalogPhotometryBand.SelectedItem).Id;
			}

			Settings.Default.FocalReducerUsed = cbxFocalReducer.Checked;
			Settings.Default.FocalReducerValue = (float)nudFocalReducerValue.Value; 
		}

		public override bool ValidateSettings()
		{
			if (cbxCatalogue.SelectedIndex != -1)
			{
				StarCatalog chosenCatalogue = (StarCatalog)(cbxCatalogue.SelectedIndex + 1);

				if (!Directory.Exists(tbxCatalogueLocation.Text))
				{
					tbxCatalogueLocation.Focus();
					MessageBox.Show(this, "Please select a valid folder", "Validation Error", MessageBoxButtons.OK,
									MessageBoxIcon.Error);
					return false;
				}

				string path = tbxCatalogueLocation.Text;
				if (!StarCatalogueFacade.IsValidCatalogLocation(chosenCatalogue, ref path))
				{
					tbxCatalogueLocation.Focus();
					MessageBox.Show(this,
									string.Format("Selected folder does not contain the {0} catalogue", chosenCatalogue),
									"Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
				tbxCatalogueLocation.Text = path;
			}

			return true;
		}

		private void cbxCatalogue_SelectedIndexChanged(object sender, EventArgs e)
		{
			// For each supported catalog we provide custom options for magnitude band to use for photometry
			StarCatalog catalog = (StarCatalog)(cbxCatalogue.SelectedIndex + 1);
			switch (catalog)
			{
				case StarCatalog.UCAC3:
					cbxCatalogPhotometryBand.Items.Clear();
					cbxCatalogPhotometryBand.Items.AddRange(StarCatalogueFacade.MagnitudeBandsForCatalog(catalog));
					break;

				case StarCatalog.UCAC2:
					cbxCatalogPhotometryBand.Items.Clear();
					cbxCatalogPhotometryBand.Items.AddRange(StarCatalogueFacade.MagnitudeBandsForCatalog(catalog));
					break;

				case StarCatalog.NOMAD:
					cbxCatalogPhotometryBand.Items.Clear();
					cbxCatalogPhotometryBand.Items.AddRange(StarCatalogueFacade.MagnitudeBandsForCatalog(catalog));
					break;

				case StarCatalog.PPMXL:
					cbxCatalogPhotometryBand.Items.Clear();
					cbxCatalogPhotometryBand.Items.AddRange(StarCatalogueFacade.MagnitudeBandsForCatalog(catalog));
					break;

				case StarCatalog.UCAC4:
					cbxCatalogPhotometryBand.Items.Clear();
					cbxCatalogPhotometryBand.Items.AddRange(StarCatalogueFacade.MagnitudeBandsForCatalog(catalog));
					break;
			}

			if (cbxCatalogPhotometryBand.Items.Count > 0)
				cbxCatalogPhotometryBand.SelectedIndex = 0;
		}

		private void btnBrowseLocation_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
				tbxCatalogueLocation.Text = folderBrowserDialog.SelectedPath;
		}

		private void cbxFocalReducer_CheckedChanged(object sender, EventArgs e)
		{
			nudFocalReducerValue.Enabled = cbxFocalReducer.Checked;
		}

	}
}

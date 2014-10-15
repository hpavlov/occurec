/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.ASCOM.Wrapper.Interfaces;
using OccuRec.Helpers;

namespace OccuRec.Controls
{
	public partial class ucCameraControl : UserControl
	{
		private VideoWrapper m_VideoWrapper;

		public ucCameraControl()
		{
			InitializeComponent();
		}

		private void ucCameraControl_Load(object sender, EventArgs e)
		{

		}

		internal void Initialize(VideoWrapper videoWrapper)
		{
			m_VideoWrapper = videoWrapper;
			UpdateControls();
		}


		private void UpdateControls()
		{
			if (m_VideoWrapper.SupportsGamma)
			{
				btnGammaDown.Enabled = m_VideoWrapper.CanDecreaseGamma;
				btnGammaUp.Enabled = m_VideoWrapper.CanIncreaseGamma;
				lblGamma.Text = m_VideoWrapper.Gamma;
			}
			else
			{
				btnGammaDown.Enabled = false;
				btnGammaUp.Enabled = false;
				lblGamma.Text = "";
			}

			if (m_VideoWrapper.SupporstGain)
			{
				btnGainDown.Enabled = m_VideoWrapper.CanDecreaseGain;
				btnGainUp.Enabled = m_VideoWrapper.CanIncreaseGain;
				lblGain.Text = m_VideoWrapper.Gain;
			}
			else
			{
				btnGainDown.Enabled = false;
				btnGainUp.Enabled = false;
				lblGain.Text = "";
			}

			btnExposureDown.Enabled = m_VideoWrapper.CanIncreaseIntegration;
			btnExposureUp.Enabled = m_VideoWrapper.CanDecreaseIntegration; 
			lblExposure.Text = m_VideoWrapper.Integration;
		}

		private void btnGammaUp_Click(object sender, EventArgs e)
		{
			m_VideoWrapper.IncreaseGamma();
			UpdateControls();
		}

		private void btnGammaDown_Click(object sender, EventArgs e)
		{
			m_VideoWrapper.DecreaseGamma();
			UpdateControls();
		}

		private void btnGainUp_Click(object sender, EventArgs e)
		{
			m_VideoWrapper.IncreaseGain();
			UpdateControls();
		}

		private void btnGainDown_Click(object sender, EventArgs e)
		{
			m_VideoWrapper.DecreaseGain();
			UpdateControls();
		}

		private void btnExposureDown_Click(object sender, EventArgs e)
		{
			m_VideoWrapper.IncreaseIntegration();
			UpdateControls();
		}

		private void btnExposureUp_Click(object sender, EventArgs e)
		{
			m_VideoWrapper.DecreaseIntegration();
			UpdateControls();
		}
	}
}

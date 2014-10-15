/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using ASCOM;
using OccuRec.ASCOM.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Utilities;

namespace OccuRec.ASCOM.Server
{
	[Serializable]
	public class IsolatedFocuser : IsolatedDevice, IASCOMFocuser
	{
		private global::ASCOM.DriverAccess.Focuser m_Focuser;

		internal IsolatedFocuser(string progId)
		{
			m_Focuser = new global::ASCOM.DriverAccess.Focuser(progId);
			SetIsolatedDevice(m_Focuser, progId);
		}

		public FocuserState GetCurrentState()
		{
			var rv = new FocuserState();

			rv.TempCompAvailable = m_Focuser.TempCompAvailable;

            try
            {
                rv.Temperature = m_Focuser.Temperature;
            }
            catch (PropertyNotImplementedException)
            {
                rv.Temperature = double.NaN;
            }
			
			rv.IsMoving = m_Focuser.IsMoving;
			rv.MaxIncrement = m_Focuser.MaxIncrement;
			rv.MaxStep = m_Focuser.MaxStep;
			rv.Absolute = m_Focuser.Absolute;

            try
            {
                rv.StepSize = m_Focuser.StepSize;
            }
			catch (PropertyNotImplementedException)
            {
                rv.StepSize = double.NaN;
            }

			
			rv.TempComp = m_Focuser.TempComp;

            try
            {
                if (rv.Absolute)
                    rv.Position = m_Focuser.Position;
                else
                    rv.Position = 0;
            }
			catch (PropertyNotImplementedException)
            {
                rv.Position = 0;
            }

			return rv;
		}

        public void Configure()
        {
            try
            {
                m_Focuser.SetupDialog();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }
        }

		public void Move(int position)
		{
            Trace.WriteLine(string.Format("OccuRec: ASCOMServer::{0}(Focuser)::Move({1})", ProgId, position));
            try
            {
                m_Focuser.Move(position);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }

            while (m_Focuser.IsMoving)
            {
                Thread.Sleep(100);
            }
		}

        public bool ChangeTempComp(bool tempComp)
        {
            try
            {
                m_Focuser.TempComp = tempComp;
                Trace.WriteLine(string.Format("OccuRec: ASCOMServer::{0}(Focuser):TempComp = {1}", ProgId, tempComp));

                return m_Focuser.TempComp == tempComp;
            }
			catch (PropertyNotImplementedException)
            { }

            return false;
        }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Wrapper.Interfaces;
using OccuRec.ASCOM.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccuRec.ASCOM.Wrapper.Devices
{
    internal class Focuser : DeviceBase, IFocuser
	{
	    private static double LARGE_TO_SMALL_STEP_FACTOR = 5.0;
        private static double SMALL_TO_SMALLEST_STEP_FACTOR = 10.0;

		private IASCOMFocuser m_IsolatedFocuser;

	    private bool m_FeaturesKnown;
	    private bool m_SupportsTempComp;
        private bool m_IsAbsolute;
	    private double m_MaxIncrement;
        private double m_MaxStep;
	    private int m_CurrentPosition;
        private int m_StartingPosition;

        private int m_LargeStepSize;
        private int m_SmallStepSize;
        private int m_SmallestStepSize;

        internal Focuser(IASCOMFocuser isolatedFocuser, int largeStepSize, int smallStepSize, int smallestStepSize)
			: base(isolatedFocuser)
		{
			m_IsolatedFocuser = isolatedFocuser;
		    m_FeaturesKnown = false;

		    m_StartingPosition = 0;
            m_CurrentPosition = 0;

            m_LargeStepSize = largeStepSize;
            m_SmallStepSize = smallStepSize;
            m_SmallestStepSize = smallestStepSize;
		}

		public FocuserState GetCurrentState()
		{
			FocuserState state = m_IsolatedFocuser.GetCurrentState();

		    if (m_IsAbsolute)
		        m_CurrentPosition = state.Position;
		    else
                // In Relative mode the position value is kept internally
		        state.Position = m_CurrentPosition;

		    return state;
		}

        private void GetFocuserFeatures(bool force)
        {
            if (force || !m_FeaturesKnown)
            {
                FocuserState state = m_IsolatedFocuser.GetCurrentState();
                if (state != null)
                {
                    m_SupportsTempComp = state.TempCompAvailable;
                    m_IsAbsolute = state.Absolute;
                    m_MaxIncrement = state.MaxIncrement;
                    m_MaxStep = state.MaxStep;

                    if (m_IsAbsolute)
                    {
                        m_StartingPosition = state.Position;
                        m_CurrentPosition = state.Position;
                    }
                    else
                    {
                        m_StartingPosition = 0;
                        m_CurrentPosition = 0;
                    }
                }

                m_FeaturesKnown = true;
            }
        }

        protected override void OnConnected()
        {
            GetFocuserFeatures(true);
        }

		public void Move(int position)
		{
			m_IsolatedFocuser.Move(position);
		}

        private bool EnsureFocuserFeaturesKnown()
        {
            if (!m_FeaturesKnown) 
                GetFocuserFeatures(false);

            return m_FeaturesKnown;
        }

        private int GetStepSize(FocuserStepSize stepSize)
        {
            int step = 0;
            if (stepSize == FocuserStepSize.Large)
            {
                if (m_LargeStepSize == -1)
                    step = (int) m_MaxIncrement/10;
                else
                    step = m_LargeStepSize;
            }
            else if (stepSize == FocuserStepSize.Small)
            {
                if (m_SmallStepSize == -1)
                    step = (int)(m_MaxIncrement / (10 * LARGE_TO_SMALL_STEP_FACTOR));
                else
                    step = m_SmallStepSize;
            }
            else if (stepSize == FocuserStepSize.Smallest)
            {
                if (m_SmallestStepSize == -1)
                    step = (int)(m_MaxIncrement / (10 * SMALL_TO_SMALLEST_STEP_FACTOR));
                else
                    step = m_SmallestStepSize;
            }
            else
                throw new ArgumentOutOfRangeException();

            return step;
        }

        public void MoveIn(FocuserStepSize stepSize)
        {
            if (EnsureFocuserFeaturesKnown())
            {
                if (!m_IsAbsolute)
                {
                    int step = -1 * GetStepSize(stepSize);

                    m_IsolatedFocuser.Move(step);

                    m_CurrentPosition += step;
                }
                else
                {
                    // Absolute position focuser are currently not supported
                }
            }
        }

        public void MoveOut(FocuserStepSize stepSize)
        {
            if (EnsureFocuserFeaturesKnown())
            {
                if (!m_IsAbsolute)
                {
                    int step = GetStepSize(stepSize);

                    m_IsolatedFocuser.Move(step);

                    m_CurrentPosition += step;
                }
                else
                {
                    // Absolute position focuser are currently not supported
                }
            }
        }

        public bool ChangeTempComp(bool tempComp)
        {
            return m_IsolatedFocuser.ChangeTempComp(tempComp);
        }
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OccRec.ASCOMWrapper.Devices;
using OccRec.ASCOMWrapper.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Helpers;

namespace OccuRec.ASCOM
{
	internal static class FocuserCommands
	{
		internal static void GetFocuserState(Signal signal, IFocuser focuser)
		{
			try
			{
				Action<FocuserState> callback = signal.Argument as Action<FocuserState>;
				if (focuser != null && focuser.Connected)
				{
					FocuserState state = focuser.GetCurrentState();

                    ASCOMHelper.SafeCallbackActionCall(callback, state);
				}
				else
				{
                    ASCOMHelper.SafeCallbackActionCall(callback, null);
				}

			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.GetFullStackTrace());
			}		
		}

        internal static void MoveFocuserAndGetState(Signal signal, bool moveIn, IFocuser focuser)
		{
			try
			{
                if (signal.Command == ControllerSignals.MoveFocuserAndGetState)
                {
                    var tuple = signal.Argument as Tuple<int, Action<FocuserState>>;
                    Action<FocuserState> callback = tuple != null ? tuple.Item2 as Action<FocuserState> : null;
                    int step = tuple != null ? (int)tuple.Item1: 0;

                    if (focuser != null && focuser.Connected)
                    {
                        focuser.Move(step);

                        FocuserState state = focuser.GetCurrentState();

                        ASCOMHelper.SafeCallbackActionCall(callback, state);
                    }
                    else
                    {
                        ASCOMHelper.SafeCallbackActionCall(callback, null);
                    }
                }
                else
                {
                    var tuple = signal.Argument as Tuple<FocuserStepSize, Action<FocuserState>>;
                    Action<FocuserState> callback = tuple != null ? tuple.Item2 as Action<FocuserState> : null;
                    FocuserStepSize stepSize = tuple != null ? (FocuserStepSize)tuple.Item1 : FocuserStepSize.Small;

                    if (focuser != null && focuser.Connected)
                    {
                        if (moveIn)
                            focuser.MoveIn(stepSize);
                        else
                            focuser.MoveOut(stepSize);

                        FocuserState state = focuser.GetCurrentState();

                        ASCOMHelper.SafeCallbackActionCall(callback, state);
                    }
                    else
                    {
                        ASCOMHelper.SafeCallbackActionCall(callback, null);
                    }
                }

			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.GetFullStackTrace());
			}
		}
	}
}

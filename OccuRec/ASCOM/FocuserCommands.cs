using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Helpers;

namespace OccuRec.ASCOM
{
	internal static class FocuserCommands
	{
		private static void SafeCallbackActionCall<TArgument>(Action<TArgument> callback, TArgument value)
		{
			if (callback != null)
			{
				try
				{
					callback(value);
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.GetFullStackTrace());
				}
			}			
		}

		internal static void GetFocuserState(Signal signal, IASCOMFocuser focuser)
		{
			try
			{
				Action<FocuserState> callback = signal.Argument as Action<FocuserState>;
				if (focuser != null && focuser.Connected)
				{
					FocuserState state = focuser.GetCurrentState();

					SafeCallbackActionCall(callback, state);
				}
				else
				{
					SafeCallbackActionCall(callback, null);
				}

			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.GetFullStackTrace());
			}		
		}

		internal static void MoveFocuserAndGetState(Signal signal, IASCOMFocuser focuser)
		{
			try
			{
				var tuple = signal.Argument as Tuple<int, Action<FocuserState>>;
				Action<FocuserState> callback = tuple != null ? tuple.Item2 as Action<FocuserState>: null;
				int position = tuple != null ? Convert.ToInt32(tuple.Item1) : 0;

				if (focuser != null && focuser.Connected)
				{
					focuser.Move(position);
					FocuserState state = focuser.GetCurrentState();

					SafeCallbackActionCall(callback, state);
				}
				else
				{
					SafeCallbackActionCall(callback, null);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.GetFullStackTrace());
			}
		}
	}
}

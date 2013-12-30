using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OccuRec.Helpers;
using OccuRec.Utilities;

namespace OccuRec.ASCOM
{
    internal class ASCOMHelper
    {
        public static void SafeCallbackActionCall<TArgument>(Action<TArgument> callback, TArgument value)
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

        public static void SafeCallbackActionCall(Action callback)
        {
            if (callback != null)
            {
                try
                {
                    callback();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.GetFullStackTrace());
                }
            }
        }
    }
}

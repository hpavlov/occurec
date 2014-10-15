/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OccuRec.Utilities;

namespace OccuRec.ASCOM
{
	internal abstract class ThreadIsolatedInvoker
	{
		private class InvocationDescriptor
		{
			public Delegate Delegate;
			public object[] Arguments;
			public object ReturnValue;
			public bool InvocationCompleted;
			public SynchronizationContext SynchronisationContext;
			public Control CallbackControl;
			public CallbackAction Callback;
		}

		private static object s_SyncLock = new object();
		private static bool s_Running = false;
		private static ConcurrentQueue<InvocationDescriptor> s_Queue = new ConcurrentQueue<InvocationDescriptor>();

		static ThreadIsolatedInvoker()
		{
			if (!s_Running)
			{
				lock (s_SyncLock)
				{
					if (!s_Running)
					{
						s_Running = true;
						ThreadPool.QueueUserWorkItem(WorkerThread);
					}
				}
			}
		}

		private static void WorkerThread(object state)
		{
			// TODO: Can we also start a message loop here?

			while (s_Running)
			{
				while (s_Queue.Count > 0)
				{
					InvocationDescriptor item;
					if (s_Queue.TryDequeue(out item))
					{
						item.ReturnValue = item.Delegate.DynamicInvoke(item.Arguments);
						item.InvocationCompleted = true;

						if (item.Callback != null)
						{
                            try
                            {
                                if (item.SynchronisationContext != null)
                                    item.SynchronisationContext.Post((s) => item.Callback.DynamicInvoke(new ObservatoryControllerCallbackArgs(item.ReturnValue)), null);
                                else if (item.CallbackControl != null)
                                    item.CallbackControl.BeginInvoke(item.Callback, new ObservatoryControllerCallbackArgs(item.ReturnValue));
                            }
		                    catch (ObjectDisposedException)
		                    { }
						}
					}
				}

                Thread.Sleep(10);
			}
		}

        public static void ShieldedInvoke<TArgument>(Action<TArgument> callback, TArgument value)
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

        public static void ShieldedInvoke(Action callback)
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

        protected void IsolatedAction<T>(Func<T, object> action, T value1, CallType callType, CallbackAction callback, Control callbackUIControl)
        {
            IsolatedActionInternal(action, callType, callback, callbackUIControl, new object[] { value1 });
        }

        protected void IsolatedAction<T1, T2>(Func<T1, T2, object> action, T1 value1, T2 value2, CallType callType, CallbackAction callback, Control callbackUIControl)
        {
            IsolatedActionInternal(action, callType, callback, callbackUIControl, new object[] { value1, value2 });
        }

	    protected void IsolatedAction(Func<object> action, CallType callType, CallbackAction callback, Control callbackUIControl)
	    {
            IsolatedActionInternal(action, callType, callback, callbackUIControl, new object[] { });
	    }

	    private void IsolatedActionInternal(Delegate action, CallType callType, CallbackAction callback, Control callbackUIControl, object[] arguments)
		{
			var item = new InvocationDescriptor()
			{
                Delegate = action,
				Arguments = arguments,
				ReturnValue = null,
				InvocationCompleted = false
			};

			if (callback != null)
			{
				if (callType != CallType.Async)
					throw new InvalidOperationException("Callbacks are only supported with Async calls.");

				item.Callback = callback;

				if (callbackUIControl != null)
					item.CallbackControl = callbackUIControl;
				else
					item.SynchronisationContext = WindowsFormsSynchronizationContext.Current;
			}

			s_Queue.Enqueue(item);

			if (callType == CallType.Sync)
				SpinWait.SpinUntil(() => item.InvocationCompleted);
		}
	}
}

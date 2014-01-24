using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
					}
				}
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

		protected TResult IsolatedFunc<TResult>(Func<TResult> callback)
		{
			var item = new InvocationDescriptor()
			{
				Delegate = callback,
				Arguments = new object[] { },
				ReturnValue = null,
				InvocationCompleted = false
			};

			s_Queue.Enqueue(item);

			SpinWait.SpinUntil(() => item.InvocationCompleted);
			return (TResult)item.ReturnValue;
		}

		protected TResult IsolatedFunc<TResult, T>(Func<T, TResult> callback, T value1)
		{
			var item = new InvocationDescriptor()
			{
				Delegate = callback,
				Arguments = new object[] { value1 },
				ReturnValue = null,
				InvocationCompleted = false
			};

			s_Queue.Enqueue(item);

			SpinWait.SpinUntil(() => item.InvocationCompleted);
			return (TResult)item.ReturnValue;
		}

		protected TResult IsolatedFunc<TResult, T1, T2>(Func<T1, T2, TResult> callback, T1 value1, T2 value2)
		{
			var item = new InvocationDescriptor()
			{
				Delegate = callback,
				Arguments = new object[] { value1, value2 },
				ReturnValue = null,
				InvocationCompleted = false
			};

			s_Queue.Enqueue(item);

			SpinWait.SpinUntil(() => item.InvocationCompleted);
			return (TResult)item.ReturnValue;
		}

		protected void IsolatedAction(Action callback)
		{
			IsolatedAction(callback, true);
		}

		protected void IsolatedAction<T>(Action<T> callback, T value1)
		{
			IsolatedAction(callback, value1, true);
		}

		protected void IsolatedAction<T1, T2>(Action<T1, T2> callback, T1 value1, T2 value2)
		{
			IsolatedAction(callback, value1, value2, true);
		}

		protected void IsolatedActionAsync(Action callback)
		{
			IsolatedAction(callback, false);
		}

		protected void IsolatedActionAsync<T>(Action<T> callback, T value1)
		{
			IsolatedAction(callback, value1, false);
		}

		protected void IsolatedActionAsync<T1, T2>(Action<T1, T2> callback, T1 value1, T2 value2)
		{
			IsolatedAction(callback, value1, value2, false);
		}

		private void IsolatedAction(Action callback, bool blocking)
		{
			var item = new InvocationDescriptor()
			{
				Delegate = callback,
				Arguments = new object[] { },
				ReturnValue = null,
				InvocationCompleted = false
			};

			s_Queue.Enqueue(item);

			if (blocking)
				SpinWait.SpinUntil(() => item.InvocationCompleted);
		}

		private void IsolatedAction<T>(Action<T> callback, T value1, bool blocking)
		{
			var item = new InvocationDescriptor()
			{
				Delegate = callback,
				Arguments = new object[] { value1 },
				ReturnValue = null,
				InvocationCompleted = false
			};

			s_Queue.Enqueue(item);

			SpinWait.SpinUntil(() => item.InvocationCompleted);
		}

		private void IsolatedAction<T1, T2>(Action<T1, T2> callback, T1 value1, T2 value2, bool blocking)
		{
			var item = new InvocationDescriptor()
			{
				Delegate = callback,
				Arguments = new object[] { value1, value2 },
				ReturnValue = null,
				InvocationCompleted = false
			};

			s_Queue.Enqueue(item);

			SpinWait.SpinUntil(() => item.InvocationCompleted);
		}
	}
}

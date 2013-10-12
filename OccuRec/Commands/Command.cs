using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace OccuRec.Commands
{
	public enum ObjectCallType
	{
		Asynchronous = 0,
		Synchronous = 1
	}

	public interface IBaseObjectProxy
	{
		int Timeout { get; set; }
		ObjectCallType CallType { get; set; }
	}

	public interface ICommand
	{
		void Execute();
		ObjectCallType CallType { get; }
		Exception Error { get; set; }

		/// <summary>
		/// Used to store the original call stack that initiated the command call before it was queued in a different 
		/// thread so in a case of an error the original invoker can be determined
		/// </summary>
		StackTrace CallStack { get; set; }
	}

	public abstract class Command<TDelegateObject> : ICommand
		where TDelegateObject : IBaseObjectProxy
	{
		protected Command(TDelegateObject delegateObject, int timeout)
		{
			DelegateObject = delegateObject;
			Timeout = timeout;
		}

		protected TDelegateObject DelegateObject { get; private set; }

		public int Timeout { get; set; }

		public Exception Error { get; set; }

		public ObjectCallType CallType
		{
			get { return DelegateObject.CallType; }
		}

		public StackTrace CallStack { get; set; }

		public void Execute()
		{
			int oldTimeout = DelegateObject.Timeout;
			DelegateObject.Timeout = Timeout;

			try
			{
				DoExecute();
			}
			catch (Exception ex)
			{
				Error = ex;
			}
			finally
			{
				DelegateObject.Timeout = oldTimeout;
			}
		}

		protected abstract void DoExecute();
	}

	public class CommandCallResult
	{
		public StackTrace OriginatingCallStack;
		public Exception CallbackException;
		public bool Success;
	}

	[Serializable]
	public class CommandCallbackException : Exception
	{
		public CommandCallResult CommandCallResult;

		public CommandCallbackException(CommandCallResult result, Exception innerException)
			: base("An error occured while calling a command callback for a command invoked\r\n" + (result.OriginatingCallStack != null ? result.OriginatingCallStack.ToString() : "[no callstack available]"), result.CallbackException ?? innerException)
		{
			CommandCallResult = result;
		}
	}

	public abstract class CommandWithCallback<TDelegateObject, TReturnType> : Command<TDelegateObject>
		where TDelegateObject : IBaseObjectProxy
	{
		public delegate void CallbackType(CommandCallResult result, TReturnType returnValue);

		private CallbackType _callback;

		protected CommandWithCallback(TDelegateObject delegateObject, int timeout, CallbackType callback)
			: base(delegateObject, timeout)
		{
			_callback = callback;
		}

		protected override sealed void DoExecute()
		{
			bool success = false;
			const int MAX_CALL_ATTEMPTS = 2;

			TReturnType returnValue = default(TReturnType);

			for (int callAttempt = 1; callAttempt <= MAX_CALL_ATTEMPTS; callAttempt++)
			{
				try
				{
					returnValue = DoObjectCall();
					success = true;
				}
				catch (Exception ex)
				{
					Trace.WriteLine(string.Format("Error executing command invoked by:\r\n {0}\r\n\r\nThe error is:\r\n{1}", CallStack != null ? CallStack.ToString() : string.Empty, ex.ToString()));

					Error = ex;
					returnValue = default(TReturnType);
					success = false;
				}

				break;
			}


			var commandResult = new CommandCallResult()
			{
				OriginatingCallStack = base.CallStack,
				Success = success
			};
			try
			{
				_callback(commandResult, returnValue);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(string.Format("Error executing a callback of a command invoked by:\r\n {0}\r\n\r\nThe error is:\r\n{1}", CallStack != null ? CallStack.ToString() : string.Empty, ex.ToString()));

				throw new CommandCallbackException(commandResult, ex);
			}
		}

		protected abstract TReturnType DoObjectCall();
	}

}

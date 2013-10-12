using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.Commands
{
	public interface ICommandFiring
	{
		event EventHandler CommandFiring;
	}

	public interface ISupportsSyncAsyncCalls
	{
		ObjectCallType CallType { get; set; }
	}

	public abstract class DelegateObjectProxy<TDelegateObject> : ICommandFiring, ISupportsSyncAsyncCalls
		where TDelegateObject : IBaseObjectProxy
	{
		#region ICommandFiring Members
#pragma warning disable 67
		public event EventHandler CommandFiring;
#pragma warning restore 67
		#endregion

		protected TDelegateObject DelegateObject { get; private set; }

		protected CommandQueue Queue { get; private set; }

		protected DelegateObjectProxy(TDelegateObject delegateObject)
		{
			DelegateObject = delegateObject;
			Queue = new CommandQueue();
		}

		~DelegateObjectProxy()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			//TODO: Dispose() doesn't do anything. Remove the IDisposable implementation from all interfaces of classes derived from DelegateObjectProxy<TService>!
		}

		#region ISupportsSyncAsyncCalls Members

		public ObjectCallType CallType
		{
			get
			{
				return DelegateObject.CallType;
			}
			set
			{
				DelegateObject.CallType = value;
			}
		}

		#endregion
	}

	public interface ISampleObjectInterface : IBaseObjectProxy
	{
		
	}

	public interface ISampleObjectProxy
	{
		
	}

	public class ConnectCommand : CommandWithCallback<ISampleObjectInterface, bool>
	{
		public delegate void CallbackType();

		public ConnectCommand(ISampleObjectInterface delegateObject, int timeout, CommandWithCallback<ISampleObjectInterface, bool>.CallbackType callback)
			: base(delegateObject, timeout, callback)
		{
			
		}

		protected override bool DoObjectCall()
		{
			throw new NotImplementedException();
		}
	}

	public class SampleObjectProxy : DelegateObjectProxy<ISampleObjectInterface>, ISampleObjectProxy
	{
		public SampleObjectProxy(ISampleObjectInterface delegateObject) :
			base(delegateObject)
		{
		}

		public void SaveEventAsPending(int timeout, CommandWithCallback<ISampleObjectInterface, bool>.CallbackType callback)
		{
			this.Queue.Add(new ConnectCommand(this.DelegateObject, timeout, callback));
		}
	}
}

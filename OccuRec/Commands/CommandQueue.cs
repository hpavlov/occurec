using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace OccuRec.Commands
{
	internal class CommandExecutionContext
	{
		internal ICommand Command;
		internal ManualResetEvent SyncRoot;

		internal CommandExecutionContext(ICommand command)
		{
			Command = command;

			if (command.CallType == ObjectCallType.Synchronous)
				SyncRoot = new ManualResetEvent(true);
		}
	}

	public class CommandQueue
	{
		public static bool Closing { get; set; }

		Queue<ICommand> _commandQueue;
		AutoResetEvent _newCommandsEvent = new AutoResetEvent(false);
		object _lockObject = new object();

		public CommandQueue()
		{
			_commandQueue = new Queue<ICommand>();
		}

		public WaitHandle NewCommandWaitHandle
		{
			get { return _newCommandsEvent; }
		}

		public void Add(ICommand command)
		{

			//If we are closing, do not queue any more commands!
			if (Closing)
				return;

			var context = new CommandExecutionContext(command);
			command.CallStack = new StackTrace();

			if (command.CallType == ObjectCallType.Synchronous)
				ExecuteCommandBlocking(context);
			else
				ExecuteCommandAsync(context);
		}

		private void ExecuteCommandBlocking(CommandExecutionContext context)
		{
			context.SyncRoot.Reset();

			// Start a new thread to execute the command
			ThreadPool.QueueUserWorkItem(new WaitCallback(ExecuteCommandWorker), context);

			bool executionAndCallbacksCompleted = false;
			do
			{
				executionAndCallbacksCompleted = context.SyncRoot.WaitOne(100);

				// Because the command may do a callback on the UI thread it is important to call DoEvents()
				// This will run the message pump to deliver the callback to the UI thread and this is how we can
				// guarantee that the UI callback will be executed before this current method returns
				System.Windows.Forms.Application.DoEvents();
			}
			while (!executionAndCallbacksCompleted && !Closing);

		}

		private void ExecuteCommandAsync(CommandExecutionContext context)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(ExecuteCommandWorker), context);
		}

		private void ExecuteCommandWorker(object state)
		{
			var context = (CommandExecutionContext)state;

			try
			{
				context.Command.Execute();
			}
			catch (Exception ex)
			{
				context.Command.Error = ex;
			}

			var disp = context.Command as IDisposable;

			if (disp != null)
				disp.Dispose();

			if (context.SyncRoot != null)
				context.SyncRoot.Set();
		}
	}
}

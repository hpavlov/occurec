using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OccuRec.Helpers
{
    public static class Extensions
    {
		public static string GetFullStackTrace(this Exception ex)
		{
			var output = new StringBuilder();

			AddExceptionInfo(ex, ref output);

			return output.ToString();			
		}

		public static string GetFullStackTrace(this Exception ex, string message)
		{
			var output = new StringBuilder(message);
			output.Append("\r\n");

			AddExceptionInfo(ex, ref output);

			return output.ToString();
		}

		private static void AddExceptionInfo(Exception ex, ref StringBuilder output)
		{
			if (ex != null && output != null)
			{
				output.Append(ex.GetType().ToString());
				output.Append(" : ");
				output.Append(ex.Message);
				output.Append("\r\n");
				output.Append(ex.StackTrace);
				output.Append("\r\n");
				output.Append("--------------------------------------------------------------------------------------------------\r\n");

				if (ex.InnerException != null)
					AddExceptionInfo(ex.InnerException, ref output);
			}
		}
    }
}

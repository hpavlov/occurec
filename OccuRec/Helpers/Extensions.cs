using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OccuRec.Tracking;

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

		public static void SetNUDValue(this NumericUpDown nud, double value)
		{
			if (!double.IsNaN(value))
				SetNUDValue(nud, (decimal)value);
		}

		public static void SetNUDValue(this NumericUpDown nud, int value)
		{
			SetNUDValue(nud, (decimal)value);
		}

		public static void SetNUDValue(this NumericUpDown nud, decimal value)
		{
			if (value < nud.Minimum)
				nud.Value = nud.Minimum;
			else if (value > nud.Maximum)
				nud.Value = nud.Maximum;
			else
				nud.Value = value;
		}

		public static void SetCBXIndex(this ComboBox cbx, int index)
		{
			if (cbx.Items.Count > 0)
				cbx.SelectedIndex = Math.Max(0, Math.Min(cbx.Items.Count - 1, index));
			else
				cbx.SelectedIndex = -1;
		}
    }
}

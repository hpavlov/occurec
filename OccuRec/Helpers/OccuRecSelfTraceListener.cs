using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace OccuRec.Helpers
{
    public class OccuRecSelfTraceListener : TraceListener
    {
        public static OccuRecSelfTraceListener Instance = new OccuRecSelfTraceListener();

        private StringBuilder m_TraceLog = new StringBuilder();
        private bool m_Collecting = true;

        public override void Write(string message)
        {
            m_TraceLog.Append(message);
        }

        public override void WriteLine(string message)
        {
            m_TraceLog.AppendLine(message);
        }

        public void Initialize()
        {
            Trace.Listeners.Add(this);
        }

        public void Stop()
        {
            Trace.Listeners.Remove(this);
            m_Collecting = false;
        }

        public void SaveLog(string fileName)
        {
            if (!m_Collecting) return;

            var log = m_TraceLog.ToString();
            m_TraceLog.Clear();

            try
            {
                File.WriteAllText(fileName, log);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
    }
}

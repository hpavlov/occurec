using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Services;
using System.Text;

namespace OccRec.ASCOMWrapper
{
	internal class TrackingHandler : ITrackingHandler
	{
		// Notifies a handler that an object has been marshaled.
		public void MarshaledObject(Object obj, ObjRef or)
		{
			if (obj.GetType() != typeof(AppDomain))
				Trace.WriteLine(string.Format("OccuRec: Marshaled instance of {0} ({1} HashCode:{2})", or.TypeInfo != null ? or.TypeInfo.TypeName : obj.GetType().ToString(), or.URI != null ? or.URI.ToString() : "N/A", obj.GetHashCode().ToString()));
			else
			{
				// Not interested in AppDomain marshalling
			}
		}

		// Notifies a handler that an object has been unmarshaled.
		public void UnmarshaledObject(Object obj, ObjRef or)
		{
			if (obj.GetType() != typeof(AppDomain))
				Trace.WriteLine(string.Format("OccuRec: Unmarshaled instance of {0} ({1} HashCode:{2})", or.TypeInfo != null ? or.TypeInfo.TypeName : obj.GetType().ToString(), or.URI != null ? or.URI.ToString() : "N/A", obj.GetHashCode().ToString()));
			else
			{
				// Not interested in AppDomain marshalling
			}
		}

		// Notifies a handler that an object has been disconnected.
		public void DisconnectedObject(Object obj)
		{
			if (obj.GetType() != typeof(AppDomain))
				Trace.WriteLine(string.Format("OccuRec: Disconnected instance of {0} (HashCode:{1})", obj.GetType().ToString(), obj.GetHashCode().ToString()));
			else
			{
				// Not interested in AppDomain marshalling
			}
		}

	}
}

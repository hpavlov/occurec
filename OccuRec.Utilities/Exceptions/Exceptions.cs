/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace OccuRec.Utilities.Exceptions
{
	[Serializable]
    public class DriverException : Exception
    {
        public DriverException(string message)
            : base(message)
        { }

        public DriverException(string message, Exception innerException)
            : base(message, innerException)
        { }

		protected DriverException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (context.State == StreamingContextStates.CrossAppDomain)
				info.SetType(typeof(DriverException));
			base.GetObjectData(info, context);
		}
    }

	[Serializable]
    public class PropertyNotImplementedException : Exception
    {
        public PropertyNotImplementedException(string message)
            : base(message)
        { }

		protected PropertyNotImplementedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (context.State == StreamingContextStates.CrossAppDomain)
				info.SetType(typeof(PropertyNotImplementedException));
			base.GetObjectData(info, context);
		}
    }

	[Serializable]
    public class MethodNotImplementedException : Exception
    {
        public MethodNotImplementedException(string message)
            : base(message)
        { }

		protected MethodNotImplementedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (context.State == StreamingContextStates.CrossAppDomain)
				info.SetType(typeof(MethodNotImplementedException));
			base.GetObjectData(info, context);
		}
    }

	[Serializable]
    public class NotConnectedException : Exception
    {
		public NotConnectedException()
		{ }

		protected NotConnectedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (context.State == StreamingContextStates.CrossAppDomain)
				info.SetType(typeof(NotConnectedException));
			base.GetObjectData(info, context);
		}
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.Drivers
{
    public enum SensorType
    {
        Monochrome,
        Color,
        RGGB
    }

    public class DriverException : Exception
    {
        public DriverException(string message)
            : base(message)
        { }

        public DriverException(string message, Exception innerException)
            : base(message, innerException)
        { }   
    }


    public class PropertyNotImplementedException : Exception
    {
        public PropertyNotImplementedException(string message)
            : base(message)
        { }
    }


    public class MethodNotImplementedException : Exception
    {
        public MethodNotImplementedException(string message)
            : base(message)
        { }
    }

    public class NotConnectedException : Exception
    {
        
    }

}

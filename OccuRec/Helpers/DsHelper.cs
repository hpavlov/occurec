using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DirectShowLib;
using OccuRec.Properties;

namespace OccuRec.Helpers
{
	public class DsHelper
	{
        private static IPin PrintInfoAndReturnPin(IBaseFilter filter, IPin pin, PinDirection direction, Guid mediaType, Guid pinCategory, string debugInfo)
        {
			if (Settings.Default.VideoGraphDebugMode)
			{
				FilterInfo fInfo;
				int hr = filter.QueryFilterInfo(out fInfo);
				DsError.ThrowExceptionForHR(hr);

				string vendorInfo = null;
				hr = filter.QueryVendorInfo(out vendorInfo);
				if ((uint)hr != 0x80004001) /* Not Implemented*/
					DsError.ThrowExceptionForHR(hr);

				string mediaTypeStr = "N/A";
				if (mediaType == MediaType.AnalogVideo)
					mediaTypeStr = "AnalogVideo";
				else if (mediaType == MediaType.Video)
					mediaTypeStr = "Video";
				else if (mediaType == MediaType.Null)
					mediaTypeStr = "Null";

				string categoryStr = "N/A";
				if (pinCategory == PinCategory.Capture)
					categoryStr = "Capture";
				else if (pinCategory == PinCategory.Preview)
					categoryStr = "Preview";
				else if (pinCategory == PinCategory.AnalogVideoIn)
					categoryStr = "AnalogVideoIn";
				else if (pinCategory == PinCategory.CC)
					categoryStr = "CC";

				PinInfo pinInfo;
				hr = pin.QueryPinInfo(out pinInfo);
				DsError.ThrowExceptionForHR(hr);

				Trace.WriteLine(string.Format("Using {0} pin '{1}' of media type '{2}' and category '{3}' {4} ({5}::{6})",
					direction == PinDirection.Input ? "input" : "output",
					pinInfo.name,
					mediaTypeStr,
					categoryStr,
					debugInfo,
					vendorInfo, fInfo.achName));
			}

            return pin;
        }

	    public static IPin FindPin(IBaseFilter filter, PinDirection direction, Guid mediaType, Guid pinCategory, string preferredName)
        {
            if (Guid.Empty != pinCategory)
            {
                int idx = 0;

                do
                {
                    IPin pinByCategory = DsFindPin.ByCategory(filter, pinCategory, idx);

                    if (pinByCategory != null)
                    {
                        if (IsMatchingPin(pinByCategory, direction, mediaType))
                            return PrintInfoAndReturnPin(filter, pinByCategory, direction, mediaType, pinCategory, "found by category");

                        Marshal.ReleaseComObject(pinByCategory);
                    }
                    else
                        break;

                    idx++;
                }
                while (true);
            }

            if (!string.IsNullOrEmpty(preferredName))
            {
                IPin pinByName = DsFindPin.ByName(filter, preferredName);
                if (pinByName != null && IsMatchingPin(pinByName, direction, mediaType))
                    return PrintInfoAndReturnPin(filter, pinByName, direction, mediaType, pinCategory, "found by name");

                Marshal.ReleaseComObject(pinByName);
            }

            IEnumPins pinsEnum;
            IPin[] pins = new IPin[1];

            int hr = filter.EnumPins(out pinsEnum);
            DsError.ThrowExceptionForHR(hr);

            while (pinsEnum.Next(1, pins, IntPtr.Zero) == 0)
            {
                IPin pin = pins[0];
                if (pin != null)
                {
                    if (IsMatchingPin(pin, direction, mediaType))
                        return PrintInfoAndReturnPin(filter, pin, direction, mediaType, pinCategory, "found by direction and media type");

                    Marshal.ReleaseComObject(pin);
                }
            }

            return null;
        }

        private static bool IsMatchingPin(IPin pin, PinDirection direction, Guid mediaType)
        {
            PinDirection pinDirection;
            int hr = pin.QueryDirection(out pinDirection);
            DsError.ThrowExceptionForHR(hr);

            if (pinDirection != direction)
                // The pin lacks direction
                return false;

            IPin connectedPin;
            hr = pin.ConnectedTo(out connectedPin);
            if ((uint)hr != 0x80040209 /* Pin is not connected */)
                DsError.ThrowExceptionForHR(hr);

            if (connectedPin != null)
            {
                // The pin is already connected
                Marshal.ReleaseComObject(connectedPin);
                return false;
            }

            IEnumMediaTypes mediaTypesEnum;
            hr = pin.EnumMediaTypes(out mediaTypesEnum);
            DsError.ThrowExceptionForHR(hr);

            AMMediaType[] mediaTypes = new AMMediaType[1];

            while (mediaTypesEnum.Next(1, mediaTypes, IntPtr.Zero) == 0)
            {
                Guid majorType = mediaTypes[0].majorType;
                DsUtils.FreeAMMediaType(mediaTypes[0]);

                if (majorType == mediaType)
                {
                    // We have found the pin we were looking for
                    return true;
                }
            }

            return false;
        }

	}
}

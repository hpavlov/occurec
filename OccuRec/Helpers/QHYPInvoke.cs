using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace OccuRec.Helpers
{
    public class QHYCCDResult
    {
        public static int QHYCCD_READ_DIRECTLY = 0x2001;

        public static int QHYCCD_DELAY_200MS = 0x2000;
        /**
         * It means the camera using GiGaE transfer data */
        public static int QHYCCD_QGIGAE = 7;

        /**
         * It means the camera using usb sync transfer data */
        public static int QHYCCD_USBSYNC = 6;

        /**
         * It means the camera using usb async transfer data */
        public static int QHYCCD_USBASYNC = 5;

        /**
         * It means the camera is color one */
        public static int QHYCCD_COLOR = 4;

        /**
         * It means the camera is mono one*/
        public static int QHYCCD_MONO = 3;

        /**
         * It means the camera has cool function */
        public static int QHYCCD_COOL = 2;

        /**
         * It means the camera do not have cool function */
        public static int QHYCCD_NOTCOOL = 1;

        /**
         * camera works well */
        public static int QHYCCD_SUCCESS = 0;

        /**
         * Other error */
        public static int QHYCCD_ERROR = -1;

        /**
         * There is no camera connected */
        public static int QHYCCD_ERROR_NO_DEVICE = -2;

        /**
         * Do not support the function */
        public static int QHYCCD_ERROR_NOT_SUPPORTED = -3;

        /**
         * Set camera params error */
        public static int QHYCCD_ERROR_SETPARAMS = -4;

        /**
         * Get camera params error */
        public static int QHYCCD_ERROR_GETPARAMS = -5;
        /**
         * The camera is exposing now */
        public static int QHYCCD_ERROR_EXPOSING = -6;

        /**
         * The camera expose failed */
        public static int QHYCCD_ERROR_EXPFAILED = -7;

        /**
         * There is another instance is getting data from camera */
        public static int QHYCCD_ERROR_GETTINGDATA = -8;

        /**
         * Get data from camera failed */
        public static int QHYCCD_ERROR_GETTINGFAILED = -9;

        /**
         * Init camera failed */
        public static int QHYCCD_ERROR_INITCAMERA = -10;

        /**
         * Release SDK resouce failed */
        public static int QHYCCD_ERROR_RELEASERESOURCE = -11;

        /**
         * Init SDK resouce failed */
        public static int QHYCCD_ERROR_INITRESOURCE = -12;

        /**
         * There is no match camera */
        public static int QHYCCD_ERROR_NO_CAMERA_MATCH = -13;

        /**
         * Open cam failed */
        public static int QHYCCD_ERROR_OPENCAM = -14;

        /**
         * Init cam class failed */
        public static int QHYCCD_ERROR_INITCLASS = -15;

        /**
         * Set Resolution failed */
        public static int QHYCCD_ERROR_RESOLUTION_FAILED = -16;

        /**
         * Set usbtraffic failed */
        public static int QHYCCD_ERROR_USB_TRAFFIC = -17;

        /**
         * Set usb speed failed */
        public static int QHYCCD_ERROR_USB_SPEED = -18;

        /**
         * Set expose time failed */
        public static int QHYCCD_ERROR_SETEXPOSE = -19;

        /**
         * Set cam gain failed */
        public static int QHYCCD_ERROR_SETGAIN = -20;

        /**
         * Set cam white balance red failed */
        public static int QHYCCD_ERROR_SETRED = -21;

        /**
         * Set cam white balance blue failed */
        public static int QHYCCD_ERROR_SETBLUE = -22;

        /**
         * Set cam white balance blue failed */
        public static int QHYCCD_ERROR_EVTCMOS = -23;

        /**
         * Set cam white balance blue failed */
        public static int QHYCCD_ERROR_EVTUSB = -24;

        /**
         * Set cam white balance blue failed */
        public static int QHYCCD_ERROR_WHITE_BALANCE_FAILED = -25;
    }

    public enum StreamMode : byte
    {
        Default = 0,
        SingleFrame = 0,
        Live = 1
    }

    public enum CONTROL_ID : int
    {
        CONTROL_BRIGHTNESS = 0, //!< image brightness
        CONTROL_CONTRAST,       //!< image contrast 
        CONTROL_WBR,            //!< red of white balance 
        CONTROL_WBB,            //!< blue of white balance
        CONTROL_WBG,            //!< the green of white balance 
        CONTROL_GAMMA,          //!< screen gamma 
        CONTROL_GAIN,           //!< camera gain 
        CONTROL_OFFSET,         //!< camera offset 
        CONTROL_EXPOSURE,       //!< expose time (us)
        CONTROL_SPEED,          //!< transfer speed 
        CONTROL_TRANSFERBIT,    //!< image depth bits 
        CONTROL_CHANNELS,       //!< image channels 
        CONTROL_USBTRAFFIC,     //!< hblank 
        CONTROL_ROWNOISERE,     //!< row denoise 
        CONTROL_CURTEMP,        //!< current cmos or ccd temprature 
        CONTROL_CURPWM,         //!< current cool pwm 
        CONTROL_MANULPWM,       //!< set the cool pwm 
        CONTROL_CFWPORT,        //!< control camera color filter wheel port 
        CONTROL_COOLER,         //!< check if camera has cooler
        CONTROL_ST4PORT,        //!< check if camera has st4port
        CAM_COLOR,
        CAM_BIN1X1MODE,         //!< check if camera has bin1x1 mode 
        CAM_BIN2X2MODE,         //!< check if camera has bin2x2 mode 
        CAM_BIN3X3MODE,         //!< check if camera has bin3x3 mode 
        CAM_BIN4X4MODE,         //!< check if camera has bin4x4 mode 
        CAM_MECHANICALSHUTTER,                   //!< mechanical shutter  
        CAM_TRIGER_INTERFACE,                    //!< triger  
        CAM_TECOVERPROTECT_INTERFACE,            //!< tec overprotect
        CAM_SINGNALCLAMP_INTERFACE,              //!< singnal clamp 
        CAM_FINETONE_INTERFACE,                  //!< fine tone 
        CAM_SHUTTERMOTORHEATING_INTERFACE,       //!< shutter motor heating 
        CAM_CALIBRATEFPN_INTERFACE,              //!< calibrated frame 
        CAM_CHIPTEMPERATURESENSOR_INTERFACE,     //!< chip temperaure sensor
        CAM_USBREADOUTSLOWEST_INTERFACE,         //!< usb readout slowest 

        CAM_8BITS,                               //!< 8bit depth 
        CAM_16BITS,                              //!< 16bit depth
        CAM_GPS,                                 //!< check if camera has gps 

        CAM_IGNOREOVERSCAN_INTERFACE,            //!< ignore overscan area 

        QHYCCD_3A_AUTOBALANCE,
        QHYCCD_3A_AUTOEXPOSURE,
        QHYCCD_3A_AUTOFOCUS,
        CONTROL_AMPV,                            //!< ccd or cmos ampv
        CONTROL_VCAM                             //!< Virtual Camera on off 
    };

    public enum BAYER_ID
    {
        BAYER_GB = 1,
        BAYER_GR,
        BAYER_BG,
        BAYER_RG
    };

    public enum CodecID
    {
        NONE_CODEC,
        H261_CODEC
    };

    public class QHYPInvoke
    {
        private const string LIBRARY_QHYCCD = "qhyccd.dll";

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "InitQHYCCDResource")]
        public static extern int InitQHYCCDResource();

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "ReleaseQHYCCDResource")]
        public static extern int ReleaseQHYCCDResource();

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "OpenQHYCCD")]
        public static extern IntPtr OpenQHYCCD(string cameraId);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "ScanQHYCCD")]
        public static extern int ScanQHYCCD();

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetQHYCCDId")]
        public static extern int GetQHYCCDId(int index, [MarshalAs(UnmanagedType.LPArray)]byte[] cameraId);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetQHYCCDFWVersion")]
        public static extern int GetQHYCCDFWVersion(IntPtr handle, [MarshalAs(UnmanagedType.LPArray)]byte[] versionInfo);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "CloseQHYCCD")]
        public static extern int CloseQHYCCD(IntPtr handle);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetQHYCCDType")]
        public static extern int GetQHYCCDType(IntPtr handle);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetQHYCCDChipInfo")]
        public static extern int GetQHYCCDChipInfo(IntPtr handle, ref double chipWidth, ref double chipHeight, ref int width, ref int height, ref double pixelWidth, ref double pixelHeight, ref int bpp);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetQHYCCDBinMode")]
        public static extern int SetQHYCCDBinMode(IntPtr handle, int binx, int biny);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetQHYCCDBitsMode")]
        public static extern int SetQHYCCDBitsMode(IntPtr handle, int bits);

        // NOTE: This needs to be called every second to keep the temperature
        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "ControlQHYCCDTemp")]
        public static extern int ControlQHYCCDTemp(IntPtr handle, double targetTemp);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "ControlQHYCCDGuide")]
        public static extern int ControlQHYCCDGuide(IntPtr handle, int direction, short duration);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "SendOrder2QHYCCDCFW")]
        public static extern int SendOrder2QHYCCDCFW(IntPtr handle, string command, int length);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetQHYCCDCFWStatus")]
        public static extern int GetQHYCCDCFWStatus(IntPtr handle, string status);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetQHYCCDStreamMode")]
        public static extern int SetQHYCCDStreamMode(IntPtr handle, StreamMode mode);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetQHYCCDTrigerMode")]
        public static extern int SetQHYCCDTrigerMode(IntPtr handle, int triggerMode);


        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "InitQHYCCD")]
        public static extern int InitQHYCCD(IntPtr handle);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "IsQHYCCDControlAvailable")]
        private static extern int IsQHYCCDControlAvailableExternal(IntPtr handle, CONTROL_ID control);

        public static bool IsQHYCCDControlAvailable(IntPtr handle, CONTROL_ID control)
        {
            int result = IsQHYCCDControlAvailableExternal(handle, control);

            if (result == QHYCCDResult.QHYCCD_SUCCESS)
                return true;
            else if (result == QHYCCDResult.QHYCCD_ERROR_NOT_SUPPORTED)
                return false;
            else if (result == QHYCCDResult.QHYCCD_ERROR)
                return false;

            throw new QHYCCDException(result);
        }

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetQHYCCDParam")]
        public static extern double GetQHYCCDParam(IntPtr handle, CONTROL_ID control);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetQHYCCDParamMinMaxStep")]
        public static extern int GetQHYCCDParamMinMaxStep(IntPtr handle, CONTROL_ID control, ref double min, ref double max, ref double step);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetQHYCCDParam")]
        public static extern int SetQHYCCDParam(IntPtr handle, CONTROL_ID control, double value);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetQHYCCDResolution")]
        public static extern int SetQHYCCDResolution(IntPtr handle, int left, int top, int width, int height);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetQHYCCDMemLength")]
        public static extern int GetQHYCCDMemLength(IntPtr handle);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "ExpQHYCCDSingleFrame")]
        private static extern int ExpQHYCCDSingleFrameExternal(IntPtr handle);

        public static bool ExpQHYCCDSingleFrame(IntPtr handle)
        {
            int result = ExpQHYCCDSingleFrameExternal(handle);

            if (result == QHYCCDResult.QHYCCD_SUCCESS || result == QHYCCDResult.QHYCCD_READ_DIRECTLY)
                return true;
            else if (result == QHYCCDResult.QHYCCD_DELAY_200MS)
            {
                Thread.Sleep(200);
                return true;
            }
            else if (result == QHYCCDResult.QHYCCD_ERROR_EXPOSING)
                return false;
            else if (result == QHYCCDResult.QHYCCD_ERROR_EXPFAILED)
                return false;

            throw new QHYCCDException(result);
        }

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "CancelQHYCCDExposing")]
        public static extern int CancelQHYCCDExposing(IntPtr handle);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetQHYCCDExposureRemaining")]
        public static extern int GetQHYCCDExposureRemaining(IntPtr handle);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetQHYCCDSingleFrame")]
        public static extern int GetQHYCCDSingleFrame(IntPtr handle, ref int width, ref int height, ref int bpp, ref int channels, [MarshalAs(UnmanagedType.LPArray)]byte[] pixelBytes);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "CancelQHYCCDExposingAndReadout")]
        public static extern int CancelQHYCCDExposingAndReadout(IntPtr handle);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "BeginQHYCCDLive")]
        public static extern int BeginQHYCCDLive(IntPtr handle);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetQHYCCDLiveFrame")]
        public static extern int GetQHYCCDLiveFrame(IntPtr handle, ref int width, ref int height, ref int bpp, ref int channels, [MarshalAs(UnmanagedType.LPArray)]byte[] pixelBytes);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "StopQHYCCDLive")]
        public static extern int StopQHYCCDLive(IntPtr handle);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetQHYCCDCameraStatus")]
        public static extern int GetQHYCCDCameraStatus(IntPtr handle, [MarshalAs(UnmanagedType.LPArray)]byte[] status);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetQHYCCDShutterStatus")]
        public static extern int GetQHYCCDShutterStatus(IntPtr handle);


        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetQHYCCDGPSMasterSlave")]
        public static extern int SetQHYCCDGPSMasterSlave(IntPtr handle, byte mode);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetQHYCCDGPSVCOXFreq")]
        public static extern int SetQHYCCDGPSVCOXFreq(IntPtr handle, ushort voltage);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetQHYCCDGPSLedCalMode")]
        public static extern int SetQHYCCDGPSLedCalMode(IntPtr handle, byte mode);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetQHYCCDGPSLedCal")]
        public static extern int SetQHYCCDGPSLedCal(IntPtr handle, int pos, byte width);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetQHYCCDGPSPOSA")]
        public static extern int SetQHYCCDGPSPOSA(IntPtr handle, byte isSlave, int pos, byte width);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetQHYCCDGPSPOSB")]
        public static extern int SetQHYCCDGPSPOSB(IntPtr handle, byte isSlave, int pos, byte width);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetQHYCCDGPSSlaveModeParameter")]
        public static extern int SetQHYCCDGPSSlaveModeParameter(IntPtr handle, int targetSec, int targetUs, int deltaTSec, int deltaTUs, int expTime);

        [DllImport(LIBRARY_QHYCCD, CallingConvention = CallingConvention.StdCall, EntryPoint = "QHYCCDVendRequestWrite")]
        public static extern int QHYCCDVendRequestWrite(IntPtr handle, byte param, ushort value, ushort index, uint length, [MarshalAs(UnmanagedType.LPArray)]byte[] data);


        public static void CHECK(int result)
        {
            if (result < 0)
                throw new QHYCCDException(result);
        }

        public static bool SUCCEEDED(int result)
        {
            return result == 0;
        }
    }

    public class QHYCCDException : Exception
    {
        public QHYCCDException()
            : base()
        { }

        public QHYCCDException(string message)
            : base(message)
        { }

        public QHYCCDException(int qhyccdCode)
            : base(TranslateErrorCode(qhyccdCode))
        { }

        public QHYCCDException(string message, Exception innerException)
            : base(message, innerException)
        { }

        private static string TranslateErrorCode(int qhyccdCode)
        {
            string translatedMessage = "";

            if (qhyccdCode == QHYCCDResult.QHYCCD_SUCCESS)
                translatedMessage = "The operation completed successfuly.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_QGIGAE)
                translatedMessage = "The camera is using GiGaE.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_USBSYNC)
                translatedMessage = "The camera is using USB sync.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_USBASYNC)
                translatedMessage = "The camera is using USB async.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_COLOR)
                translatedMessage = "This is a colour camera.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_MONO)
                translatedMessage = "This is a mono camera.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_COOL)
                translatedMessage = "The camera supports cooling.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_NOTCOOL)
                translatedMessage = "The camera doesn't support cooling.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR)
                translatedMessage = "Unspecified error has occured.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_NO_DEVICE)
                translatedMessage = "No camera is connected.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_NOT_SUPPORTED)
                translatedMessage = "This operation is not supported.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_SETPARAMS)
                translatedMessage = "Error setting camera parameter.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_GETPARAMS)
                translatedMessage = "Error getting camera parameter.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_EXPOSING)
                translatedMessage = "The camera is exposing right now.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_EXPFAILED)
                translatedMessage = "The camera exposure has failed.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_GETTINGDATA)
                translatedMessage = "Another operation is retreiving data from the camera.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_GETTINGFAILED)
                translatedMessage = "Failed to retreiving data from the camera.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_INITCAMERA)
                translatedMessage = "Failed to initialize the camera.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_RELEASERESOURCE)
                translatedMessage = "Error releasing resource.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_INITRESOURCE)
                translatedMessage = "Error initializing resource.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_NO_CAMERA_MATCH)
                translatedMessage = "No matching camera has been identified.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_OPENCAM)
                translatedMessage = "Failed to open the camera.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_INITCLASS)
                translatedMessage = "Failed to initialize camera class.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_RESOLUTION_FAILED)
                translatedMessage = "Failed setting camera resolution.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_USB_TRAFFIC)
                translatedMessage = "Failed setting USB traffic.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_USB_SPEED)
                translatedMessage = "Failed setting USB speed.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_SETEXPOSE)
                translatedMessage = "Error setting exposure time.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_SETGAIN)
                translatedMessage = "Error setting camera gain.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_SETRED)
                translatedMessage = "Error setting white balance for 'red'.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_SETBLUE)
                translatedMessage = "Error setting white balance for 'blue'.";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_EVTCMOS)
                translatedMessage = "QHYCCD_ERROR_EVTCMOS";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_EVTUSB)
                translatedMessage = "QHYCCD_ERROR_EVTUSB";
            else if (qhyccdCode == QHYCCDResult.QHYCCD_ERROR_WHITE_BALANCE_FAILED)
                translatedMessage = "Error setting white balance.";

            return string.Format("Error code: {0}. {1}", qhyccdCode, translatedMessage).TrimEnd();
        }
    }

    public class QHYCCDNotConnectedException : Exception
    {
        public QHYCCDNotConnectedException()
            : base()
        {
        }
    }

    public static class DeviceIds
    {
        /* IMG series */

        /**
         * Type define for IMG0S */
        public const int DEVICETYPE_IMG0S = 1000;

        /**
         * Type define for IMG0H */
        public const int DEVICETYPE_IMG0H = 1001;

        /**
         * Type define for IMG0L */
        public const int DEVICETYPE_IMG0L = 1002;

        /**
         * Type define for IMG0X */
        public const int DEVICETYPE_IMG0X = 1003;

        /**
         * Type define for IMG1S */
        public const int DEVICETYPE_IMG1S = 1004;

        /**
         * Type define for IMG2S */
        public const int DEVICETYPE_IMG2S = 1005;

        /**
         * Type define for IMG1E */
        public const int DEVICETYPE_IMG1E = 1006;


        /* QHY5 seires */

        /**
         * Type define for QHY5 */
        public const int DEVICETYPE_QHY5 = 2001;


        /* QHY5II series */

        /**
         * Type define for QHY5II */
        public const int DEVICETYPE_QHY5II = 3001;

        /**
         * Type define for QHY5LII_M */
        public const int DEVICETYPE_QHY5LII_M = 3002;

        /**
         * Type define for QHY5LII_C */
        public const int DEVICETYPE_QHY5LII_C = 3003;

        /**
         * Type define for QHY5TII */
        public const int DEVICETYPE_QHY5TII = 3004;

        /**
         * Type define for QHY5RII */
        public const int DEVICETYPE_QHY5RII = 3005;

        /**
         * Type define for QHY5PII */
        public const int DEVICETYPE_QHY5PII = 3006;

        /**
         * Type define for QHY5VII */
        public const int DEVICETYPE_QHY5VII = 3007;

        /**
         * Type define for QHY5HII */
        public const int DEVICETYPE_QHY5HII = 3008;

        /**
         * Type define for QHYXXX */
        public const int DEVICETYPE_MINICAM5S_M = 3009;

        /**
         * Type define for QHYXXX */
        public const int DEVICETYPE_MINICAM5S_C = 3010;

        /**
         * Type define for QHY5PII_C */
        public const int DEVICETYPE_QHY5PII_C = 3011;

        /**
         * Type define for QHY5RII-M */
        public const int DEVICETYPE_QHY5RII_M = 3012;

        /**
         * Type define for QHY5RII-M */
        public const int DEVICETYPE_MINICAM5F_M = 3013;

        /**
         * Type define for QHY5PII_M */
        public const int DEVICETYPE_QHY5PII_M = 3014;

        /**
         * Type define for QHY5TII */
        public const int DEVICETYPE_QHY5TII_C = 3015;

        /**
         * Type define for POLEMASTER */
        public const int DEVICETYPE_POLEMASTER = 3016;

        /**
         * Type define for QHY5IIEND */
        public const int DEVICETYPE_QHY5IIEND = 3999;


        /* QHY5III seires */

        /**
         * Type define for QHY5III174*/
        public const int DEVICETYPE_QHY5III174 = 4000;

        /**
         * Type define for QHY5III174 */
        public const int DEVICETYPE_QHY5III174M = 4001;

        /**
         * Type define for QHY5III174C*/
        public const int DEVICETYPE_QHY5III174C = 4002;

        /**
         * Type define for QHY174*/
        public const int DEVICETYPE_QHY174 = 4003;

        /**
         * Type define for QHY174M*/
        public const int DEVICETYPE_QHY174M = 4004;

        /**
         * Type define for QHY174C*/
        public const int DEVICETYPE_QHY174C = 4005;

        /**
         * Type define for QHY5III178*/
        public const int DEVICETYPE_QHY5III178 = 4006;

        /**
         * Type define for QHY5III178C*/
        public const int DEVICETYPE_QHY5III178C = 4007;

        /**
         * Type define for QHY5III178M*/
        public const int DEVICETYPE_QHY5III178M = 4008;

        /**
         * Type define for QHY178*/
        public const int DEVICETYPE_QHY178 = 4009;

        /**
         * Type define for QHY178M*/
        public const int DEVICETYPE_QHY178M = 4010;

        /**
         * Type define for QHY178C*/
        public const int DEVICETYPE_QHY178C = 4011;

        /**
         * Type define for QHY5III185*/
        public const int DEVICETYPE_QHY5III185 = 4012;

        /**
         * Type define for QHY5III185C*/
        public const int DEVICETYPE_QHY5III185C = 4013;

        /**
         * Type define for QHY5III185M*/
        public const int DEVICETYPE_QHY5III185M = 4014;

        /**
         * Type define for QHY185*/
        public const int DEVICETYPE_QHY185 = 4015;

        /**
         * Type define for QHY185M*/
        public const int DEVICETYPE_QHY185M = 4016;

        /**
         * Type define for QHY185C*/
        public const int DEVICETYPE_QHY185C = 4017;

        /**
         * Type define for QHY5III224*/
        public const int DEVICETYPE_QHY5III224 = 4018;

        /**
         * Type define for QHY5III224C*/
        public const int DEVICETYPE_QHY5III224C = 4019;

        /**
         * Type define for QHY5III224M*/
        public const int DEVICETYPE_QHY5III224M = 4020;

        /**
         * Type define for QHY224*/
        public const int DEVICETYPE_QHY224 = 4021;

        /**
         * Type define for QHY224M*/
        public const int DEVICETYPE_QHY224M = 4022;

        /**
         * Type define for QHY224C*/
        public const int DEVICETYPE_QHY224C = 4023;

        /**
         * Type define for QHY5III290*/
        public const int DEVICETYPE_QHY5III290 = 4024;

        /**
         * Type define for QHY5III290C*/
        public const int DEVICETYPE_QHY5III290C = 4025;

        /**
         * Type define for QHY5III290M*/
        public const int DEVICETYPE_QHY5III290M = 4026;

        /**
         * Type define for QHY290*/
        public const int DEVICETYPE_QHY290 = 4027;

        /**
         * Type define for QHY290M*/
        public const int DEVICETYPE_QHY290M = 4028;

        /**
         * Type define for QHY290C*/
        public const int DEVICETYPE_QHY290C = 4029;

        /**
         * Type define for QHY5III236*/
        public const int DEVICETYPE_QHY5III236 = 4030;

        /**
         * Type define for QHY5III236C*/
        public const int DEVICETYPE_QHY5III236C = 4031;

        /**
         * Type define for QHY5III290M*/
        public const int DEVICETYPE_QHY5III236M = 4032;

        /**
         * Type define for QHY236*/
        public const int DEVICETYPE_QHY236 = 4033;

        /**
         * Type define for QHY236M*/
        public const int DEVICETYPE_QHY236M = 4034;

        /**
         * Type define for QHY236C*/
        public const int DEVICETYPE_QHY236C = 4035;

        /**
         * Type define for GSENSE400*/
        public const int DEVICETYPE_QHY5IIIG400M = 4036;

        /**
         * Type define for QHY163*/
        public const int DEVICETYPE_QHY163 = 4037;

        /**
         * Type define for QHY163M*/
        public const int DEVICETYPE_QHY163M = 4038;

        /**
         * Type define for QHY163C*/
        public const int DEVICETYPE_QHY163C = 4039;

        /**
         * Type define for QHY165*/
        public const int DEVICETYPE_QHY165 = 4040;

        /**
         * Type define for QHY165C*/
        public const int DEVICETYPE_QHY165C = 4041;

        /**
         * Type define for QHY367*/
        public const int DEVICETYPE_QHY367 = 4042;

        /**
         * Type define for QHY367C*/
        public const int DEVICETYPE_QHY367C = 4043;

        /**
         * Type define for QHY183*/
        public const int DEVICETYPE_QHY183 = 4044;

        /**
         * Type define for QHY183C*/
        public const int DEVICETYPE_QHY183C = 4045;

        /**
         * Type define for QHY5IIIEND*/
        public const int DEVICETYPE_QHY5IIIEND = 4999;

        /* QHYA seires */

        /**
         * Type define for QHY90A/IC90A */
        public const int DEVICETYPE_90A = 900;

        /**
         * Type define for QHY16200A/IC16200A */
        public const int DEVICETYPE_16200A = 901;

        /**
         * Type define for 695A*/
        public const int DEVICETYPE_695A = 916;

        /**
         * Type define for QHY9T */
        public const int DEVICETYPE_16803 = 906;


        /**
         * Type define for QHY16 */
        public const int DEVICETYPE_QHY16 = 16;

        /**
         * Type define for QHY6 */
        public const int DEVICETYPE_QHY6 = 60;

        /**
         * Type define for QHY7 */
        public const int DEVICETYPE_QHY7 = 70;

        /**
         * Type define for QHY2PRO */
        public const int DEVICETYPE_QHY2PRO = 221;

        /**
         * Type define for IMG2P */
        public const int DEVICETYPE_IMG2P = 220;

        /**
         * Type define for QHY8 */
        public const int DEVICETYPE_QHY8 = 400;

        /**
         * Type define for QHY8PRO */
        public const int DEVICETYPE_QHY8PRO = 453;

        /**
         * Type define for QHY16000 */
        public const int DEVICETYPE_QHY16000 = 361;

        /**
         * Type define for QHY12 */
        public const int DEVICETYPE_QHY12 = 613;

        /**
         * Type define for IC8300 */
        public const int DEVICETYPE_IC8300 = 890;

        /**
         * Type define for QHY9S */
        public const int DEVICETYPE_QHY9S = 892;

        /**
         * Type define for QHY10 */
        public const int DEVICETYPE_QHY10 = 893;

        /**
         * Type define for QHY8L */
        public const int DEVICETYPE_QHY8L = 891;

        /**
         * Type define for QHY11 */
        public const int DEVICETYPE_QHY11 = 894;

        /**
         * Type define for QHY21 */
        public const int DEVICETYPE_QHY21 = 895;

        /**
         * Type define for QHY22 */
        public const int DEVICETYPE_QHY22 = 896;

        /**
         * Type define for QHY23 */
        public const int DEVICETYPE_QHY23 = 897;

        /**
         * Type define for QHY15 */
        public const int DEVICETYPE_QHY15 = 898;

        /**
         * Type define for QHY27 */
        public const int DEVICETYPE_QHY27 = 899;


        /**
         * Type define for QHY28 */
        public const int DEVICETYPE_QHY28 = 902;

        /**
         * Type define for QHY9T */
        public const int DEVICETYPE_QHY9T = 905;

        /**
         * Type define for QHY29 */
        public const int DEVICETYPE_QHY29 = 907;

        /**
         * Type define for SOLAR1600 */
        public const int DEVICETYPE_SOLAR1600 = 908;

        /**
         * Type define for QHY15GIGAE */
        public const int DEVICETYPE_QHY15G = 9000;

        /**
         * Type define for SOLAR800G */
        public const int DEVICETYPE_SOLAR800G = 9001;

        public const int DEVICETYPE_A0340G = 9003;

        public const int DEVICETYPE_QHY08050G = 9004;

        public const int DEVICETYPE_QHY694G = 9005;

        public const int DEVICETYPE_QHY27G = 9006;

        public const int DEVICETYPE_QHY23G = 9007;

        public const int DEVICETYPE_QHY16000G = 9008;

        public const int DEVICETYPE_QHY160002AD = 9009;

        public const int DEVICETYPE_QHY814G = 9010;

        public const int DEVICETYPE_QHY45GX = 9011;


        public static string GetCameraModel(int deviceId)
        {
            switch (deviceId)
            {
                case DEVICETYPE_IMG0S:
                    return "IMG0S";

                case DEVICETYPE_IMG0H:
                    return "IMG0H";

                case DEVICETYPE_IMG0L:
                    return "IMG0L";

                case DEVICETYPE_IMG0X:
                    return "IMG0X";

                case DEVICETYPE_IMG1S:
                    return "IMG1S";

                case DEVICETYPE_IMG2S:
                    return "IMG2S";

                case DEVICETYPE_IMG1E:
                    return "IMG1E";

                case DEVICETYPE_QHY5:
                    return "QHY5";

                case DEVICETYPE_QHY5II:
                    return "QHY5II";

                case DEVICETYPE_QHY5LII_M:
                    return "QHY5LII_M";

                case DEVICETYPE_QHY5LII_C:
                    return "QHY5LII_C";

                case DEVICETYPE_QHY5TII:
                    return "QHY5TII";

                case DEVICETYPE_QHY5RII:
                    return "QHY5RII";

                case DEVICETYPE_QHY5PII:
                    return "QHY5PII";

                case DEVICETYPE_QHY5VII:
                    return "QHY5VII";

                case DEVICETYPE_QHY5HII:
                    return "QHY5HII";

                case DEVICETYPE_MINICAM5S_M:
                    return "QHYXXX";

                case DEVICETYPE_MINICAM5S_C:
                    return "QHYXXX";

                case DEVICETYPE_QHY5PII_C:
                    return "QHY5PII_C";

                case DEVICETYPE_QHY5RII_M:
                    return "QHY5RII-M";

                case DEVICETYPE_MINICAM5F_M:
                    return "QHY5RII-M";

                case DEVICETYPE_QHY5PII_M:
                    return "QHY5PII_M";

                case DEVICETYPE_QHY5TII_C:
                    return "QHY5TII";

                case DEVICETYPE_POLEMASTER:
                    return "POLEMASTER";

                case DEVICETYPE_QHY5IIEND:
                    return "QHY5IIEND";

                case DEVICETYPE_QHY5III174:
                    return "QHY5III174";

                case DEVICETYPE_QHY5III174M:
                    return "QHY5III174";

                case DEVICETYPE_QHY5III174C:
                    return "QHY5III174C";

                case DEVICETYPE_QHY174:
                    return "QHY174";

                case DEVICETYPE_QHY174M:
                    return "QHY174M";

                case DEVICETYPE_QHY174C:
                    return "QHY174C";

                case DEVICETYPE_QHY5III178:
                    return "QHY5III178";

                case DEVICETYPE_QHY5III178C:
                    return "QHY5III178C";

                case DEVICETYPE_QHY5III178M:
                    return "QHY5III178M";

                case DEVICETYPE_QHY178:
                    return "QHY178";

                case DEVICETYPE_QHY178M:
                    return "QHY178M";

                case DEVICETYPE_QHY178C:
                    return "QHY178C";

                case DEVICETYPE_QHY5III185:
                    return "QHY5III185";

                case DEVICETYPE_QHY5III185C:
                    return "QHY5III185C";

                case DEVICETYPE_QHY5III185M:
                    return "QHY5III185M";

                case DEVICETYPE_QHY185:
                    return "QHY185";

                case DEVICETYPE_QHY185M:
                    return "QHY185M";

                case DEVICETYPE_QHY185C:
                    return "QHY185C";

                case DEVICETYPE_QHY5III224:
                    return "QHY5III224";

                case DEVICETYPE_QHY5III224C:
                    return "QHY5III224C";

                case DEVICETYPE_QHY5III224M:
                    return "QHY5III224M";

                case DEVICETYPE_QHY224:
                    return "QHY224";

                case DEVICETYPE_QHY224M:
                    return "QHY224M";

                case DEVICETYPE_QHY224C:
                    return "QHY224C";

                case DEVICETYPE_QHY5III290:
                    return "QHY5III290";

                case DEVICETYPE_QHY5III290C:
                    return "QHY5III290C";

                case DEVICETYPE_QHY5III290M:
                    return "QHY5III290M";

                case DEVICETYPE_QHY290:
                    return "QHY290";

                case DEVICETYPE_QHY290M:
                    return "QHY290M";

                case DEVICETYPE_QHY290C:
                    return "QHY290C";

                case DEVICETYPE_QHY5III236:
                    return "QHY5III236";

                case DEVICETYPE_QHY5III236C:
                    return "QHY5III236C";

                case DEVICETYPE_QHY5III236M:
                    return "QHY5III290M";

                case DEVICETYPE_QHY236:
                    return "QHY236";

                case DEVICETYPE_QHY236M:
                    return "QHY236M";

                case DEVICETYPE_QHY236C:
                    return "QHY236C";

                case DEVICETYPE_QHY5IIIG400M:
                    return "GSENSE400";

                case DEVICETYPE_QHY163:
                    return "QHY163";

                case DEVICETYPE_QHY163M:
                    return "QHY163M";

                case DEVICETYPE_QHY163C:
                    return "QHY163C";

                case DEVICETYPE_QHY165:
                    return "QHY165";

                case DEVICETYPE_QHY165C:
                    return "QHY165C";

                case DEVICETYPE_QHY367:
                    return "QHY367";

                case DEVICETYPE_QHY367C:
                    return "QHY367C";

                case DEVICETYPE_QHY183:
                    return "QHY183";

                case DEVICETYPE_QHY183C:
                    return "QHY183C";

                case DEVICETYPE_QHY5IIIEND:
                    return "QHY5IIIEND";

                case DEVICETYPE_90A:
                    return "QHY90A/IC90A";

                case DEVICETYPE_16200A:
                    return "QHY16200A/IC16200A";

                case DEVICETYPE_695A:
                    return "695A";

                case DEVICETYPE_16803:
                    return "QHY9T";

                case DEVICETYPE_QHY16:
                    return "QHY16";

                case DEVICETYPE_QHY6:
                    return "QHY6";

                case DEVICETYPE_QHY7:
                    return "QHY7";

                case DEVICETYPE_QHY2PRO:
                    return "QHY2PRO";

                case DEVICETYPE_IMG2P:
                    return "IMG2P";

                case DEVICETYPE_QHY8:
                    return "QHY8";

                case DEVICETYPE_QHY8PRO:
                    return "QHY8PRO";

                case DEVICETYPE_QHY16000:
                    return "QHY16000";

                case DEVICETYPE_QHY12:
                    return "QHY12";

                case DEVICETYPE_IC8300:
                    return "IC8300";

                case DEVICETYPE_QHY9S:
                    return "QHY9S";

                case DEVICETYPE_QHY10:
                    return "QHY10";

                case DEVICETYPE_QHY8L:
                    return "QHY8L";

                case DEVICETYPE_QHY11:
                    return "QHY11";

                case DEVICETYPE_QHY21:
                    return "QHY21";

                case DEVICETYPE_QHY22:
                    return "QHY22";

                case DEVICETYPE_QHY23:
                    return "QHY23";

                case DEVICETYPE_QHY15:
                    return "QHY15";

                case DEVICETYPE_QHY27:
                    return "QHY27";

                case DEVICETYPE_QHY28:
                    return "QHY28";

                case DEVICETYPE_QHY9T:
                    return "QHY9T";

                case DEVICETYPE_QHY29:
                    return "QHY29";

                case DEVICETYPE_SOLAR1600:
                    return "SOLAR1600";

                case DEVICETYPE_QHY15G:
                    return "QHY15GIGAE";

                case DEVICETYPE_SOLAR800G:
                    return "SOLAR800G";

                case DEVICETYPE_A0340G:
                    return "A0340G";

                case DEVICETYPE_QHY08050G:
                    return "QHY08050G";

                case DEVICETYPE_QHY694G:
                    return "QHY694G";

                case DEVICETYPE_QHY27G:
                    return "QHY27G";

                case DEVICETYPE_QHY23G:
                    return "QHY23G";

                case DEVICETYPE_QHY16000G:
                    return "QHY16000G";

                case DEVICETYPE_QHY160002AD:
                    return "QHY160002AD";

                case DEVICETYPE_QHY814G:
                    return "QHY814G";

                case DEVICETYPE_QHY45GX:
                    return "QHY45GX";
            }

            return "Unknown";
        }
    }

    public enum QHYGPSStatus
    {
        PoweredUp = 0,
        NoTiming = 1,
        NoPPS = 2,
        PPS = 3
    }

    public class ImageHeader
    {
        public ImageHeader(int frameNo, byte[] imageHead, int bpp)
        {
            this.FrameNo = frameNo;
            int mult = bpp == 16 ? 2 : 1;
            int add = bpp == 16 ? 1 : 0;
            SeqNumber = 256 * 256 * 256 * imageHead[0 * mult + add] + 256 * 256 * imageHead[1 * mult + add] + 256 * imageHead[2 * mult + add] + imageHead[3 * mult + add];
            TempNumber = imageHead[4 * mult + add];
            Width = 256 * imageHead[5 * mult + add] + imageHead[6 * mult + add];
            Height = 256 * imageHead[7 * mult + add] + imageHead[8 * mult + add];
            Latitude = 256 * 256 * 256 * imageHead[9 * mult + add] + 256 * 256 * imageHead[10 * mult + add] + 256 * imageHead[11 * mult + add] + imageHead[12 * mult + add];
            Longitude = 256 * 256 * 256 * imageHead[13 * mult + add] + 256 * 256 * imageHead[14 * mult + add] + 256 * imageHead[15 * mult + add] + imageHead[16 * mult + add];

            StartFlag = imageHead[17 * mult + add];
            StartSec = 256 * 256 * 256 * imageHead[18 * mult + add] + 256 * 256 * imageHead[19 * mult + add] + 256 * imageHead[20 * mult + add] + imageHead[21 * mult + add];
            StartUs = 256 * 256 * imageHead[22 * mult + add] + 256 * imageHead[23 * mult + add] + imageHead[24 * mult + add];

            EndFlag = imageHead[25 * mult + add];
            EndSec = 256 * 256 * 256 * imageHead[26 * mult + add] + 256 * 256 * imageHead[27 * mult + add] + 256 * imageHead[28 * mult + add] + imageHead[29 * mult + add];
            EndUs = 256 * 256 * imageHead[30 * mult + add] + 256 * imageHead[31 * mult + add] + imageHead[32 * mult + add];

            NowFlag = imageHead[33 * mult + add];
            NowSec = 256 * 256 * 256 * imageHead[34 * mult + add] + 256 * 256 * imageHead[35 * mult + add] + 256 * imageHead[36 * mult + add] + imageHead[37 * mult + add];
            NowUs = 256 * 256 * imageHead[38 * mult + add] + 256 * imageHead[39 * mult + add] + imageHead[40 * mult + add];
            MaxClock = 256 * 256 * imageHead[41 * mult + add] + 256 * imageHead[42 * mult + add] + imageHead[43 * mult + add];

            // NOTE: Lo and Hi 4-bits of NowFlag are the GPS Status at Start and End exposure. We only use one of the flags here
            GPSStatus = (QHYGPSStatus)((NowFlag >> 4) & 0x0F);
        }

        public readonly QHYGPSStatus GPSStatus;
        public readonly int FrameNo;
        public readonly int SeqNumber;
        public readonly int TempNumber;
        public readonly int Width;
        public readonly int Height;
        public readonly int Latitude;
        public readonly int Longitude;

        public double ParseLatitude
        {
            get
            {
                int part = Latitude % 100000;
                int min = (Latitude / 100000) % 100;
                int deg = (Latitude / 10000000) % 100;
                int sign = (Latitude / 1000000000) == 1 ? -1 : 1;
                return sign * (deg + min / 60.0 + (part / 100000.0) / 60.0);
            }
        }

        public double ParseLongitude
        {
            get
            {
                int part = Longitude % 10000;
                int min = (Longitude / 10000) % 100;
                int deg = (Longitude / 1000000) % 1000;
                int sign = (Longitude / 1000000000) == 1 ? -1 : 1;
                return sign * (deg + min / 60.0 + (part / 10000.0) / 60.0);

            }
        }

        public readonly int StartFlag;
        public readonly int StartSec;
        public readonly int StartUs;
        public DateTime StartTime
        {
            get
            {
                return new DateTime(1900, 1, 1, 12, 0, 0).AddDays(34979.5 + StartSec/3600.0/24.0).AddSeconds(StartUs/10E6);
            }
        }

        public readonly int EndFlag;
        public readonly int EndSec;
        public readonly int EndUs;
        public DateTime EndTime
        {
            get
            {
                return new DateTime(1900, 1, 1, 12, 0, 0).AddDays(34979.5 + EndSec / 3600.0 / 24.0).AddSeconds(EndUs / 10E6);
            }
        }

        public readonly int NowFlag;
        public readonly int NowSec;
        public readonly int NowUs;
        public readonly int MaxClock;

        public bool GpsTimeAvailable
        {
            get { return MaxClock <  10000500; }
        }
    }

    public class VOXFreqFitter
    {
        private const uint INVALID_FREQ_MIN = 9999000;
        private const uint INVALID_FREQ_MAX = 10000500;
        private const uint TARGET_FREQ = 10000000;
        private const ushort TARGET_PRECISION = 2;
        private const ushort LARGE_MOVEMENT = 100;
        private const int WAIT_SECONDS_AFTER_REACH_TARGET = 120; /* 2 min */

        public ushort CURRENT_STEP = 0;

        private ushort m_StartingVoltage;

        private ushort m_PrevVoltage = 0;
        private uint m_PrevFreq = 0;

        private List<ushort> m_VoltageHistory = new List<ushort>();
        private List<uint> m_FreqHistory = new List<uint>();

        private DateTime? m_NextAdjustment;

        public VOXFreqFitter(ushort startingVoiltage)
        {
            m_StartingVoltage = startingVoiltage;
        }

        public ushort Initialize()
        {
            CURRENT_STEP = LARGE_MOVEMENT;

            m_VoltageHistory.Clear();
            m_FreqHistory.Clear();

            m_PrevVoltage = m_StartingVoltage;
            m_NextAdjustment = null;

            Trace.WriteLine(string.Format("QHYCDD VOX Fitter: Initialized at {0}", m_StartingVoltage));

            return m_StartingVoltage;
        }

        public ushort? GetNextValue(uint currentFreq)
        {
            if (currentFreq <= INVALID_FREQ_MIN || currentFreq >= INVALID_FREQ_MAX)
                return null;

            if (m_NextAdjustment.HasValue)
            {
                if (m_NextAdjustment.Value > DateTime.UtcNow)
                    return null;
                else
                {
                    m_NextAdjustment = null;
                    return m_PrevVoltage;
                }
            }

            if (m_FreqHistory.Count == 0)
            {
                m_VoltageHistory.Add(m_PrevVoltage);
                m_FreqHistory.Add(currentFreq);

                StepUp(currentFreq);
            }
            else
            {
                m_VoltageHistory.Add(m_PrevVoltage);
                m_FreqHistory.Add(currentFreq);

                if (Math.Abs(currentFreq - TARGET_FREQ) < TARGET_PRECISION)
                {
                    m_PrevFreq = currentFreq;
                    m_NextAdjustment = DateTime.UtcNow.AddSeconds(WAIT_SECONDS_AFTER_REACH_TARGET);
                    Trace.WriteLine(string.Format("QHYCDD VOX Fitter: {0} => {1}. Reached Target. Next adjustment at {2}", m_PrevVoltage, m_PrevFreq, m_NextAdjustment.Value.ToString("HH:mm:ss UTC")));
                    return null;
                }

                if (currentFreq > m_PrevFreq)
                {
                    if (currentFreq < TARGET_FREQ)
                        StepUp(currentFreq);
                    else
                    {
                        if (CURRENT_STEP > 2) CURRENT_STEP /= 2;
                        StepDown(currentFreq);
                    }
                }
                else
                {
                    if (currentFreq > TARGET_FREQ)
                        StepDown(currentFreq);
                    else
                    {
                        if (CURRENT_STEP > 2) CURRENT_STEP /= 2;
                        StepUp(currentFreq);
                    }
                }
            }

            return m_PrevVoltage;
        }

        private void StepUp(uint currentFreq)
        {
            RecordNextStep(currentFreq, (ushort)(m_PrevVoltage + CURRENT_STEP));

            Trace.WriteLine(string.Format("QHYCDD VOX Fitter: {0} => {1}. StepUp {2}", m_PrevVoltage, m_PrevFreq, CURRENT_STEP));
        }

        private void StepDown(uint currentFreq)
        {
            RecordNextStep(currentFreq, (ushort)(m_PrevVoltage - CURRENT_STEP));

            Trace.WriteLine(string.Format("QHYCDD VOX Fitter: {0} => {1}. StepDown {2}", m_PrevVoltage, m_PrevFreq, CURRENT_STEP));
        }

        private void RecordNextStep(uint currentFreq, ushort proposedVoltage)
        {
            m_PrevFreq = currentFreq;
            m_PrevVoltage = proposedVoltage;
        }
    }
}

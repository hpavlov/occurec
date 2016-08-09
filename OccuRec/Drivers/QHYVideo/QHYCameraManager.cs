using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.Helpers;

namespace OccuRec.Drivers.QHYVideo
{
    public class QHYCameraManager: IDisposable
    {
        public static QHYCameraManager Instance = new QHYCameraManager();

        private bool m_Initialized = false;

        public List<string> ListAvailableCameras()
        {
            var rv = new List<string>();

            QHYPInvoke.CHECK(QHYPInvoke.InitQHYCCDResource());
            m_Initialized = true;

            int numCameras = QHYPInvoke.ScanQHYCCD();

            for (int i = 0; i < numCameras; i++)
            {
                byte[] cameraId = new byte[256];
                int result = QHYPInvoke.GetQHYCCDId(i, cameraId);
                if (result == QHYCCDResult.QHYCCD_SUCCESS)
                {
                    string cameraIdStr = Encoding.ASCII.GetString(cameraId).TrimEnd('\0');
                    rv.Add(cameraIdStr);
                }
                else
                    throw new QHYCCDException(result);
            }

            return rv;
        }

        public void Dispose()
        {
            if (m_Initialized)
            {
                QHYPInvoke.ReleaseQHYCCDResource();
                GC.SuppressFinalize(this);

                m_Initialized = false;
            }
        }
    }
}

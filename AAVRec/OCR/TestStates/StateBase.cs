using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using AAVRec.Properties;

namespace AAVRec.OCR.TestStates
{
    public abstract class StateBase
    {
        internal abstract void Reset(StateContext context);
        internal abstract TestFrameResult TestTimeStamp(StateContext context, OsdFrameInfo frameTimestamp);
    }
}

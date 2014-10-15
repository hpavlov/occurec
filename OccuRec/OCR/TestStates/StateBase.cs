/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using OccuRec.Properties;

namespace OccuRec.OCR.TestStates
{
    public abstract class StateBase
    {
        internal abstract void Reset(StateContext context);
        internal abstract TestFrameResult TestTimeStamp(StateContext context, OsdFrameInfo frameTimestamp);
    }
}

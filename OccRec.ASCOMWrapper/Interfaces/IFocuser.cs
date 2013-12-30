﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccuRec.ASCOM.Wrapper.Interfaces
{
    public enum FocuserStepSize
    {
        Large,
        Small,
        Smallest
    }

    public interface IFocuser : IASCOMFocuser
    {
        void MoveIn(FocuserStepSize stepSize);
        void MoveOut(FocuserStepSize stepSize);
    }
}
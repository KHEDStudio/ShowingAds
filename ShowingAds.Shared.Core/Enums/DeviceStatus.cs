using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.Shared.Core.Enums
{
    [Flags]
    public enum DeviceStatus
    {
        Online = 0b_0000_0000,
        Offline = 0b_0000_0001,
        CannotDownloadFile = 0b_0000_0010,
        VideoViewLostFocus = 0b_0000_0100,
        HDMICableUnplagged = 0b_0000_1000
    }
}

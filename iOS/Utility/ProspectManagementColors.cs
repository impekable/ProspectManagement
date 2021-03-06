﻿using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace ProspectManagement.iOS.Utility
{
    public static class ProspectManagementColors
    {
        public static UIColor AccentColor = new UIColor(134 / 255f, 188 / 255f, 194 /255f, 1);
        public static UIColor DarkColor = new UIColor(134 / 255f, 188 / 255f, 194 / 255f, 1);
        public static UIColor DarkTextColor = new UIColor(55 / 255f, 59 / 255f, 73 / 255f, 1);
        public static UIColor BorderColor = new UIColor(255 / 255f, 225 / 255f, 225 / 255f, 1);
        public static UIColor LabelColor = UIDevice.CurrentDevice.CheckSystemVersion(13, 0) ? UIColor.LabelColor : UIColor.Black;
        public static UIColor MessageColorGray = UIDevice.CurrentDevice.CheckSystemVersion(13, 0) ? UIColor.SystemGray4Color : new UIColor(229 / 255f, 229 / 255f, 229 / 255f, 1);
    }
}

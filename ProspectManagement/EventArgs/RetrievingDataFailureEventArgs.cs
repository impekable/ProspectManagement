﻿using System;

namespace ProspectManagement.Core
{
    public class RetrievingDataFailureEventArgs : EventArgs
    {
        readonly string _retrievingDataFailureMessage;

        public RetrievingDataFailureEventArgs(string retrievingDataFailureText)
        {
            _retrievingDataFailureMessage = retrievingDataFailureText;
        }

        public string RetrievingDataFailureMessage => _retrievingDataFailureMessage;
    }
}

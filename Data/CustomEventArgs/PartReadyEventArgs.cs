using System;

namespace Data.CustomEventArgs
{
    public class PartReadyEventArgs : EventArgs
    {
        public byte[] Part { get; set; }
    }
}

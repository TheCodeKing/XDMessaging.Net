using System;
using XDMessaging.Messages;

namespace XDMessaging
{
    // ReSharper disable once InconsistentNaming
    public sealed class XDMessageEventArgs : EventArgs
    {
        public XDMessageEventArgs(DataGram dataGram)
        {
            DataGram = dataGram;
        }

        public DataGram DataGram { get; }
    }
}
using System;
using Conditions;

namespace Messenger
{
    [Serializable]
    public class FormattedUserMessage
    {
        public FormattedUserMessage(string formatMessage, params string[] args)
        {
            formatMessage.Requires("formatMessage").IsNotNull();
            args.Requires("args").IsNotNull();

            FormattedTextMessage = string.Format(formatMessage, args);
        }

        public string FormattedTextMessage { get; }
    }
}
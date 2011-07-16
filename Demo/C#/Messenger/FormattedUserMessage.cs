using System;

namespace TheCodeKing.Demo
{
    /// <summary>
    /// Class used to demostrate sending objects via the XDMessaging library.
    /// </summary>
    [Serializable]
    public class FormattedUserMessage
    {
        #region Constructors and Destructors

        public FormattedUserMessage(string formatMessage, params string[] args)
        {
            if (string.IsNullOrEmpty(formatMessage))
            {
                throw new ArgumentException("formatMessage");
            }
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            FormattedTextMessage = string.Format(formatMessage, args);
        }

        #endregion

        #region Properties

        public string FormattedTextMessage { get; set; }

        #endregion
    }
}
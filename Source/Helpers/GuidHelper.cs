using System;

namespace TheCodeKing.Net.Messaging.Helpers
{
    public static class GuidHelper
    {
        #region Public Methods

        public static bool TryParse(string value, out Guid guid)
        {
            try
            {
                var parsedValue = new Guid(value);
                guid = parsedValue;
                return true;
            }
            catch (ArgumentNullException)
            {
            }
            catch (FormatException)
            {
            }
            catch (OverflowException)
            {
            }
            guid = Guid.Empty;
            return false;
        }

        #endregion
    }
}
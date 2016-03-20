using System;

namespace SocialInfoSampleApp.BackgroundAgent.Core
{
    public static class MessageBodyPayloadFieldExtensions
    {
        /// <summary>
        /// Gets the name of a value in the MessageBodyPayloadField enum
        /// </summary>
        /// <param name="field">The enum value to get the name for</param>
        /// <returns>A string with the name of the enum value</returns>
        public static string GetName(this MessageBodyPayloadField field)
        {
            return Enum.GetName(typeof(MessageBodyPayloadField), field);
        }
    }
}

using System;
using Windows.Foundation.Collections;

namespace SocialInfoSampleApp.BackgroundAgent.Core
{
    /// <summary>
    /// A basic interface for a message
    /// </summary>
    public interface ISocialInfoMessage
    {
        /// <summary>
        /// The major version of the protocol that is being used
        /// </summary>
        UInt32 MajorVersion { get; }

        /// <summary>
        /// The minor version of the protocol that is being used
        /// </summary>
        UInt32 MinorVersion { get; }

        /// <summary>
        /// The major and minor version combined into a single value
        /// </summary>
        UInt32 Version { get; }

        /// <summary>
        /// The type of operation that will be performed
        /// </summary>
        OperationType OperationType { get; }

        /// <summary>
        /// Serializes the message into a ValueSet.
        /// </summary>
        /// <returns>A ValueSet with the correct fields set</returns>
        ValueSet Serialize();
    }
}

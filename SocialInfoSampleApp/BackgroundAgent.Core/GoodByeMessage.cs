using System;

namespace SocialInfoSampleApp.BackgroundAgent.Core
{
    /// <summary>
    /// The implementation of a GoodByeMessage
    /// </summary>
    public sealed class GoodByeMessage : SocialInfoMessageBase
    {
        /// <summary>
        /// Constructs a new GoodByeMessage
        /// </summary>
        /// <param name="version">The protocol version being used</param>
        public GoodByeMessage(UInt32 version) :
            base(version)
        {
            OperationType = OperationType.GoodBye;
        }
    }
}

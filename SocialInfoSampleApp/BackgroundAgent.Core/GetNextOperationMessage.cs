using System;

namespace SocialInfoSampleApp.BackgroundAgent.Core
{
    /// <summary>
    /// The implementation of a GetNextOperationMessage
    /// </summary>
    public sealed class GetNextOperationMessage : SocialInfoMessageBase
    {
        /// <summary>
        /// Constructs a new GetNextOperationMessage
        /// </summary>
        /// <param name="version">The protocol version being used</param>
        public GetNextOperationMessage(UInt32 version) :
            base(version)
        {
            OperationType = OperationType.GetNext;
        }
    }
}
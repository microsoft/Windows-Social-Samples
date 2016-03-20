using System;
using Windows.Foundation.Collections;

namespace SocialInfoSampleApp.BackgroundAgent.Core
{
    /// <summary>
    /// An implementation of the OperationResultMessage
    /// </summary>
    public sealed class OperationResultMessage : SocialInfoMessageBase
    {
        /// <summary>
        /// The error code associated with the operation
        /// </summary>
        private UInt32 mErrorCode;

        /// <summary>
        /// The operation ID for the operation
        /// </summary>
        private UInt32 mOperationId;

        public UInt32 ErrorCode
        {
            get
            {
                return mErrorCode;
            }

            private set
            {
                mErrorCode = value;
            }
        }

        public UInt32 OperationId
        {
            get
            {
                return mOperationId;
            }

            private set
            {
                mOperationId = value;
            }
        }

        /// <summary>
        /// Constructs an OperationResultMessage
        /// </summary>
        /// <param name="version">The protocol version being used</param>
        /// <param name="errorCode">The error code associated with the operation</param>
        /// <param name="operationId">The ID of the operation that was performed</param>
        public OperationResultMessage(
            UInt32 version,
            UInt32 errorCode,
            UInt32 operationId) :
            base(version)
        {
            OperationType = OperationType.OperationResult;
            OperationId = operationId;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Serializes the object into a ValueSet.
        /// </summary>
        public override ValueSet Serialize()
        {
            ValueSet fields = base.Serialize();

            fields.Add(
                MessageBodyPayloadField.ErrorCode.GetName(),
                (UInt32)ErrorCode);

            fields.Add(
                MessageBodyPayloadField.OperationId.GetName(),
                (UInt32)OperationId);

            return fields;
        }
    }
}

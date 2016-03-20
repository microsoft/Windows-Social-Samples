using System;
using Windows.Foundation.Collections;

namespace SocialInfoSampleApp.BackgroundAgent.Core
{
    public static class SocialInfoMessageFactory
    {
        /// <summary>
        /// Parses a message and creates the appropriate ISocialMessage type.
        /// </summary>
        /// <param name="messageBody">
        /// The ValueSet that contains all of the key-value pairs
        /// </param>
        /// <returns>An ISocialInfoMessage for the operation</returns>
        public static ISocialInfoMessage CreateSocialInfoMessage(ValueSet messageBody)
        {
            // Get the operation type to determine which type of message to create.
            UInt32 type = (UInt32)ParseField(messageBody, MessageBodyPayloadField.Type);

            // Get the version of the message
            UInt32 version = (UInt32)ParseField(messageBody, MessageBodyPayloadField.Version);

            // At this point, we have enough information to construct the specific message type.
            OperationType operationType = (OperationType)(type);

            ISocialInfoMessage message = null;
            switch (operationType)
            {
                case OperationType.GoodBye:
                    message = new GoodByeMessage(version);
                    break;

                case OperationType.DownloadContactFeed:
                case OperationType.DownloadDashboardFeed:
                case OperationType.DownloadHomeFeed:
                    message = ParseDownloadFeedMessage(version, operationType, messageBody);
                    break;

                case OperationType.GetNext:
                    message = new GetNextOperationMessage(version);
                    break;

                case OperationType.OperationResult:
                    message = ParseOperationResultMessage(version, messageBody);
                    break;

                default:
                    throw new ArgumentException("The operation type is not valid.");
            }

            return message;
        }

        /// <summary>
        /// Parses an OperationResult message
        /// </summary>
        /// <param name="version">The version of the protocol being used</param>
        /// <param name="operationType">The operation that is being performed</param>
        private static ISocialInfoMessage ParseOperationResultMessage(
            UInt32 version,
            ValueSet messageBody)
        {
            UInt32 operationId = (UInt32)ParseField(messageBody, MessageBodyPayloadField.OperationId);
            UInt32 errorCode = (UInt32)ParseField(messageBody, MessageBodyPayloadField.ErrorCode);

            return new OperationResultMessage(version, errorCode, operationId);
        }

        /// <summary>
        /// Parses a download feed message
        /// </summary>
        /// <param name="version">The version of the protocol being used</param>
        /// <param name="operationType">The operation that is being performed</param>
        /// <param name="messageBody">The ValueSet that contains all of the fields</param>
        private static ISocialInfoMessage ParseDownloadFeedMessage(
            UInt32 version,
            OperationType operationType,
            ValueSet messageBody)
        {
            UInt32 operationId = (UInt32)ParseField(messageBody, MessageBodyPayloadField.OperationId);
            string ownerRemoteId = ParseField(messageBody, MessageBodyPayloadField.OwnerRemoteId) as string;

            // This is an optional field
            string lastFeedItemRemoteId = ParseField(
                messageBody,
                MessageBodyPayloadField.LastFeedItemRemoteId,
                false) as string;

            DateTimeOffset? lastFeedItemTimestamp =
                ParseField(messageBody, MessageBodyPayloadField.LastFeedItemTimestamp) as DateTimeOffset?;

            UInt32 itemCount = (UInt32)ParseField(messageBody, MessageBodyPayloadField.ItemCount);
            bool isFetchMore = (bool)ParseField(messageBody, MessageBodyPayloadField.IsFetchMore);

            Type objectType = null;
            switch (operationType)
            {
                case OperationType.DownloadContactFeed:
                    objectType = typeof(DownloadContactFeedMessage);
                    break;

                case OperationType.DownloadDashboardFeed:
                    objectType = typeof(DownloadDashboardFeedMessage);
                    break;

                case OperationType.DownloadHomeFeed:
                    objectType = typeof(DownloadHomeFeedMessage);
                    break;

                default:
                    throw new ArgumentException("Unexpected feed type");
            }

            // Create the message using the type information from above
            ISocialInfoMessage message = Activator.CreateInstance(
                objectType,
                new object[]
                {
                    version,
                    operationId,
                    ownerRemoteId,
                    lastFeedItemRemoteId,
                    lastFeedItemTimestamp.Value.DateTime,
                    itemCount,
                    isFetchMore
                }) as ISocialInfoMessage;

            return message;
        }

        /// <summary>
        /// Parses the selected field from the messageBody
        /// </summary>
        /// <param name="messageBody">The property bag</param>
        /// <param name="field">The desired field</param>
        /// <param name="validateField">
        /// True to validate the field for being null. If false, null values are acceptable.
        /// </param>
        private static object ParseField(
            ValueSet messageBody,
            MessageBodyPayloadField field,
            bool validateField = true)
        {
            string fieldName = field.GetName();

            object fieldValue = messageBody[fieldName];

            if (validateField)
            {
                ValidateParameter(fieldValue, fieldName);
            }

            return fieldValue;
        }

        /// <summary>
        /// Validates the object as not being null. Throws an ArgumentException
        /// when the value is null.
        /// </summary>
        /// <param name="value">The string value from the body</param>
        /// <param name="fieldName">The name of the field being validated</param>
        private static void ValidateParameter(object value, string fieldName)
        {
            if (value == null)
            {
                throw new ArgumentException("The " +
                    fieldName +
                    " of the message could not be found.");
            }
        }
    }
}

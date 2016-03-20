namespace SocialInfoSampleApp.BackgroundAgent.Core
{
    /// <summary>
    /// The fields in a message body
    /// </summary>
    public enum MessageBodyPayloadField
    {
        /// <summary>
        /// The version of the protocol that is being used
        /// </summary>
        Version,

        /// <summary>
        /// The type of operation that is being performed
        /// </summary>
        Type,

        /// <summary>
        /// The ID of the operation
        /// </summary>
        OperationId,

        /// <summary>
        /// The social media service specific ID of the owner
        /// </summary>
        OwnerRemoteId,

        /// <summary>
        /// The social media service specific ID of the feed item
        /// </summary>
        LastFeedItemRemoteId,

        /// <summary>
        /// The timestamp of the last retrieved feed
        /// </summary>
        LastFeedItemTimestamp,

        /// <summary>
        /// The number of items that are requested
        /// </summary>
        ItemCount,

        /// <summary>
        /// IsFetchMore
        /// </summary>
        IsFetchMore,

        /// <summary>
        /// The error code associated with an operation
        /// </summary>
        ErrorCode,
    }
}

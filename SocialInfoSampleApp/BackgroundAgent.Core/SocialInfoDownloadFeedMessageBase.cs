using System;
using System.Threading.Tasks;

namespace SocialInfoSampleApp.BackgroundAgent.Core
{
    /// <summary>
    /// An abstract class to implement the required properties of an
    /// ISocialInfoDownloadFeedMessage
    /// </summary>
    public abstract class SocialInfoDownloadFeedMessageBase :
        SocialInfoMessageBase,
        ISocialInfoDownloadFeedMessage
    {
        /// <summary>
        /// The operation ID for the operation
        /// </summary>
        private UInt32 mOperationId;

        /// <summary>
        /// The remote ID for the owner
        /// </summary>
        private string mOwnerRemoteId;

        /// <summary>
        /// The remote ID for the last feed item that was downloaded
        /// </summary>
        private string mLastFeedItemRemoteId;

        /// <summary>
        /// The timestamp of the last feed item that was downloaded
        /// </summary>
        private DateTime mLastFeedItemTimestamp;

        /// <summary>
        /// The number of requested items
        /// </summary>
        private UInt32 mItemCount;

        /// <summary>
        /// IsFetchMore
        /// </summary>
        private bool mIsFetchMore;

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

        public string OwnerRemoteId
        {
            get
            {
                return mOwnerRemoteId;
            }

            private set
            {
                mOwnerRemoteId = value;
            }
        }

        public string LastFeedItemRemoteId
        {
            get
            {
                return mLastFeedItemRemoteId;
            }

            private set
            {
                mLastFeedItemRemoteId = value;
            }
        }

        public DateTime LastFeedItemTimestamp
        {
            get
            {
                return mLastFeedItemTimestamp;
            }

            private set
            {
                mLastFeedItemTimestamp = value;
            }
        }

        public UInt32 ItemCount
        {
            get
            {
                return mItemCount;
            }

            private set
            {
                mItemCount = value;
            }
        }

        public bool IsFetchMore
        {
            get
            {
                return mIsFetchMore;
            }

            private set
            {
                mIsFetchMore = value;
            }
        }

        /// <summary>
        /// Constructor for the base class
        /// </summary>
        /// <param name="version">The protocol version</param>
        /// <param name="operationId">The operation ID</param>
        /// <param name="ownerRemoteId">The ID of the owner of the remote system</param>
        /// <param name="lastFeedItemRemoteId">The ID of the last feed item</param>
        /// <param name="lastFeedItemTimestamp">The timestamp of the last feed item</param>
        /// <param name="itemCount">The number of items requested</param>
        /// <param name="isFetchMore">IsFetchMore</param>
        protected SocialInfoDownloadFeedMessageBase(
            UInt32 version,
            UInt32 operationId,
            string ownerRemoteId,
            string lastFeedItemRemoteId,
            DateTime lastFeedItemTimestamp,
            UInt32 itemCount,
            bool isFetchMore) :
           base(version)
        {
            OperationId = operationId;
            OwnerRemoteId = ownerRemoteId;
            LastFeedItemRemoteId = lastFeedItemRemoteId;
            LastFeedItemTimestamp = lastFeedItemTimestamp;
            ItemCount = itemCount;
            IsFetchMore = isFetchMore;
        }

        /// <summary>
        /// The implementation for downloading feeds. This must be implemented
        /// by all derived classes.
        /// </summary>
        /// <returns>An awaitable task</returns>
        public abstract Task DownloadFeedAsync();
    }
}

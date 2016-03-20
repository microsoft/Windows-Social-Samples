using SocialInfoSampleApp.BackgroundAgent.Core.Feeds;
using SocialInfoSampleApp.SharedComponents;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.SocialInfo;
using Windows.ApplicationModel.SocialInfo.Provider;

namespace SocialInfoSampleApp.BackgroundAgent.Core
{
    public sealed class DownloadDashboardFeedMessage : SocialInfoDownloadFeedMessageBase
    {
        /// <summary>
        /// Constructs a DownloadDashboardFeedMessage
        /// </summary>
        /// <param name="version">The protocol version being used</param>
        /// <param name="operationId">The ID of the operation</param>
        /// <param name="ownerRemoteId">The ID of the owner on the remote system</param>
        /// <param name="lastFeedItemRemoteId">
        /// The ID of the last feed item that was downloaded
        /// </param>
        /// <param name="lastFeedItemTimestamp">
        /// The timestamp when the last feed item was downloaded
        /// </param>
        /// <param name="itemCount">The number of requested feed items</param>
        /// <param name="isFetchMore">IsFetchMore</param>
        public DownloadDashboardFeedMessage(
            UInt32 version,
            UInt32 operationId,
            string ownerRemoteId,
            string lastFeedItemRemoteId,
            DateTime lastFeedItemTimestamp,
            UInt32 itemCount,
            bool isFetchMore) :
            base(version,
                operationId,
                ownerRemoteId,
                lastFeedItemRemoteId,
                lastFeedItemTimestamp,
                itemCount,
                isFetchMore)
        {
            OperationType = OperationType.DownloadDashboardFeed;
        }

        /// <summary>
        /// Downloads the feed asynchronously
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public override async Task DownloadFeedAsync()
        {
            // Get the dashboard feed item from the database
            FeedItem dashboardFeedItem = InMemorySocialCache.Instance.GetDashboardFeed(OwnerRemoteId);

            if (dashboardFeedItem != null)
            {
                // Check if the platform supports the SocialInfo APIs
                if (!PlatformUtilities.IsSocialInfoApiAvailable())
                {
                    return;
                }

                SocialDashboardItemUpdater dashboard =
                    await SocialInfoProviderManager.CreateDashboardItemUpdaterAsync(OwnerRemoteId);

                dashboard.Content.Message = dashboardFeedItem.Message;
                dashboard.Content.Title = dashboardFeedItem.Title;
                dashboard.Timestamp = dashboardFeedItem.Timestamp;

                // The TargetUri of the dashboard always has to be set
                dashboard.TargetUri = dashboardFeedItem.TargetUri;

                // For a thumbnail, there must always be a TargetUri.
                if ((dashboardFeedItem.ImageUri != null) && (dashboardFeedItem.TargetUri != null))
                {
                    dashboard.Thumbnail = new SocialItemThumbnail()
                    {
                        ImageUri = dashboardFeedItem.ImageUri,
                        TargetUri = dashboardFeedItem.TargetUri
                    };
                }

                await dashboard.CommitAsync();
            }
        }
    }
}

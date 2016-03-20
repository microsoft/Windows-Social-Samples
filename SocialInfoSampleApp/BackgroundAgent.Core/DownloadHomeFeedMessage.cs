using SocialInfoSampleApp.BackgroundAgent.Core.Feeds;
using SocialInfoSampleApp.SharedComponents;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.SocialInfo;
using Windows.ApplicationModel.SocialInfo.Provider;

namespace SocialInfoSampleApp.BackgroundAgent.Core
{
    public sealed class DownloadHomeFeedMessage : SocialInfoDownloadFeedMessageBase
    {
        /// <summary>
        /// Constructs a DownloadHomeFeedMessage
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
        public DownloadHomeFeedMessage(
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
            OperationType = OperationType.DownloadHomeFeed;
        }

        /// <summary>
        /// Downloads the feed asynchronously
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public override async Task DownloadFeedAsync()
        {
            // Query the "database" for the home feeds
            IEnumerable<FeedItem> homeFeedItems =
                InMemorySocialCache.Instance.GetHomeFeeds(OwnerRemoteId, ItemCount);

            // Check if the platform supports the SocialInfo APIs
            if (!PlatformUtilities.IsSocialInfoApiAvailable())
            {
                return;
            }

            // Create the social feed updater
            SocialFeedUpdater feedUpdater = await SocialInfoProviderManager.CreateSocialFeedUpdaterAsync(
                SocialFeedKind.HomeFeed,
                SocialFeedUpdateMode.Replace,
                OwnerRemoteId);

            // Generate each of the feed items
            foreach (FeedItem fi in homeFeedItems)
            {
                SocialFeedItem item = new SocialFeedItem();

                item.Timestamp = fi.Timestamp;
                item.RemoteId = fi.Id;
                item.TargetUri = fi.TargetUri;
                item.Author.DisplayName = fi.AuthorDisplayName;
                item.Author.RemoteId = fi.AuthorId;
                item.PrimaryContent.Title = fi.Title;
                item.PrimaryContent.Message = fi.Message;

                if (fi.ImageUri != null)
                {
                    item.Thumbnails.Add(new SocialItemThumbnail()
                    {
                        TargetUri = fi.TargetUri,
                        ImageUri = fi.ImageUri
                    });
                }

                feedUpdater.Items.Add(item);
            }

            await feedUpdater.CommitAsync();
        }
    }
}

using System;

namespace SocialInfoSampleApp.BackgroundAgent.Core.Feeds
{
    /// <summary>
    /// A simple class that represents a feed message on the remote system.
    /// </summary>
    public class FeedItem
    {
        /// <summary>
        /// The type of feed
        /// </summary>
        public FeedType FeedType { get; set; }

        /// <summary>
        /// The URI the feed is linked to
        /// </summary>
        public Uri TargetUri { get; set; }

        /// <summary>
        /// The URI of the image associated with this feed. If this
        /// property is null, there is no image.
        /// </summary>
        public Uri ImageUri { get; set; }

        /// <summary>
        /// The title of the feed item
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The body of the feed item
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The ID of the author
        /// </summary>
        public string AuthorId { get; set; }

        /// <summary>
        /// The display name of the author
        /// </summary>
        public string AuthorDisplayName { get; set; }

        /// <summary>
        /// The ID of the feed item
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The timestamp of the feed item
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}

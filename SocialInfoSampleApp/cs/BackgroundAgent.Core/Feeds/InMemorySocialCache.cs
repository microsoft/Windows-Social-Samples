using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Storage;

namespace SocialInfoSampleApp.BackgroundAgent.Core.Feeds
{
    /// <summary>
    /// A class that represents a database of feed items that can be downloaded.
    /// </summary>
    public class InMemorySocialCache
    {
        /// <summary>
        /// The URL of the image to show if the feed item has an associated image.
        /// </summary>
        private const string cImageUrl = 
            "https://mediaplatstorage1.blob.core.windows.net/windows-universal-samples-media/rainier.jpg";

        /// <summary>
        /// A singleton instance of this class
        /// </summary>
        private static readonly Lazy<InMemorySocialCache> mInstance =
            new Lazy<InMemorySocialCache>(() => new InMemorySocialCache());

        /// <summary>
        /// A list of FeedItem that are stored
        /// </summary>
        private IList<FeedItem> mItems;

        /// <summary>
        /// Constructs the database by reading the XML file that contains that data
        /// </summary>
        private InMemorySocialCache()
        {
            this.mItems = new List<FeedItem>();

            Task.Run(async () =>
            {
                // Get the feeds file from the package
                StorageFile file = await Package.Current.InstalledLocation.TryGetItemAsync("Assets\\Feeds.xml") as StorageFile;

                if (file == null)
                {
                    return;
                }

                XDocument doc = XDocument.Load(file.Path);
                IEnumerable<XElement> feeds = doc.Descendants("Feed");

                // Get a list of feed items
                foreach (XElement feed in feeds)
                {
                    FeedItem item = new FeedItem()
                    {
                        AuthorDisplayName = feed.Attribute("AuthorDisplayName").Value,
                        Message = feed.Attribute("Body").Value,
                        Title = feed.Attribute("Title").Value,
                        AuthorId = feed.Attribute("AuthorId").Value,
                        Timestamp = Convert.ToDateTime(feed.Attribute("Timestamp").Value),
                        Id = feed.Attribute("Id").Value,
                        TargetUri = new Uri(feed.Attribute("TargetUri").Value),
                    };

                    switch (feed.Attribute("FeedType").Value)
                    {
                        case "Contact":
                            item.FeedType = FeedType.Contact;
                            break;

                        case "Home":
                            item.FeedType = FeedType.Home;
                            break;

                        case "Dashboard":
                            item.FeedType = FeedType.Dashboard;
                            break;

                        default:
                            throw new ArgumentException("Unexpected feed type.");

                    }

                    if (Convert.ToBoolean(feed.Attribute("UseImage").Value))
                    {
                        item.ImageUri = new Uri(cImageUrl);
                    }

                    this.mItems.Add(item);
                }
            }).Wait();
        }

        /// <summary>
        /// Gets the instance of the cache
        /// </summary>
        public static InMemorySocialCache Instance
        {
            get
            {
                return mInstance.Value;
            }
        }

        /// <summary>
        /// Queries the database for all of the home feeds
        /// </summary>
        /// <param name="authorId">The ID of the user for whom the feeds should be obtained</param>
        /// <param name="itemCount">The number of feeds to get</param>
        public IEnumerable<FeedItem> GetHomeFeeds(string authorId, UInt32 itemCount)
        {
            IEnumerable<FeedItem> homeFeedItems = this.mItems.
                Where(p => p.AuthorId != authorId).
                Where(p => p.FeedType == FeedType.Home).
                OrderByDescending(p => p.Timestamp).
                Take((int)itemCount);

            return homeFeedItems;
        }

        /// <summary>
        /// Gets the dashboard feed item for the specified user
        /// </summary>
        /// <param name="authorId">The ID of the user to get the feed item for</param>
        public FeedItem GetDashboardFeed(string authorId)
        {
            return mItems.
                Where(p => p.AuthorId == authorId).
                Where(p => p.FeedType == FeedType.Dashboard).
                OrderByDescending(p => p.Timestamp).
                FirstOrDefault();
        }

        /// <summary>
        /// Gets the contact feeds for the specified user
        /// </summary>
        /// <param name="ownerRemoteId">The ID of the user for whom the feeds should be obtained</param>
        /// <param name="itemCount">The number of feeds to get</param>
        public IEnumerable<FeedItem> GetContactFeeds(string authorId, UInt32 itemCount)
        {
            IEnumerable<FeedItem> contactFeedItems = this.mItems.
                Where(p => p.AuthorId != authorId).
                Where(p => p.FeedType == FeedType.Contact).
                OrderByDescending(p => p.Timestamp).
                Take((int)itemCount);

            return contactFeedItems;
        }
    }
}

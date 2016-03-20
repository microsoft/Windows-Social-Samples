using System;
using System.Threading.Tasks;

namespace SocialInfoSampleApp.BackgroundAgent.Core
{
    /// <summary>
    /// An interface for a download feed message
    /// </summary>
    public interface ISocialInfoDownloadFeedMessage
    {
        /// <summary>
        /// Gets the operation ID associated with this operation
        /// </summary>
        UInt32 OperationId { get; }

        /// <summary>
        /// Downloads the feed from the social media service
        /// </summary>
        Task DownloadFeedAsync();
    }
}

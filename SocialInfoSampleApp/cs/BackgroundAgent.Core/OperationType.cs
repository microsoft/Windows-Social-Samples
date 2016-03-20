namespace SocialInfoSampleApp.BackgroundAgent.Core
{
    /// <summary>
    /// Defines the operation types that are supported between
    /// the app and the agent.
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// Request the next operation from the app
        /// </summary>
        GetNext = 0x10,

        /// <summary>
        /// Download the home feed for the specified user
        /// </summary>
        DownloadHomeFeed = 0x11,

        /// <summary>
        /// Download the contact feed for the specified user
        /// </summary>
        DownloadContactFeed = 0x13,

        /// <summary>
        /// Download the dashboard feed for the specified user
        /// </summary>
        DownloadDashboardFeed = 0x15,

        /// <summary>
        /// Send the result of an operation back to the app
        /// </summary>
        OperationResult = 0x80,

        /// <summary>
        /// End the processing loop of the background agent
        /// </summary>
        GoodBye = 0xF1,
    }
}
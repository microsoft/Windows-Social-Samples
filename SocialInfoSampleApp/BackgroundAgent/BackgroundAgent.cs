using SocialInfoSampleApp.BackgroundAgent.Core;
using SocialInfoSampleApp.SharedComponents;
using System;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Metadata;

namespace SocialInfoSampleApp.BackgroundAgent
{
    public sealed class BackgroundAgent : IBackgroundTask
    {
        /// <summary>
        /// Holds the task deferral. This will allow the background agent to continue
        /// running even after the Run method returns due to asynchronous operations.
        /// </summary>
        private BackgroundTaskDeferral mTaskDeferral;

        /// <summary>
        /// A connection to the AppService.
        /// </summary>
        private AppServiceConnection mAppServiceConnection;

        /// <summary>
        /// The major version of the protocol that is being used
        /// </summary>
        private const UInt32 cSocialAppContactMajorVersion = 1;

        /// <summary>
        /// The minor version of the protocol that is being used
        /// </summary>
        private const UInt32 cSocialAppContactMinorVersion = 0;

        /// <summary>
        /// Packs the version into one UInt32
        /// </summary>
        private static UInt32 SocialAppContactVersion
        {
            get
            {
                return ((cSocialAppContactMajorVersion << 16) | cSocialAppContactMinorVersion);
            }
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get the task deferral and the app service connection
            this.mTaskDeferral = taskInstance.GetDeferral();
            AppServiceTriggerDetails triggerDetails =
                taskInstance.TriggerDetails as AppServiceTriggerDetails;
            this.mAppServiceConnection = triggerDetails.AppServiceConnection;

            // Continue processing messages until the GoodBye message is sent
            bool continueProcessing = true;

            // Before exiting, this method needs to complete the deferral that
            // was acquired. Catch all exceptions that may be thrown.
            try
            {
                while (continueProcessing)
                {
                    // Create a next operation message to send to the app
                    GetNextOperationMessage getNextOperationMessage =
                        new GetNextOperationMessage(SocialAppContactVersion);

                    // Send a message to the app
                    AppServiceResponse response =
                        await this.mAppServiceConnection.SendMessageAsync(
                            getNextOperationMessage.Serialize());

                    if (response == null)
                    {
                        throw new InvalidOperationException("A null response was received.");
                    }

                    // Check the status
                    if (response.Status != AppServiceResponseStatus.Success)
                    {
                        throw new Exception(String.Format(
                            "App service response was unsuccessful. Status = {0}",
                            response.Status));
                    }

                    // Parse the response to get the correct message type
                    ISocialInfoMessage message =
                        SocialInfoMessageFactory.CreateSocialInfoMessage(response.Message);

                    // Check the version of the message
                    // If the version of the message is not the expected version by this agent,
                    // then there was a protocol change in the People app. You will need to
                    // contact the People app team at Microsoft to get the most up-to-date
                    // protocol definition.
                    if (message.MajorVersion != cSocialAppContactMajorVersion)
                    {
                        throw new InvalidOperationException(
                            "This version of the protocol is not supported.");
                    }

                    // Handle the message based on its type
                    switch (message.OperationType)
                    {
                        case OperationType.DownloadContactFeed:
                        case OperationType.DownloadDashboardFeed:
                        case OperationType.DownloadHomeFeed:

                            // Cast the message to an ISocialInfoDownloadFeedMessage
                            ISocialInfoDownloadFeedMessage downloadFeedMessage = 
                                message as ISocialInfoDownloadFeedMessage;

                            // Keep track of the operation ID to report it
                            UInt32 operationId = downloadFeedMessage.OperationId;

                            // Save the error code associated with the operation
                            UInt32 errorCode = 0;

                            try
                            {
                                // Download the feed
                                await downloadFeedMessage.DownloadFeedAsync();
                            }
                            catch (Exception exception)
                            {
                                errorCode = (UInt32)exception.HResult;
                            }

                            // Create the operation result message
                            OperationResultMessage resultMessage =
                                new OperationResultMessage(
                                    SocialAppContactVersion,
                                    errorCode,
                                    downloadFeedMessage.OperationId);

                            // Send the result back to the app
                            await this.mAppServiceConnection.SendMessageAsync(
                                resultMessage.Serialize());

                            break;

                        case OperationType.GoodBye:
                            continueProcessing = false;
                            break;

                        default:
                            throw new InvalidOperationException(
                                "The selected operation type is not supported.");
                    }
                }
            }
            catch
            {
                // Throw the exception
                throw;
            }
            finally
            {
                // Complete the task deferral
                this.mTaskDeferral.Complete();

                this.mTaskDeferral = null;
                this.mAppServiceConnection = null;
            }
        }
    }
}
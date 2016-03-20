# Social feeds integration sample

This sample app demonstrates how to integrate social feeds into Windows 10, with all of the emphasis placed on writing a background agent that will help us perform the processing.

The app is written as a Windows 10 application on the Universal Windows Platform. Currently, these APIs are available on PC, Tablet, and Phone, however may not be present on other Windows 10 devices. We're currently working to ensure this is supported on all devices by the Universal Windows Platform.

## The sample app consists of four projects:

### SocialInfoSampleApp

The foreground app with a very basic UI to simulate login/logout behavior.

### SocialInfoSampleApp.SharedComponents

This is a winmd project that has the shared logic for reading and writing a setting to mark whether or not the user is logged in.

### SocialInfoSampleApp.BackgroundAgent

This is the code for the background agent, which is where all of the SocialInfo APIs are used to integrate the feeds into the People app and uses the AppService to communicate with it. The implementation of this agent is a simple processing loop.

- Get a message from the AppService
- Decode the message - The message types are as follows:
    - GetNextOperation - Sent from the agent to the People app to get the next operation for the agent to perform
    - GoodBye - Issued by the People app when there are no more operations to perform
    - DownloadContactFeed - Issued by the People app to indicate that the feeds should be retrieved for a certain contact
    - DownloadHomeFeed - Issued by the People app to indicate that the feeds should be retrieved for the primary user
    - DownloadDashboarFeed - Issued by the People app to indicate that the feeds should be retrieved for the primary user
    - OperationResult - Sent from the agent to the People app to indicate the result of the requested operation
- Perform some operation
- Send a result
- Repeat the above steps


### SocialInfoSampleApp.BackgroundAgent.Core

This is a .NET project to handle the AppService message parsing and downloading of feeds. It also is responsible for simulating the download of feeds from some external web service.

Important classes:

- SocialInfoMessageFactory - Handles the parsing and serialization of messages that are from the received from and sent to the AppService.
- ISocialInfoMessage - All messages derive from this interface. The decision as to what operation to perform is based on the OperationType property.
- ISocialInfoDownloadFeedMessage - Any message that indicates a feed download will inherit from this interface. The important method to understand is DownloadFeedAsync. This is responsible for downloading the requested feed type from an external source.

**NOTE:** This method is not the only way to write the download operation. It is possible to call another method from the BackgroundAgent that will do the feed download without having to use this method.
              
In the SocialInfoSampleApp.BackgroundAgent.Core.Feeds namespace, there are 3 classes that represent a feed item. These classes are creating for illustration purposes. The specific structure of the feed items from the external source should be used in their place.
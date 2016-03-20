using SocialInfoSampleApp.SharedComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.SocialInfo.Provider;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SocialInfoSampleApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// The name of the contact list that is created and used by this app
        /// </summary>
        private const string cContactListName = "SocialInfoSampleApp";

        public MainPage()
        {
            this.InitializeComponent();

            // Determine if the user was logged in already
            bool isUserLoggedIn = ApplicationSettings.IsUserLoggedIn();

            // Enable/disable the buttons
            LogInButton.IsEnabled = !isUserLoggedIn;
            LogOutButton.IsEnabled = isUserLoggedIn;
        }

        /// <summary>
        /// Handler for the user clicking the Login button
        /// </summary>
        public async void LogIn_Button_Click(object sender, RoutedEventArgs e)
        {
            // Authenticate the user with the service.
            // This app does not have a web service, so there is no authentication
            // that needs to happen.

            // Get the ContactList and ContactAnnotationList
            ContactList contactList = await _GetContactListAsync();
            ContactAnnotationList annotationList = await _GetContactAnnotationListAsync();

            // After the user was authenticated, create a Me contact in the contact
            // list for this app.
            await _CreateMeContactAsync(contactList, annotationList);

            // Provision the app for social feed integration
            await _SocialInfoProvisionAsync();

            // At this point, the user was logged in and the contact was created.
            // Mark the user as being logged in for future launches and for the
            // background agent.
            ApplicationSettings.SaveIsUserLoggedIn(true);

            // Update the state of the buttons
            LogInButton.IsEnabled = false;
            LogOutButton.IsEnabled = !LogInButton.IsEnabled;

            // Sync the contacts
            await _SyncContactsAsync(contactList, annotationList);
        }

        /// <summary>
        /// Handler for the user clicking the LogOut button
        /// </summary>
        public async void LogOut_Button_Click(object sender, RoutedEventArgs e)
        {
            // Get the ContactList and ContactAnnotationList
            ContactList contactList = await _GetContactListAsync();

            // Delete the contact list to remove all of the information that was
            // inserted by this app
            await contactList.DeleteAsync();

            // Use the SocialInfoProviderManager to deprovision this app.
            // In build 10240, this API is only present in the Mobile SDK extension.
            if (PlatformUtilities.IsSocialInfoApiAvailable())
            {
                await SocialInfoProviderManager.DeprovisionAsync();
            }

            // Mark the user as no longer logged in
            ApplicationSettings.SaveIsUserLoggedIn(false);

            // Update the state of the buttons
            LogInButton.IsEnabled = true;
            LogOutButton.IsEnabled = !LogInButton.IsEnabled;
        }

        /// <summary>
        /// Provisions the app for People app integration if it is supported
        /// by the current platform.
        /// </summary>
        /// <returns>An awaitable Task for social provisioning</returns>
        private async Task _SocialInfoProvisionAsync()
        {
            // Use the SocialInfoProviderManager to provision this app
            if (PlatformUtilities.IsSocialInfoApiAvailable())
            {
                bool isProvisionSuccessful = await SocialInfoProviderManager.ProvisionAsync();

                // Throw an exception if the app could not be provisioned
                if (!isProvisionSuccessful)
                {
                    throw new Exception("Could not provision the app with the SocialInfoProviderManager.");
                }
            }
        }

        /// <summary>
        /// Sync the contacts for the user
        /// </summary>
        /// <param name="contactList">The ContactList for this app</param>
        /// <param name="annotationList">The ContactAnnotationList</param>
        private async Task _SyncContactsAsync(
            ContactList contactList,
            ContactAnnotationList annotationList)
        {
            // This app will read the contacts from a file and do some at this time.
            // In the case of a web service, this should be done on a background task.

            // Get the XML file that contains that user information
            StorageFile file =
                await Package.Current.InstalledLocation.TryGetItemAsync("Assets\\Contacts.xml")
                as StorageFile;

            if (file == null)
            {
                return;
            }

            // Parse the XML file and create all of the contacts
            XDocument doc = XDocument.Load(file.Path);
            var contacts = doc.Descendants("Contact");

            foreach (XElement contact in contacts)
            {
                Contact currentContact = new Contact();
                currentContact.FirstName = contact.Attribute("FirstName").Value;
                currentContact.LastName = contact.Attribute("LastName").Value;
                currentContact.Emails.Add(new ContactEmail()
                {
                    Address = contact.Attribute("Email").Value,
                    Kind = ContactEmailKind.Personal
                });
                currentContact.RemoteId = contact.Attribute("Id").Value;

                await contactList.SaveContactAsync(currentContact);

                ContactAnnotation annotation = new ContactAnnotation();
                annotation.ContactId = currentContact.Id;
                annotation.RemoteId = currentContact.RemoteId;
                annotation.SupportedOperations =
                    ContactAnnotationOperations.SocialFeeds;

                await annotationList.TrySaveAnnotationAsync(annotation);
            }
        }

        /// <summary>
        /// Creates the Me contact
        /// </summary>
        /// <param name="contactList">The ContactList for this app</param>
        /// <param name="annotationList">The ContactAnnotationList</param>
        private async Task _CreateMeContactAsync(
            ContactList contactList,
            ContactAnnotationList annotationList)
        {
            // All of the contact information will come from the web service.
            // This app will use some default values.
            Contact meContact = await contactList.GetMeContactAsync();

            meContact.FirstName = "Eleanor";
            meContact.LastName = "Taylor";

            meContact.Emails.Add(new ContactEmail()
            {
                Address = "eleanor@fabrikam.com",
                Kind = ContactEmailKind.Personal
            });

            // The RemoteId is the ID used by the web service to identify the user.
            meContact.RemoteId = "Eleanor_Taylor";
            await contactList.SaveContactAsync(meContact);

            // Set the annotations for the me contact.
            // By setting the SocialFeeds annotation, the People app will be able to
            // show social feeds for that contact.
            ContactAnnotation annotation = new ContactAnnotation();
            annotation.ContactId = meContact.Id;
            annotation.RemoteId = meContact.RemoteId;
            annotation.SupportedOperations = ContactAnnotationOperations.SocialFeeds;

            bool saveSuccessful = await annotationList.TrySaveAnnotationAsync(annotation);

            if (!saveSuccessful)
            {
                throw new Exception("Could not save the annotations for the me contact.");
            }
        }

        /// <summary>
        /// Get the ContactList for this app
        /// </summary>
        private async Task<ContactList> _GetContactListAsync()
        {
            // Get the contact store and the contact lists that are already present
            ContactStore store = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);
            IReadOnlyList<ContactList> contactLists = await store.FindContactListsAsync();

            // Find the contact list that belongs to this app
            ContactList contactList = contactLists.
                FirstOrDefault(p => p.DisplayName == cContactListName);

            // Create the contact list if it is not present
            if (contactList == null)
            {
                contactList = await store.CreateContactListAsync(cContactListName);
            }

            return contactList;
        }

        /// <summary>
        /// Get the ContactAnnotationList
        /// </summary>
        private async Task<ContactAnnotationList> _GetContactAnnotationListAsync()
        {
            // Get the store and all of the annotation lists
            ContactAnnotationStore annotationStore = await ContactManager.RequestAnnotationStoreAsync(ContactAnnotationStoreAccessType.AppAnnotationsReadWrite);
            IReadOnlyList<ContactAnnotationList> annotationLists = await annotationStore.FindAnnotationListsAsync();

            ContactAnnotationList annotationList;
            if (annotationLists.Count == 0)
            {
                annotationList = await annotationStore.CreateAnnotationListAsync();
            }
            else
            {
                annotationList = annotationLists.First();
            }

            return annotationList;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialInfoSampleApp.BackgroundAgent.Core.Feeds
{
    /// <summary>
    /// The types of feeds that are saved
    /// </summary>
    public enum FeedType
    {
        /// <summary>
        /// The Home feed is the feed that is seen from the What's New tab in the People app.
        /// This will display all of the feeds that are relevant to the primary user of the
        /// device.
        /// </summary>
        Home,

        /// <summary>
        /// The Contact feed is a feed specific to a contact. When a contact is selected
        /// from the main contact list in the People app, the contact card will open. On
        /// that page, there is a What's New tab that will show the feeds that are
        /// relevant to that contact.
        /// </summary>
        Contact,

        /// <summary>
        /// The Dashboard feed is the feed that is seen on a contact card of the People app.
        /// This is generally the single item that appears next to the contact picture.
        /// </summary>
        Dashboard
    }
}

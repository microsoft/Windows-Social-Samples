using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace SocialInfoSampleApp.SharedComponents
{
    public sealed class PlatformUtilities
    {
        /// <summary>
        /// Performs a runtime check to determine if the SocialInfo APIs are available
        /// on the current platform.
        /// </summary>
        /// <returns>True if the SocialInfo APIs are available, othwerise false</returns>
        public static bool IsSocialInfoApiAvailable()
        {
            return ApiInformation.IsApiContractPresent(
                     "Windows.ApplicationModel.SocialInfo.SocialInfoContract",
                     1);
        }
    }
}

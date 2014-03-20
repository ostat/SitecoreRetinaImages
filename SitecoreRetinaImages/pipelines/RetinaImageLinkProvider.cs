/*
 * Version: 1.0
 * 
 * Blog: http://sitecorecontextitem.wordpress.com/
 * GitHub: https://github.com/scottmulligan/SitecoreAdaptiveImages
 * Twitter: @scottmulligan
 * 
 * LEGAL:
 * Sitecore Retina Images by Scott Mulligan is licensed under a Creative Commons Attribution 3.0 Unported License.
 */

using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using System;
using System.Web;
using System.Linq;

namespace SitecoreRetinaImages.pipelines
{
    class RetinaImageLinkProvider : MediaProvider
    {
        /// <summary>
        ///     The name of the cookie containing the pixel ratio value
        /// </summary>
        private readonly string _cookieName = Settings.GetSetting("cookieName");

        /// <summary>
        ///     Gets a media URL.
        /// </summary>
        /// <param name="item">The media item.</param>
        /// <returns>
        ///     The media URL.
        /// </returns>
        public override string GetMediaUrl(MediaItem item)
        {
            Assert.ArgumentNotNull(item, "item");
            var mediaUrlOptions = new MediaUrlOptions();
            return GetMediaUrl(item, mediaUrlOptions);
        }

        /// <summary>
        ///     Gets the media URL.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="mediaUrlOptions">The media URL options.</param>
        /// <returns></returns>
        public override string GetMediaUrl(MediaItem item, MediaUrlOptions mediaUrlOptions)
        {
            Assert.ArgumentNotNull(item, "item");
            Assert.ArgumentNotNull(mediaUrlOptions, "mediaUrlOptions");
            double pixelDensity = GetCookiePixelDensity();
            //Return the standard image if any of the following conditions are met
            if (!IsImage(item) || Context.Database == null || Context.Site.Name == "shell" || !Context.PageMode.IsNormal
                || pixelDensity <= 1 || !((Item)item).HasChildren)
                return base.GetMediaUrl(item, mediaUrlOptions);
            //Get the first child of the media item (It should be the retina version of the image)
            Item retinaItem = ((Item)item).Children.FirstOrDefault();
            //Return the standard image if width and height are not set on the retina media items
            if (retinaItem == null || String.IsNullOrEmpty(retinaItem["Width"]) || String.IsNullOrEmpty(retinaItem["Height"]))
                return base.GetMediaUrl(item, mediaUrlOptions);
            int retinaWidth = System.Convert.ToInt32(retinaItem["Width"]);
            int retinaHeight = System.Convert.ToInt32(retinaItem["Height"]);
            if (mediaUrlOptions.Width > 0 && mediaUrlOptions.Height > 0
                && retinaWidth >= (mediaUrlOptions.Width * pixelDensity) && retinaHeight >= (mediaUrlOptions.Height * pixelDensity))
            {
                mediaUrlOptions.Width = System.Convert.ToInt32(mediaUrlOptions.Width * pixelDensity);
                mediaUrlOptions.Height = System.Convert.ToInt32(mediaUrlOptions.Height * pixelDensity);
                //Return the retina version with width and height parameters adjusted accordingly
                return base.GetMediaUrl(retinaItem, mediaUrlOptions);
            }
            //Return the retina version
            return base.GetMediaUrl(retinaItem, mediaUrlOptions);
        }

        /// <summary>
        ///     Determines whether the specified item is an image.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///     <c>true</c> if the specified item is image; otherwise, <c>false</c>.
        /// </returns>
        public bool IsImage(MediaItem item)
        {
            return item.MimeType.ToLower().Contains("image");
        }

        /// <summary>
        ///     Resolutions the cookie is set.
        /// </summary>
        /// <returns></returns>
        public bool IsResolutionCookieSet()
        {
            return HttpContext.Current.Request.Cookies[_cookieName] != null;
        }

        /// <summary>
        ///     Gets the cookie pixel density.
        /// </summary>
        /// <returns></returns>
        public double GetCookiePixelDensity()
        {
            // Double check that the cookie identifying screen resolution is set
            if (!IsResolutionCookieSet()) return 1;
            // Split the cookie into resolution and pixel density ratio
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies[_cookieName];
            if (httpCookie == null) return 1;
            string[] cookieResolution = httpCookie.Value.Split(',');
            // If we were able to get the cookie pixel density ratio
            if (cookieResolution.Length > 1)
            {
                double clientPixelDensity;
                if (double.TryParse(cookieResolution[1], out clientPixelDensity))
                    return clientPixelDensity;
            }
            return 1;
        }
    }
}

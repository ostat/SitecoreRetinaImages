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

using SitecoreRetinaImages.pipelines;

namespace SitecoreRetinaImages.hooks
{
    class RetinaImagesMediaProviderHook : Sitecore.Events.Hooks.IHook
    {
        public void Initialize()
        {
            //Sitecore.Diagnostics.Log.Info("Initalizing RetinaImagesMediaProviderHook", this);
            Sitecore.Resources.Media.MediaManager.Provider = new RetinaImageLinkProvider();
            //Sitecore.Diagnostics.Log.Info("RetinaImagesMediaProviderHook initialized", this);
        }
    }
}

Sitecore Retina Images
======================

<b>The Responsive Device Resolver module allows you to create rules that change the Context Device based on a user's detected screen resolution. This is an alternative approach that allows you to use Sitecore Devices without relying on device detection and external User Agent DB services that require subscription fees.</b>

Accompanying blog post: http://sitecorecontextitem.wordpress.com/2014/03/12/sitecore-retina-images/

<b>Installation</b>

After installing the module package you will also need to add the following JS snippet to the head section of your main layout file(s). This snippet sets the browser cookie that stores the device’s detected pixel density.

<pre>
<script>document.cookie = '<%= Sitecore.Configuration.Settings.GetSetting("cookieName") %>=' + Math.max(screen.width, screen.height) + ("devicePixelRatio" in window ? "," + devicePixelRatio : ",1") + '; path=/';</script>
</pre>

<b>Usage</b>

A new insert option is added for each image in the media library that allows you to upload an additional retina version that is stored directly below the standard image. It’s completely optional to upload a retina version so you don’t need to add a second version for every image if you don’t want to.

When linking to an image in the media library, you will still link to the standard version of the image. When rendering a page, Sitecore will check against a cookie value that stores the device’s pixel density and decide whether or not to serve the retina version. An important thing to note is that a javaScript snippet is used to set the pixel density cookie so the first page load will not have access to the cookie and will serve the standard image. All subsequent page requests will have access to the cookie and serve appropriate images. 

Retina versions will be displayed for images that are added into rich text fields or output via the sc:Image or sc:FieldRenderer tag.

<b>Additional Info</b>

I thought about retina images in Sitecore for a while and finally decided upon this set up for a variety of reasons. First off, I wanted this to be something that adds functionality but doesn’t require much additional work to maintain. Secondly, I wanted to make sure that standard screens are served standard images. In no scenario should a high resolution image be served to a standard screen since retina images are substantially larger. In most cases a retina image is ~2.5 to 3 times larger in file size than its standard counterpart. Initially, I wasn’t sure how to handle the retina images in the media library but I decided that adding a retina image version should be an optional extra step. This way you don’t have to add one in cases where it’s not necessary or you don’t have a high-res version.

I also thought about hooking into the Mobile Device Detector module (https://marketplace.sitecore.net/Modules/Mobile_Device_Detector.aspx) to determine the device's pixel density but that information does not seem to available with the free subscription and I do not have access to a paid subscription to do more investigation to see if this is possible with 51degrees.com.
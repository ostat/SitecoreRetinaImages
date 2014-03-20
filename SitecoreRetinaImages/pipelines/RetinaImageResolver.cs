using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using HtmlAgilityPack;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Pipelines.RenderField;
using Sitecore.Xml;

namespace SitecoreRetinaImages.pipelines
{
    class RetinaImageResolver
    {

        /// The regular expression used to identify image media in the rich-text editor
        private readonly Regex _regex = new Regex("/([A-Fa-f0-9]{32,32})\\.ashx");

        public void Process(RenderFieldArgs args)
        {
            if (args.FieldTypeKey != "rich text" ||
                (String.IsNullOrEmpty(args.Result.FirstPart) && String.IsNullOrEmpty(args.Result.LastPart)))
                return;

            args.Result.FirstPart = ReplaceImage(args.Result.FirstPart);
            args.Result.LastPart = ReplaceImage(args.Result.LastPart);
        }

        /// <summary>
        /// Replaces the image with a retina image if it exists.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public string ReplaceImage(string value)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(value);
            HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes("//img[@src]");
            if (nodes == null || nodes.Count == 0) return value;

            foreach (HtmlNode node in nodes)
            {
                string src = node.GetAttributeValue("src", String.Empty);
                if (src == null) continue;
                Match match = _regex.Match(src);
                if (!match.Success) continue;
                ID guid = ID.Parse(match.Groups[1].Value);
                Item mediaItem = Context.Database.GetItem(guid);
                if (mediaItem == null || mediaItem.Children.Count != 1) continue;
                Item retinaImageItem = mediaItem.Children.FirstOrDefault();
                if (retinaImageItem == null) continue;
                value = value.Replace(mediaItem.ID.ToString().ToLower(), retinaImageItem.ID.ToString().ToLower());
            }
            return value;
        }

    }
}

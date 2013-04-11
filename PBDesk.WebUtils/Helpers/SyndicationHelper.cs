using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace PBDesk.WebUtils.Helpers
{
    public class SyndicationHelper
    {
        public static SyndicationFeed LoadFeedFromUrl(string url)
        {
            SyndicationFeed feed = null;
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    using (XmlReader reader = CustomXmlReader.Create(url))
                    {
                        feed = SyndicationFeed.Load(reader);
                        reader.Close();
                    }
                }
                catch
                {
                    feed = null;
                }
            }
            return feed;
        }
    }
}

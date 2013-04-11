using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace PBDesk.WebUtils.Helpers
{
    public class CustomXmlReader : XmlTextReader
    {
        readonly string[] Rss20DateTimeHints = { "pubDate" };
        readonly string[] Atom10DateTimeHints = { "updated", "published", "lastBuildDate" };
        readonly string[] LocaleHints = { "language" };


        private bool isRss2DateTime = false;
        private bool isAtomDateTime = false;
        private bool isLocale = false;

        CultureInfo currentCultureInfo = CultureInfo.CurrentCulture;


        public CustomXmlReader(Stream stream) : base(stream) { }

        public override bool IsStartElement(string localname, string ns)
        {

            isRss2DateTime = false;
            isAtomDateTime = false;

            if (Rss20DateTimeHints.FirstOrDefault(x => x == localname) != null)
                isRss2DateTime = true;
            else if (Atom10DateTimeHints.FirstOrDefault(x => x == localname) != null)
                isAtomDateTime = true;
            else if (LocaleHints.FirstOrDefault(x => x == localname) != null)
                isLocale = true;

            return base.IsStartElement(localname, ns);

        }

        public override string ReadString()
        {
            string value = base.ReadString();

            if (isLocale)
            {
                isLocale = false;

                try
                {
                    currentCultureInfo = new CultureInfo(value);
                }
                catch
                {
                    // Nothing critical.
                }

                return value;
            }

            try
            {
                // Determine wether were about to parse a dateTime and if so, try to parse it ourself
                // This way, we can make sure the datetime is valid before the actual parser parsers the datetime.
                // Note that for this implementation i just use the actual method that is going to parse the datetime,
                // This ofcourse should be made illegal and therefore i strongly suggest you make your own exceptional implementation
                if (isRss2DateTime)
                {
                    MethodInfo objMethod = typeof(Rss20FeedFormatter).GetMethod("DateFromString", BindingFlags.NonPublic | BindingFlags.Static);
                    objMethod.Invoke(null, new object[] { value, this });
                }
                else if (isAtomDateTime)
                {
                    MethodInfo objMethod = typeof(Atom10FeedFormatter).GetMethod("DateFromString", BindingFlags.NonPublic | BindingFlags.Instance);
                    objMethod.Invoke(new Atom10FeedFormatter(), new object[] { value, this });
                }

                return value;
            }
            catch (TargetInvocationException)
            {
                // This is most likely to happen when datefromstring method fails 
                // which tells us the specific DateTime is not conform to the specifications
                // We should try and parse the datetime ourself

                // Failed dates from MSDN magazine feed
                // 10/26/2011 00:00:00 GMT
                // 5/2/2011 00:00:00 GMT
                // Mon, 14 Apr 2008 13:17:18 GMT 00:00:00 GMT
                string[] formats = 
                                {
                                    "M/d/yyyy hh:mm:ss GMT", 
                                    "ddd, dd MMM yyyy HH:mm:ss GMT 00:00:00 GMT", 
                                    "yyyy-MM-ddTHH:mm:ssZ"
                                };

                const string syndicationTimeFormat = "R";
                DateTimeOffset dateTimeOffset;
                if (DateTimeOffset.TryParseExact(value, formats, currentCultureInfo, System.Globalization.DateTimeStyles.None, out dateTimeOffset))
                    return dateTimeOffset.ToString(syndicationTimeFormat);
                else
                {
                    // We can't fail reading. Better we return incorrect date
                    return DateTimeOffset.UtcNow.ToString(syndicationTimeFormat);
                }
            }

        }

        public static XmlReader Create(string url)
        {
            WebClient client = new WebClient();
            return new CustomXmlReader(client.OpenRead(url));

        }
        //https://connect.microsoft.com/VisualStudio/feedback/details/325421/syndicationfeed-load-fails-to-parse-datetime-against-a-real-world-feeds-ie7-can-read
    } 
}

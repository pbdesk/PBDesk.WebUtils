using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace PBDesk.WebUtils.UrlShortner
{
    public static class UrlShortner
    {
        private static GoogleUrlApi Api
        {
            get
            {
                try
                {
                    return new GoogleUrlApi(ConfigurationManager.AppSettings["GoogleAPIKey"]);
                }
                catch
                {
                    return null;
                }
            }
        }
        public static string Shorten(string longUrl)
        {
            if (Api != null && !string.IsNullOrWhiteSpace(Api.GoogleApiKey))
            {
                GoogleUrlReply reply = Api.Shorten(longUrl);
                if (reply != null && reply.id != null)
                {
                    if (reply.status != null)
                    {
                        if (reply.status != "OK")
                        {
                            return null;
                        }
                    }
                    return reply.id;
                }
            }
            return null;
        }

        public static string Expand(string shortUrl)
        {
            if (Api != null && !string.IsNullOrWhiteSpace(Api.GoogleApiKey))
            {
                GoogleUrlReply reply = Api.Expand(shortUrl);
                if (reply != null && reply.id != null)
                {
                    if (reply.status != null)
                    {
                        if (reply.status != "OK")
                        {
                            return null;
                        }
                    }
                    return reply.longUrl;
                }
            }
            return null;

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;


namespace PBDesk.WebUtils.UrlShortner
{
    public class GoogleUrlApi
    {
        #region Constants
        private const string GoogleUrlShortningServiceUrl = "https://www.googleapis.com/urlshortener/v1/url";
        #endregion

        #region Properties
        public string GoogleApiKey { get; set; }
        #endregion

        #region Constructors
        public GoogleUrlApi()
        {
            GoogleApiKey = string.Empty;
        }

        public GoogleUrlApi(string googleApiKey)
        {
            GoogleApiKey = googleApiKey;
        }
        #endregion

        #region Public Methods

        public GoogleUrlReply Shorten(string longUrl)
        {
            GoogleUrlReply reply = null;
            if (!string.IsNullOrEmpty(longUrl))
            {
                string data =  "{\"longUrl\":\"" + longUrl + "\"}";
                string requestUrl = GoogleUrlShortningServiceUrl;
                if (!string.IsNullOrEmpty(GoogleApiKey))
                {
                    requestUrl += "?key=" + GoogleApiKey;
                }
                string response = Post(requestUrl, data);
                if (!string.IsNullOrEmpty(response))
                {
                    reply = JsonConvert.DeserializeObject<GoogleUrlReply>(response);
                }                
            }
            return reply;
        }

        public GoogleUrlReply Expand(string shortUrl)
        {
            GoogleUrlReply reply = null;
            if (!string.IsNullOrEmpty(shortUrl))
            {
                string requestUrl = GoogleUrlShortningServiceUrl + "?shortUrl=" + shortUrl;

                if (!string.IsNullOrEmpty(GoogleApiKey))
                {
                    requestUrl += "&key=" + GoogleApiKey;
                }
                string response = Get(requestUrl);
                if (!string.IsNullOrEmpty(response))
                {
                    reply = JsonConvert.DeserializeObject<GoogleUrlReply>(response);
                }  

            }
            return reply;

        }
        #endregion

        #region Private Support Methods
        private string Get(string url)
        {
            using (WebClient web = new WebClient())
            {
                return web.DownloadString(url);
            }
        }

        private string Post(string url, string data)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";

            byte[] byteData = Encoding.UTF8.GetBytes(data);

            request.ContentLength = byteData.Length;

            using (Stream s = request.GetRequestStream())
            {
                s.Write(byteData, 0, byteData.Length);
                s.Close();
            }

            string replyData;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        replyData = reader.ReadToEnd();
                    }
                }
            }

            return replyData;
        }
        #endregion
    }
}

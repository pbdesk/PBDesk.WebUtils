using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PBDesk.WebUtils.Helpers;

namespace PBDesk.WebUtils.MediaTypeFormatters
{
    public class JsonNetMediaTypeFormatter : JsonMediaTypeFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNetMediaTypeFormatter"/> class.
        /// </summary>
        public JsonNetMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" });
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/json") { CharSet = "utf-8" });

        }



        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, System.Net.Http.HttpContent content, IFormatterLogger formatterLogger)
        {
            var task = Task<object>.Factory.StartNew(() =>
            {
                var ser = JsonNetExtensions.GetJsonSerializer();

                using (var sr = new StreamReader(readStream))
                {
                    using (var jreader = new JsonTextReader(sr))
                    {
                        ser.Converters.Add(new IsoDateTimeConverter());
                        return ser.Deserialize(jreader, type);
                    }
                }
                /*
                var sr = new StreamReader(stream);
                var jreader = new JsonTextReader(sr);

                var ser = new JsonSerializer();
                ser.Converters.Add(new IsoDateTimeConverter());

                object val = ser.Deserialize(jreader, type);
                return val;*/
            });

            return task;
        }


        //public override System.Threading.Tasks.Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContentHeaders contentHeaders, IFormatterLogger formatterLogger)
        //{
        //    var task = Task<object>.Factory.StartNew(() =>
        //    {
        //        var ser = JsonNetExtensions.GetJsonSerializer();

        //        using (var sr = new StreamReader(stream))
        //        {
        //            using (var jreader = new JsonTextReader(sr))
        //            {
        //                ser.Converters.Add(new IsoDateTimeConverter());
        //                return ser.Deserialize(jreader, type);
        //            }
        //        }
        //        /*
        //        var sr = new StreamReader(stream);
        //        var jreader = new JsonTextReader(sr);

        //        var ser = new JsonSerializer();
        //        ser.Converters.Add(new IsoDateTimeConverter());

        //        object val = ser.Deserialize(jreader, type);
        //        return val;*/
        //    });

        //    return task;
        //}


        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, TransportContext transportContext)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var settings = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                };

                string json = JsonConvert.SerializeObject(value, Formatting.Indented,
                                                          new JsonConverter[1] { new IsoDateTimeConverter() });

                byte[] buf = System.Text.Encoding.Default.GetBytes(json);
                writeStream.Write(buf, 0, buf.Length);
                writeStream.Flush();
            });

            return task;
        }
        //public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext transportContext)
        //{
        //    var task = Task.Factory.StartNew(() =>
        //    {
        //        var settings = new JsonSerializerSettings()
        //        {
        //            NullValueHandling = NullValueHandling.Ignore,
        //        };

        //        string json = JsonConvert.SerializeObject(value, Formatting.Indented,
        //                                                  new JsonConverter[1] { new IsoDateTimeConverter() });

        //        byte[] buf = System.Text.Encoding.Default.GetBytes(json);
        //        stream.Write(buf, 0, buf.Length);
        //        stream.Flush();
        //    });

        //    return task;
        //}

    }
}

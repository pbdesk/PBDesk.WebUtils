using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PBDesk.WebUtils.Helpers
{
    public static class JsonNetExtensions
    {
        /// <summary>
        /// Deserializes the JSON-serialized stream and casts it to the object type specified.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <param name="stream">The stream.</param>
        /// <returns>The deserialized object as type <typeparamref name="T"/></returns>
        public static T ReadAsJson<T>(this Stream stream) where T : class
        {
            return ReadAsJson(stream, typeof(T)) as T;
        }

        /// <summary>
        /// Reads the JSON-serialized stream and deserializes it into a CLR object.
        /// </summary>
        /// <param name="stream">The stream to deserialize.</param>
        /// <param name="instanceType">Type of the instance.</param>
        /// <returns>The deserialized object.</returns>
        /// <remarks></remarks>
        public static Object ReadAsJson(this Stream stream, Type instanceType)
        {
            if (stream == null)
            {
                return null;
            }

            using (var jsonTextReader = new JsonTextReader(new StreamReader(stream)))
            {
                return Deserialize(jsonTextReader, instanceType);
            }
        }

        /// <summary>
        /// Serializes the object into JSON and writes the data into the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="instance">The object instance to serialize.</param>
        public static void WriteAsJson(this Stream stream, Object instance)
        {
            if (instance == null)
            {
                return;
            }

            using (var jsonTextWriter = new JsonTextWriter(new StreamWriter(stream)) { CloseOutput = false })
            {
                Serialize(jsonTextWriter, instance);
            }
        }

        public static JsonSerializer GetJsonSerializer()
        {
            return new JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,


            };
        }

        private static Object Deserialize(JsonReader jsonReader, Type instanceType)
        {
            try
            {
                using (jsonReader)
                {
                    var jsonSerializer = GetJsonSerializer();

                    return jsonSerializer.Deserialize(jsonReader, instanceType);
                }
            }
            catch (JsonReaderException)
            {
                // TODO: (DG) Internal logging?...
                jsonReader.Close();
                throw;
            }
            catch (JsonSerializationException)
            {
                // TODO: (DG) Internal logging?...
                jsonReader.Close();
                throw;
            }
        }

        private static void Serialize(JsonWriter jsonWriter, Object instance)
        {
            try
            {
                var jsonSerializer = GetJsonSerializer();

                jsonSerializer.Serialize(jsonWriter, instance);
                jsonWriter.Flush();
            }
            catch (JsonWriterException)
            {
                // TODO: (DG) Internal logging?...
                jsonWriter.Close();
                throw;
            }
            catch (JsonSerializationException)
            {
                // TODO: (DG) Internal logging?...
                jsonWriter.Close();
                throw;
            }
        }
    }
}

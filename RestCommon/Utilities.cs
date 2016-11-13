//-----------------------------------------------------------------------
// <summary>
//      Utilities class.
// </summary>
//-----------------------------------------------------------------------

using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;

using Newtonsoft.Json;

namespace Rest.Common
{

    public class Utilities
    {

        /// <summary>
        /// Convert an object instance to json string using .NET DataContractJsonSerializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns><see cref="string"/>Json</returns>
        public string SerializeToJsonWithDataContractJsonSerializer<T>(T t)
        {
            var ser = new DataContractJsonSerializer(typeof(T));

            using (var ms = new MemoryStream())
            {
                ser.WriteObject(ms, t);

                ms.Position = 0;
                using (var sr = new StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }


        /// <summary>
        /// Convert json to an object instance using .NET DataContractJsonSerializer.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="t">Instance of any type.</param>
        /// <param name="json"></param>
        /// <param name="encoding">Encoding</param>
        /// <returns>T - Passed in type.</returns>
        public T DeserializeToObjectWithDataContractJsonSerializer<T>(T t, string json, Encoding encoding)
        {
            var ser = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream(encoding.GetBytes(json)))
            {
                return (T)ser.ReadObject(ms);
            }
        }


        /// <summary>
        /// Convert an object instance to json string using Newtonsoft.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="t">Instance of any type.</param>
        /// <returns><see cref="string"/>Json</returns>
        public string SerializeToJson<T>(T t)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                // For not serializing any properties that are null.
                NullValueHandling = NullValueHandling.Ignore

                // Ignore any missing members.
                //MissingMemberHandling = MissingMemberHandling.Ignore
            };

           

            var jsonSerializer = JsonSerializer.Create(jsonSerializerSettings);
            

            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = jsonSerializerSettings.Formatting;
                    //jsonTextWriter.WriteStartObject();
                    //jsonTextWriter.WritePropertyName(t.GetType().Name);
                    jsonSerializer.Serialize(jsonTextWriter, t);
                    //jsonTextWriter.WriteEndObject();
                }

                return stringWriter.ToString();
            }
        }


        /// <summary>
        /// Convert json to an object instance using Newtonsoft.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="json"></param>
        /// <param name="encoding">Encoding</param>
        /// <returns>T - Passed in type.</returns>
        public T DeserializeToObject<T>(string json, Encoding encoding)
        {
            var jsonSerializerSettings = new JsonSerializerSettings();
            

            // For simple deserialization use:
            //return JsonConvert.DeserializeObject<T>(json);

            var jsonSerializer = JsonSerializer.Create(jsonSerializerSettings);
            using (var reader = new JsonTextReader(new StringReader(json)))
            {
                return (T)jsonSerializer.Deserialize(reader, typeof(T));
            }
        }




    }
}

//-----------------------------------------------------------------------
// <summary>
//      Request class.
// </summary>
//-----------------------------------------------------------------------

using System;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;


namespace Rest.Client
{
    using Common;
    using Common.Logging;

    public class Request : IRequest
    {
        public string ContentType { get; set; }

        public string Data { get; set; }

        public HttpMethod WebMethod { get; set; }

        public int Timeout { get; set; }

        public byte MaxRetries { get; set; }
        public ushort RetrySleepTime { get; set; }

        public Encoding Encoding { get; set; }

        public ILog Log { get; set; }

        public Dictionary<string, string> Headers { get; set; }


        /// <summary>
        /// Handles the configuration of the request.
        /// </summary>
        /// <param name="webMethod">HttpMethod</param>
        /// <param name="contentType">string -  content-type such as 'application/json'</param>
        /// <param name="data">string - Serialized string.</param>
        /// <param name="encoding">Encoding</param>
        /// <param name="timeout">int</param>
        /// <param name="maxRetries">byte</param>
        /// <param name="retrySleepTime">short</param>
        public Request(HttpMethod webMethod, string contentType, string data, Encoding encoding, int timeout = 30, byte maxRetries = 5)
        {
            if (webMethod == null)
            {
                throw new ArgumentException("WebMethod is not defined.");
            }

            if (webMethod != HttpMethod.Get)
            {
                if (string.IsNullOrWhiteSpace(contentType))
                {
                    throw new ArgumentException("ContentType is not defined.");
                }

                if (encoding == null)
                {
                    throw new ArgumentException("Encoding is not defined.");
                }
            }

            if (timeout < 0)
            {
                throw new ArgumentException("Timeout must be zero or larger.");
            }

            WebMethod = webMethod;
            ContentType = contentType;
            Data = data;
            Timeout = timeout;
            MaxRetries = maxRetries;
            Encoding = encoding;

            Log = new Log();
        }


        public bool TrySerializeToJson<T>(T t, out string json)
        {
            json = string.Empty;

            try
            {
                var utilities = new Utilities();
                json = utilities.SerializeToJson(t);
                return true;
            }
            catch (Exception ex)
            {
                Log.ErrorException = ex;
            }

            return false;
        }


        public void SerializeToJson<T>(T t)
        {
            var utilities = new Utilities();
            this.Data = utilities.SerializeToJson(t);
        }

    }
}


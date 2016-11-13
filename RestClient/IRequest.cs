//-----------------------------------------------------------------------
// <summary>
//      Request interface.
// </summary>
//-----------------------------------------------------------------------

using System.Text;
using System.Net.Http;
using System.Collections.Generic;

namespace Rest.Client
{
    using Common.Logging;

    public interface IRequest
    {
        /// <summary>
        ///  WebMethod or verb - System.Net.Http.HttpMethod.
        /// </summary>
        HttpMethod WebMethod { get; set; }


        /// <summary>
        /// "text/plain;charset=utf-8"
        /// </summary>
        string ContentType { get; set; }

        
        /// <summary>
        /// The data to be sent over http.
        /// </summary>
        string Data { get; set; }


        /// <summary>
        /// The timeout for the request.
        /// </summary>
        int Timeout { get; set; }

        /// <summary>
        /// The maximum time to retry the request in milliseconds.
        /// </summary>
        byte MaxRetries { get; set; }
        /// <summary>
        /// Encoding type of the request.
        /// </summary>
        Encoding Encoding { get; set; }


        /// <summary>
        /// Log object containing exceptions, errors, warnings, and information.
        /// </summary>
        ILog Log { get; set; }

        /// <summary>
        /// The collection of headers to send with the request.
        /// </summary>
        Dictionary<string, string> Headers { get; set; }


        /// <summary>
        /// Attempt to Serialize given object to json.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="t">Generic object</param>
        /// <param name="json">string - output parameter</param>
        /// <returns><see cref="bool"/></returns>
        bool TrySerializeToJson<T>(T t, out string json);


        /// <summary>
        /// Serialize the given object into the the this.Data.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="t">Generic object</param>
        void SerializeToJson<T>(T t);
    }
}

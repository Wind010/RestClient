//-----------------------------------------------------------------------
// <summary>
//      Response interface.
// </summary>
//-----------------------------------------------------------------------


using System.Text;
using System.Net;

namespace Rest.Client
{
    using Common.Logging;

    public interface IResponse
    {

        // The raw response from reading the http response stream.
        string RawResponse { get; set; }


        /// <summary>
        /// WebResponse ContentEncoding.
        /// </summary>
        Encoding Encoding { get; set; }

        /// <summary>
        /// Content-Type
        /// </summary>
        string ContentType { get; set; }


        /// <summary>
        /// Exceptions thrown during the request, if any.  
        /// </summary>
        /// <remarks>Will contain only network transport or framework exceptions thrown during the request.
        /// HTTP protocol errors are handled by RestSharp and will not appear here.</remarks>
        ILog Log { get; set; }


        /// <summary>
        /// Status of the request. 
        /// </summary>
        HttpStatusCode StatusCode { get; set; }


        string StatusDescription { get; set; }


        /// <summary>
        /// Attempt to deserialize the RawResponse json string to the passed in type.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="t">Generic object - output parameter</param>
        /// <returns><see cref="bool"/></returns>
        bool TryDeserializeFromJson<T>(out T t);
    }
}

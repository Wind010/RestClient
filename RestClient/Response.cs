//-----------------------------------------------------------------------
// <summary>
//      Response class.
// </summary>
//-----------------------------------------------------------------------

using System;
using System.Text;
using System.Net;

namespace Rest.Client
{
    using Common;
    using Common.Logging;

    public class Response : IResponse
    {
        public string RawResponse { get; set; }

        public Encoding Encoding { get; set; }

        public string ContentType { get; set; }
        
        public ILog Log { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string StatusDescription { get; set; }

     
        public Response()
        {
            Log = new Log();
        }


        public bool TryDeserializeFromJson<T>(out T t)
        {
            t = default(T);

            try
            {
                var utilities = new Utilities();
                t = utilities.DeserializeToObject<T>(RawResponse, Encoding);
                return true;
            }
            catch (Exception ex)
            {
                Log.ErrorException = ex;
            }

            return false;
        }



    }
}

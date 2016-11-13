//-----------------------------------------------------------------------
// <summary>
//      Client class.
// </summary>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;


using System.Net;
using System.Net.Http;


namespace Rest.Client
{
    using Common.Logging;


    public class Client : IClient
    {
        private string _fullUri = string.Empty;

        public string BaseUri { get; set; }

        public string UriParameters { get; set; }

        public Request Request { get; set; }

        public string FullUri
        {
            get { return this._fullUri; }
        }

        public ILog Log { get; set; }

        public Action ProgressDlgShow { get; set; }

        public Action ProgressDlgHide { get; set; }

        private HttpWebRequest _httpRequest;


        #region Constructors


        static Client()
        {
            // Ensures that any HTTPS requests are issued with PA-DSS compliant security
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        public Client(string baseUri)
        {
            if (string.IsNullOrEmpty(baseUri))
            {
                throw new ArgumentNullException("baseUri");
            }

            BaseUri = baseUri;

            Log = new Log();
        }


        public Client(string baseUri, string uriParameters)
        {
            if (string.IsNullOrEmpty(baseUri))
            {
                throw new ArgumentNullException("baseUri");
            }

            if (string.IsNullOrEmpty(baseUri))
            {
                throw new ArgumentNullException("uriParameters");
            }

            BaseUri = baseUri;
            UriParameters = uriParameters;

            Log = new Log();
        }


        public Client(string baseUri, string uriParameters, IRequest request)
        {
            if (string.IsNullOrEmpty(baseUri))
            {
                throw new ArgumentNullException("baseUri");
            }

            if (string.IsNullOrEmpty(uriParameters))
            {
                throw new ArgumentNullException("uriParameters");
            }

            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            BaseUri = baseUri;
            UriParameters = uriParameters;

            Request = (Request)request;
            Log = new Log();
        }


        #endregion Constructors



        /// <summary>
        /// Send http request to the combined BaseUri and UriParameters.
        /// Check the Log, Request.Log, and Response.Log properties for 
        /// errors, warnings, and information.  
        /// </summary>
        /// <param name="request"></param>
        /// <returns><see cref="IResponse"/> </returns>
        public IResponse Send(IRequest request)
        {
            IResponse response = new Response();

            try
            {
                this.SendActual(request);
            }
            catch (WebException webEx)
            {
                response = (Response)this.ProcessWebException(webEx, request, response);
            }
            // Other possible exceptions are ProtocolViolationExceptions (network) and InvalidOperationExceptions?

            return response;
        }


        /// <summary>
        /// Send http request to the combined BaseUri and UriParameters asynchronously.
        /// This is the method you want to use if your application has a UI.
        /// The requests will retry depending on the request configuration.  
        /// Check the Log, Request.Log, and Response.Log properties for errors, warnings, and information.
        /// The await exists within this call.
        /// </summary>
        /// <param name="request">IRequest</param>
        /// <returns><see cref="Task{IResponse}"/> </returns>
        public async Task<IResponse> SendAsyncAwait(IRequest request)
        {
            IResponse iResponse = null;

            iResponse = await Task.Run(() => this.Send(request)).ConfigureAwait(continueOnCapturedContext: false);

            return iResponse;
        }


        /// <summary>
        /// Send http request to the combined BaseUri and UriParameters asynchronously.
        /// This is the method you want to use if your application has a UI.
        /// The requests will retry depending on the request configuration.  
        /// Check the Log, Request.Log, and Response.Log properties for errors, warnings, and information.
        /// The await exists within this call.
        /// </summary>
        /// <param name="request">IRequest</param>
        /// <returns><see cref="IResponse"/> </returns>
        public IResponse SendAsyncTpl(IRequest request)
        {
            IResponse iResponse = null;
            Task.Run(() => this.Send(request)).ContinueWith((t) =>
            {
                if (this.ProgressDlgHide != null && System.Environment.UserInteractive)
                {
                    this.ProgressDlgHide();  // Continue back on the UI thread.
                }

                iResponse = t.Result;
            }
            , TaskScheduler.FromCurrentSynchronizationContext());

            if (this.ProgressDlgShow != null && System.Environment.UserInteractive)
            {
                this.ProgressDlgShow();  // This is what causes the UI thread to pause, but stay responsive.
            }

            return iResponse;
        }


        /// <summary>
        /// Send http request to the combined BaseUri and UriParameters.
        /// </summary>
        /// <returns><see cref="IResponse"/> </returns>
        private IResponse SendActual(IRequest request)
        {
            var response = new Response();

            if (request == null)
            {
                throw new ArgumentNullException("request", "Request is not defined.");
            }

            if (request.WebMethod == null)
            {
                throw new ArgumentNullException("request", "Request.WebMethod is not defined.");
            }

            Request = (Request)request;

            this._fullUri = GetFullUri();
            _httpRequest = (HttpWebRequest)WebRequest.Create(this._fullUri);

            _httpRequest.Method = request.WebMethod.ToString();
            _httpRequest.ContentType = request.ContentType;
            _httpRequest.Timeout = request.Timeout;
            _httpRequest = AddHeaders(Request, _httpRequest);

            if (!string.IsNullOrWhiteSpace(request.Data) && request.WebMethod == HttpMethod.Post)
            {
                var encoding = request.Encoding;
                byte[] bytes = encoding.GetBytes(request.Data);
                _httpRequest.ContentLength = bytes.Length;

                using (Stream requestStream = _httpRequest.GetRequestStream())
                {
                    // Send the data.
                    requestStream.Write(bytes, 0, bytes.Length);
                }
            }

            response = (Response)GetResponse(_httpRequest);

            _httpRequest = null;

            return response;
        }

        public void AbortRequest()
        {
            if (_httpRequest != null)
            {
                _httpRequest.Abort();
            }
        }

        /// <summary>
        /// Add <see cref="IRequest"/> headers to the <see cref="HttpWebRequest"/> headers.  
        /// </summary>
        /// <param name="request">IRequest</param>
        /// <param name="httpRequest">HttpWebRequest</param>
        /// <returns> <see cref="HttpWebRequest"/> </returns>
        private HttpWebRequest AddHeaders(IRequest request, HttpWebRequest httpRequest)
        {
            if ((request.Headers != null) && (request.Headers.Count > 0))
            {
                foreach (var header in request.Headers)
                {
                    // Don't add if the key is not valid.
                    if (!string.IsNullOrWhiteSpace(header.Key))
                    {
                        // If we don't have a value, just add the key.
                        if (string.IsNullOrWhiteSpace(header.Value))
                        {
                            httpRequest.Headers.Add(header.Key);
                        }
                        else
                        {
                            httpRequest.Headers.Add(header.Key, header.Value);
                        }
                    }
                }
            }

            return httpRequest;
        }



        /// <summary>
        /// Get response from the HttpWebRequest.
        /// </summary>
        /// <param name="httpWebRequest">HttpWebRequest</param>
        /// <returns><see cref="IResponse"/> </returns>
        public IResponse GetResponse(HttpWebRequest httpWebRequest)
        {
            using (var webResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                var response = new Response
                {
                    StatusCode = webResponse.StatusCode,
                    StatusDescription = webResponse.StatusDescription,
                    Encoding =
                        !string.IsNullOrEmpty(webResponse.ContentEncoding)
                            ? Encoding.GetEncoding(webResponse.ContentEncoding)
                            : Encoding.UTF8,
                    ContentType = webResponse.ContentType
                };

                if (webResponse.StatusCode != HttpStatusCode.OK)
                {
                    var message = string.Format("Request failed. Received HTTP {0}", webResponse.StatusCode);
                    response.StatusCode = webResponse.StatusCode;
                    response.Log.Errors.Add(message);
                    return response;
                }

                // Grab the response
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        return response;
                    }

                    // Read the stream and save string with same encoding as request.
                    using (var reader = new StreamReader(responseStream, this.Request.Encoding))
                    {
                        // We may use the raw response or we may not.  The consideration here is memory.
                        // De-serialization from the stream is more memory efficient.
                        response.RawResponse = reader.ReadToEnd();
                    }
                }

                return response;
            }
        }


        /// <summary>
        /// Combine the BaseUri with UriParameters with '/' delimiter.
        /// </summary>
        /// <returns>string</returns>
        private string GetFullUri()
        {
            if (string.IsNullOrWhiteSpace(BaseUri))
            {
                throw new ArgumentException("BaseUri is not set.");
            }

            if (string.IsNullOrWhiteSpace(UriParameters))
            {
                throw new ArgumentException("UriParameters is not set.");
            }

            const string Delim = "/";
            var sb = new StringBuilder(BaseUri);
            if (!BaseUri.EndsWith(Delim))
            {
                sb.Append(Delim);
            }

            sb.Append(UriParameters);

            return sb.ToString();
        }


        /// <summary>
        /// Process the WebException by capturing the httpResponse of the exception into the Response object.
        /// Increments retry count or sets retry count to maximum.
        /// </summary>
        /// <param name="webEx">WebException</param>
        /// <param name="request">IRequest</param>
        /// <param name="response">IResponse</param>
        /// <returns><see cref="IResponse"/> </returns>
        private IResponse ProcessWebException(WebException webEx, IRequest request, IResponse response)
        {
            if (webEx == null)
            {
                return response;
            }

            if (request == null)
            {
                throw new ArgumentException("Request is null when it really shouldn't be.");
            }

            if (response == null)
            {
                throw new ArgumentException("Response is null when it really shouldn't be.");
            }

            if (webEx.Response == null)
            {
                // WebExeption without a Response object indicates possible 'Not Found' (404) or timeout.
                response.Log.ErrorException = webEx;
            }
            else
            {
                var httpResponse = (HttpWebResponse)webEx.Response;
                response = this.PopulateResponse(webEx, httpResponse, request, response);
            }

            return response;
        }


        /// <summary>
        /// Populate the response object with exception details and the response.
        /// </summary>
        /// <param name="webEx">WebException</param>
        /// <param name="httpResponse">HttpWebResponse</param>
        /// <param name="request">IRequest</param>
        /// <param name="response">IResponse</param>
        /// <returns><see cref="IResponse"/></returns>
        private IResponse PopulateResponse(WebException webEx, HttpWebResponse httpResponse, IRequest request, IResponse response)
        {
            // Populate the response object.

            response.Log.ErrorException = webEx;

            response.StatusCode = httpResponse.StatusCode;
            response.StatusDescription = httpResponse.StatusDescription;
            response.ContentType = httpResponse.ContentType;

            if (string.IsNullOrEmpty(httpResponse.ContentEncoding) && request.Encoding != null)
            {
                response.Encoding = request.Encoding;
            }
            else
            {
                response.Encoding = Encoding.GetEncoding(httpResponse.ContentEncoding);
            }

            if (httpResponse.ContentLength > 0)
            {
                using (var exceptionResponse = new StreamReader(httpResponse.GetResponseStream()))
                {
                    response.RawResponse = exceptionResponse.ReadToEnd();
                }
            }

            return response;
        }

    }
}

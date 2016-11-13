//-----------------------------------------------------------------------
// <summary>
//      Integration test for RestClient with a combination of real 
//      endpoint and shims.
// </summary>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Configuration;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.QualityTools.Testing.Fakes;

namespace Rest.Client.Tests
{
    using System.Net;
    using System.Net.Http;
    using System.Runtime.CompilerServices;

    using Rest.Client;

    using System.Net.Fakes;
    
    using Rest.Client.Fakes;



    [TestClass]
    public class ClientTests
    {
        private static string baseUri;

        private const byte FirstRetrySuccessCount = 1;
        private const byte RetryCount = 1;

        private const short RetryCountStart = -1;

        public void ClassInitialize()
        {
            
        }


        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            baseUri = ConfigurationManager.AppSettings["TestBaseUri"];

            // Use for Fiddler.
            //_baseUri = "http://localhost.fiddler:52101/RestHostLib/Service1";
        }


        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestMethod]
        [Ignore]  // Requires real endpoint.  For use with manual test runs.
        public void Send_TestGet_Successful()
        {
            var request = new Request(HttpMethod.Get, "application/json", string.Empty, Encoding.UTF8, 30);
            const string Parameters = "TestGet";

            var client = new Client(baseUri, Parameters, request);
            IResponse response = client.Send(request);

            AssertSuccessfulHttpTransaction(client, request, response, HttpStatusCode.OK);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.RawResponse));
        }

        
        [TestMethod]
        [Ignore]  // Requires real endpoint.  For use with manual test runs.
        public void Send_TestPost_Successful()
        {
            var request = new Request(HttpMethod.Post, "application/json", "{\"name\":\"Jeff\"}", Encoding.UTF8, 30);

            var client = new Client(baseUri);
            client.Request = request;
            client.UriParameters = "TestPost";

            IResponse response = client.Send(request);

            AssertSuccessfulHttpTransaction(client, request, response, HttpStatusCode.OK);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.RawResponse));
        }


        [TestMethod]
        public void Send_PostShimResponse_Success()
        {
            // Assertions done within.
            SimulateHttpTransaction(WebExceptionStatus.Success, HttpStatusCode.OK, 0, 0);

            // Additional assertions?
        }


        [TestMethod]
        public void Send_PostShimWebExceptionWithRequestTimeout_MaximumRetry()
        {
            // Assertions done within.
            IResponse response = SimulateHttpTransaction(WebExceptionStatus.Timeout, 
                HttpStatusCode.RequestTimeout, successCount: 0, timesToRetry: RetryCount);

            // Additional assertions?
        }

        [TestMethod]
        public void Send_PostShimWebExceptionWithGatewayTimeout_MaximumRetry()
        {
            // Assertions done within.
            SimulateHttpTransaction(WebExceptionStatus.Timeout, 
                HttpStatusCode.GatewayTimeout, successCount: 0, timesToRetry: RetryCount);

            // Additional assertions?
        }

        [TestMethod]
        public void Send_PostShimWebExceptionWithServiceUnavailable_MaximumRetry()
        {
            // Assertions done within.
            SimulateHttpTransaction(WebExceptionStatus.SendFailure, 
                HttpStatusCode.ServiceUnavailable, successCount: 0, timesToRetry: RetryCount);

            // Additional assertions?
        }


        [TestMethod]
        public void Send_PostShimWebExceptionWithRequestTimeout_SuccessOnRetry()
        {
            // Assertions done within.
            SimulateHttpTransaction(WebExceptionStatus.Timeout, 
                HttpStatusCode.RequestTimeout, successCount: 1, timesToRetry: 2);

            // Additional assertions?
        }

        [TestMethod]
        public void Send_PostShimWebExceptionWithGatewayTimeout_SuccessOnRetry()
        {
            // Assertions done within.
            SimulateHttpTransaction(WebExceptionStatus.Timeout,
                HttpStatusCode.GatewayTimeout, successCount: 0, timesToRetry: 2);

            // Additional assertions?
        }

        [TestMethod]
        public void Send_PostShimWebExceptionWithServiceUnavailable_SuccessOnRetry()
        {
            // Assertions done within.
            SimulateHttpTransaction(WebExceptionStatus.SendFailure,
                HttpStatusCode.ServiceUnavailable, successCount: 0, timesToRetry: 2);

            // Additional assertions?
        }

        
        /// <summary>
        /// Simulate a transaction through the Client, Request, and Response classes. 
        /// Transaction can be succeeded or fail based off the HttpStatusCode.
        /// </summary>
        /// <param name="webExceptionStatus"> <see cref="WebExceptionStatus"/> </param>
        /// <param name="httpStatusCode"> <see cref="HttpStatusCode"/> </param>
        /// <param name="successCount">byte - The number of times of retries before the transaction succeeds.</param>
        /// <param name="timesToRetry">short - The number of times the transaction has been retried.</param>
        /// <param name="responseMsg">string - The message that is expected to be in the response.</param>
        /// <returns> <see cref="IResponse"/> </returns>
        private IResponse SimulateHttpTransaction(WebExceptionStatus webExceptionStatus, HttpStatusCode httpStatusCode, 
            byte successCount, byte timesToRetry, [CallerMemberName] string responseMsg = "")
        {
            const string requestMsg = "I sent this.";

            IClient client;
            IRequest request;
            IResponse response;

            short timesRetried = RetryCountStart;

            using (ShimsContext.Create())
            {
                var shimRequest = new ShimHttpWebRequest();
                var shimResponse = new ShimHttpWebResponse();

                ShimWebRequest.CreateString = (uri) => shimRequest.Instance;
                shimRequest.GetRequestStream = () => new MemoryStream();
                shimRequest.GetResponse = () => shimResponse.Instance;
                shimResponse.StatusCodeGet = () => HttpStatusCode.OK;  // We have to set this to bypass validation logic in Client.GetResponse().

                if (httpStatusCode == HttpStatusCode.OK)
                {
                    shimResponse.GetResponseStream = () => new MemoryStream(Encoding.ASCII.GetBytes(responseMsg));
                }
                else
                {
                    responseMsg = string.Empty;
                    shimResponse.GetResponseStream = () => ThrowWebException(webExceptionStatus, httpStatusCode, successCount, ref timesRetried);
                }

                request = new Request(HttpMethod.Post, "application/json", requestMsg, Encoding.UTF8, 30, timesToRetry);

                client = new Client(baseUri, "SimulatedPost", request);
                response = client.Send(request);
            }

            AssertSuccessfulHttpTransaction(client, request, response, httpStatusCode, successCount, timesRetried, responseMsg);

            return response;
        }


        /// <summary>
        /// General assertions called by SimulateHttpTransaction.
        /// </summary>
        /// <param name="client"> <see cref="IClient"/> </param>
        /// <param name="request"> <see cref="IRequest"/> </param>
        /// <param name="response"> <see cref="IResponse"/> </param>
        /// <param name="successCount">byte - The number of times of retries before the transaction succeeds.</param>
        /// <param name="retryTime">short - The number of times the transaction has been retried.</param>
        /// <param name="responseMsg">string - The message that is expected to be in the response.</param>
        private void AssertSuccessfulHttpTransaction(IClient client, IRequest request, IResponse response, 
            HttpStatusCode httpStatusCode, byte successCount = 0, short timesRetried = 0, string responseMsg = "")
        {
            Assert.IsNotNull(response);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.RawResponse));

            if (!string.IsNullOrWhiteSpace(responseMsg))
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(response.RawResponse));
                Assert.AreEqual(responseMsg, response.RawResponse);

                Assert.IsFalse(request.Log.ErrorsExist());
                Assert.IsFalse(client.Log.ErrorsExist());
                Assert.IsFalse(response.Log.ErrorsExist());
            }
            else
            {
                if (successCount > 0)
                {
                    Assert.IsTrue(successCount == timesRetried);
                }
                else if (timesRetried > 0)
                {
                    Assert.IsTrue(request.MaxRetries == timesRetried);
                    Assert.IsTrue(response.Log.ErrorsExist());

                    string exceptionName = Enum.GetName(typeof(HttpStatusCode), httpStatusCode);
                    Assert.IsTrue(response.Log.ErrorException.Message.Contains(exceptionName));
                }
            }
        }


        /// <summary>
        /// Generates and throws a WebException with WebResponse of specified HttpStatusCode.
        /// </summary>
        /// <param name="webExceptionStatus">WebExceptionStatus</param>
        /// <param name="httpStatusCode">HttpStatusCode</param>
        /// <param name="successCount">The number of retries before success.</param>
        /// <param name="timesRetried">The number of times retried.</param>
        private MemoryStream ThrowWebException(WebExceptionStatus webExceptionStatus, 
            HttpStatusCode httpStatusCode, byte successCount, ref short timesRetried)
        {
            timesRetried++;
            var shimResponse = new ShimHttpWebResponse();
            shimResponse.StatusCodeGet = () => httpStatusCode;

            string exceptionName = Enum.GetName(typeof(HttpStatusCode), httpStatusCode);
            WebException webEx = new WebException(exceptionName, null, webExceptionStatus, shimResponse);

            if (successCount > 0 && timesRetried == successCount)
            {
                shimResponse.StatusCodeGet = () => HttpStatusCode.OK;
                return new MemoryStream(Encoding.ASCII.GetBytes("Success!"));
            }

            throw webEx;
        }


        private string ReadStream(Encoding encoding, MemoryStream memoryStream)
        {
            string value = string.Empty;

            // Grab the response
            using (var stream = memoryStream)
            {
                if (stream == null)
                {
                    return value;
                }

                using (var reader = new StreamReader(stream, encoding))
                {
                    value = reader.ReadToEnd();
                }
            }

            return value;
        }


    }
}

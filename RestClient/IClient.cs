//-----------------------------------------------------------------------
// <summary>
//      Client interface.
// </summary>
//-----------------------------------------------------------------------

using System;
using System.Net;
using System.Threading.Tasks;

namespace Rest.Client
{

    using Common.Logging;

    public interface IClient
    {
        string BaseUri {get; set; }

        string UriParameters { get; set; }

        string FullUri { get; }


        ILog Log { get; set; }


        /// <summary>
        /// Delegate to any ShowDialog method that is expected to be shown to hold the UI thread.
        /// </summary>
        Action ProgressDlgShow { get; set; }

        /// <summary>
        /// Delegate to any HideDialog method that is expected to be called on after background thread completes.
        /// </summary>
        Action ProgressDlgHide { get; set; }


        /// <summary>
        /// Send http request to the combined BaseUri and UriParameters.
        /// </summary>
        /// <param name="request">IRequest</param>
        /// <returns><see cref="IResponse"/></returns>
        IResponse Send(IRequest request);


        /// <summary>
        /// Send http request to the combined BaseUri and UriParameters asynchronously.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns><see cref="IResponse"/></returns>
        IResponse SendAsyncTpl(IRequest request);


        /// <summary>
        /// Send http request to the combined BaseUri and UriParameters asynchronously.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns><see cref="IResponse"/></returns>
        Task<IResponse> SendAsyncAwait(IRequest request);


        /// <summary>
        /// Get the string from reading the response stream.
        /// </summary>
        /// <param name="httpWebRequest">HttpWebRequest</param>
        /// <returns><see cref="IResponse"/>The RawResponse property will be populated.</returns>
        IResponse GetResponse(HttpWebRequest httpWebRequest);



    }
}

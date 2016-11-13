//-----------------------------------------------------------------------
// <summary>
//      Global constants.
// </summary>
//-----------------------------------------------------------------------


namespace Rest.Common
{

    public static class Globals
    {

        /// <summary>
        /// Constants for standard web MIME/content types.
        /// </summary>
        public static class ContentType
        {
            public const string ApplicationJson = "application/json";
            public const string TextJson = "text/json";
            public const string TextHtml = "text/html";
            public const string TextXml = "text/xml";
            public const string ApplicationXml = "application/xml";
            public const string ApplicationSoapXml = "application/soap+xml";
            public const string PlainText = "plain/text";

            /// <summary>
            /// Content-Type sub types.
            /// </summary>
            public static class SubType
            {
                public const string UTF8 = "charset=UTF-8";
            }
        }

    }
}

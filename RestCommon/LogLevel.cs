//-----------------------------------------------------------------------
// <summary>
//      Log level enumerations.
// </summary>
//-----------------------------------------------------------------------


namespace Rest.Common.Logging
{
    /// <summary>
    ///     This enumeration defines the log levels in ascending priority.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        ///     The trace log level (lowest priority).
        /// </summary>
        Trace,

        /// <summary>
        ///     The debug log level.
        /// </summary>
        Debug,

        /// <summary>
        ///     The info log level.
        /// </summary>
        Info,

        /// <summary>
        ///     The warn log level.
        /// </summary>
        Warn,

        /// <summary>
        ///     The error log level.
        /// </summary>
        Error,

        /// <summary>
        ///     The fatal log level (highest priority).
        /// </summary>
        Fatal
    }
}

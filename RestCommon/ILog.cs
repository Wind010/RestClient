//-----------------------------------------------------------------------
// <summary>
//      Logging interface.
// </summary>
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;


namespace Rest.Common.Logging
{
    public interface ILog
    {
        Exception ErrorException { get; set; }

        IList<string> Errors { get; set; }

        IList<string> Warnings { get; set; }

        IList<string> Information { get; set; }

        string GetErrors();

        string GetWarnings();

        string GetInformation();

        bool ErrorsExist();
    }
}

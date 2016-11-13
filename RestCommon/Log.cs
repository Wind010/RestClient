//-----------------------------------------------------------------------
// <summary>
//      Logging class that serves to funnel log information.
// </summary>
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;

namespace Rest.Common.Logging
{
    public class Log : ILog
    {
        private string delim = "  ";

        public string Delim
        {
            get { return this.delim; }
            set
            {
                this.delim = string.IsNullOrEmpty(value) ? "  " : value;
            }
        }

        public Exception ErrorException { get; set; }

        public IList<string> Errors { get; set; }

        public IList<string> Warnings { get; set; }

        public IList<string> Information { get; set; }
        

        public Log()
        {
            Errors = new List<string>();
            Warnings = new List<string>();
            Information = new List<string>();
        }


        public bool ErrorsExist()
        {
            if (ErrorException != null)
            {
                return true;
            }

            if (Errors.Count > 0)
            {
                return true;
            }

            return false;
        }


        public string GetErrors()
        {
            return GetCombinedString(this.delim, Errors);
        }

        public string GetWarnings()
        {
            return GetCombinedString(this.delim, Warnings);
        }

        public string GetInformation()
        {
            return GetCombinedString(this.delim, Information);
        }


        private string GetCombinedString(string delim, IList<string> collection)
        {
            return string.Join(delim, (List<string>)collection);
        }

  

    }
}

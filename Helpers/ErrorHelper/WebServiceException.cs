using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.ErrorHelper
{
    public class WebServiceException : Exception
    {
        public WebServiceException()
        {

        }

        public WebServiceException(string? message) : base(message)
        {

        }
    }
}

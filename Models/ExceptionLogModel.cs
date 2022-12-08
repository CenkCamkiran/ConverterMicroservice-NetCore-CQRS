using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ExceptionLogModel
    {
        public DateTime exceptionDate { get; set; } 
        public string exceptionMessage { get; set; } = string.Empty;    
    }
}

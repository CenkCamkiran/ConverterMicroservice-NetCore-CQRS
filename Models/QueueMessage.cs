using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class QueueMessage
    {
        public string fileGuid { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
    }
}

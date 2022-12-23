using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class NotificationLog
    {
        public string Error { get; set; } = string.Empty;
        public string Info { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}

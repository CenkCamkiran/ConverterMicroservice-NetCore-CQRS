using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SmtpConfiguration
    {
        public string SmtpHost { get; set; } = string.Empty;
        public string SmtpPort { get; set; } = string.Empty;
        public string SmtpMailFrom { get; set; } = string.Empty;
        public string SmtpMailPassword { get; set; } = string.Empty;
        public string SmtpMailUsername { get; set; } = string.Empty;
    }
}

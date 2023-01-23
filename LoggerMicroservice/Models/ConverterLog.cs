using System;

namespace LoggerMicroservice.Models
{
    public class ConverterLog
    {
        public string Error { get; set; } = string.Empty;
        public string Info { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}

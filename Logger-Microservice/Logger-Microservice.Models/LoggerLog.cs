﻿using System;

namespace LoggerMicroservice.Models
{
    public class LoggerLog
    {
        public string ErrorMessage { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}

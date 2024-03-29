﻿using MediatR;

namespace Converter_Microservice.Queries.QueueQueries
{
    public class QueueQuery : IRequest
    {
        public string Queue { get; set; }
        public long MessageTTL { get; set; }

        public QueueQuery(string queue, long messageTTL)
        {
            Queue = queue;
            MessageTTL = messageTTL;
        }
    }
}

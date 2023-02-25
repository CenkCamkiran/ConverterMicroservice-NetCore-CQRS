using MediatR;
using Microsoft.AspNetCore.Http;

namespace WebService.Commands.LogCommands
{
    public class LogCommand : IRequest<bool>
    {
        public string IndexName { get; set; } = string.Empty;
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
        public DateTime RequestDate { get; set; }

        public LogCommand(string ındexName, HttpRequest request, HttpResponse response, DateTime requestDate)
        {
            IndexName = ındexName;
            Request = request;
            Response = response;
            RequestDate = requestDate;
        }
    }
}

using MediatR;

namespace Logger_Microservice.Commands.LogCommands
{
    public class LogCommand : IRequest<bool>
    {
        public object Model { get; set; }
        public string IndexName { get; set; }

        public LogCommand(object model, string ındexName)
        {
            Model = model;
            IndexName = ındexName;
        }
    }
}

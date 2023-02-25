using MediatR;

namespace Logger_Microservice.Commands.LogCommands
{
    public class LogCommand<TModel> : IRequest<bool> where TModel : class
    {
        public TModel Model { get; set; }
        public string IndexName { get; set; } = string.Empty;

        public LogCommand(TModel model, string ındexName)
        {
            Model = model;
            IndexName = ındexName;
        }
    }
}

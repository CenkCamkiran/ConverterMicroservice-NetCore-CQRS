using Logger_Microservice.Commands.LogCommands;
using Logger_Microservice.Repositories.Interfaces;
using MediatR;

namespace Logger_Microservice.Handlers.LogHandlers
{
    public class LogHandler<TModel> : IRequestHandler<LogCommand<TModel>, bool> where TModel : class
    {

        private readonly ILogRepository<TModel> _logRepository;

        public LogHandler(ILogRepository<TModel> logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<bool> Handle(LogCommand<TModel> request, CancellationToken cancellationToken)
        {
            return await _logRepository.IndexDocAsync(request.IndexName, request.Model);
        }
    }
}

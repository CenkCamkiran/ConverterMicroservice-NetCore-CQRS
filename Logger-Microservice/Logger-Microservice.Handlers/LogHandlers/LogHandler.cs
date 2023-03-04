using Logger_Microservice.Commands.LogCommands;
using Logger_Microservice.Repositories.Interfaces;
using MediatR;

namespace Logger_Microservice.Handlers.LogHandlers
{
    public class LogHandler : IRequestHandler<LogCommand, bool>
    {

        private readonly ILogRepository _logRepository;

        public LogHandler(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<bool> Handle(LogCommand request, CancellationToken cancellationToken)
        {
            return await _logRepository.IndexDocAsync(request.IndexName, request.Model);
        }
    }
}

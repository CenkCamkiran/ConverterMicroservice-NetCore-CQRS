namespace Logger_Microservice.Repositories.Interfaces
{
    public interface ILog4NetRepository
    {
        void Debug(string message);

        void Fatal(string message);

        void Info(string message);

        void Error(string message, Exception ex = null);
    }
}

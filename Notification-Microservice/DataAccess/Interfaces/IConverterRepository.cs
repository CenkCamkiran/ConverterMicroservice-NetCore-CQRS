namespace DataAccess.Interfaces
{
    public interface IConverterRepository
    {
        Task ConvertMP4_to_MP3(string ConvertFromFilePath, string ConvertToFilePath);
    }
}

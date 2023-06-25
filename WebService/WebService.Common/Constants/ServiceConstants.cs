namespace WebService.Common.Constants
{
    public static partial class ProjectConstants
    {
        public static string FileUploadSizeLinit { get; set; } = Environment.GetEnvironmentVariable("FILE_LENGTH_LIMIT");
    }
}

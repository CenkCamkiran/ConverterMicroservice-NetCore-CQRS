using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebService.Common.Constants
{
    public static partial class ProjectConstants
    {
        public static string FileUploadSizeLinit { get; set; } = Environment.GetEnvironmentVariable("FILE_LENGTH_LIMIT");
    }
}

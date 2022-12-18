using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IConverterRepository
    {
        Task ConvertMP4_to_MP3(string ConvertFromFilePath, string ConvertToFilePath);
    }
}

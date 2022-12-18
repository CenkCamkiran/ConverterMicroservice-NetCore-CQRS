using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Interfaces
{
    public interface IConverterOperation
    {
        Task ConvertMP4_to_MP3(ObjectDataModel objectDataModel, QueueMessage message);
    }
}

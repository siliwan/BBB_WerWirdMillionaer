using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Models.GameSessions.ReponseObjects
{
    public class ObjectResponseWithMessage<T>
    {
        public T Object { get; set; }
        public string? Message { get; set; }
    }
}

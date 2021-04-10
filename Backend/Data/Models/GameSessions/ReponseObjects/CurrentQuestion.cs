using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Models.GameSessions.ReponseObjects
{
    public class CurrentQuestion
    {
        public DateTime TimeLeftUntil { get; set; }
        public Question Question { get; set; }
        public double PercentCorrect { get; set; }
        public int Points { get; set; }
    }
}

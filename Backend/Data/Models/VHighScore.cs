using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Models
{
    public class VHighScore
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime TimeStamp { get; set; }
        public int PointsAchieved { get; set; }
        public int Duration { get; set; }
        
        public long Rank { get; private set; }
        public int PointsWeighted { get; private set; }

        public string Categories { get; set; }
    }
}

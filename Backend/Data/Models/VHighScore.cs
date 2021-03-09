using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Models
{
    public class VHighScore
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public int PointsAchieved { get; set; }
        public long Duration { get; set; }
        
        public int Rank { get; }
        public float PointsWeighted { get; }

        public ICollection<Category> Categories { get; set; } = new HashSet<Category>();
    }
}

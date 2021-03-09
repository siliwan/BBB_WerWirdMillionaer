using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Models
{
    public class Highscore
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public int PointsAchieved { get; set; }
        public long Duration { get; set; }

        public float PointsWeighted { get; private set; }

        public ICollection<Category> Categories { get; set; } = new HashSet<Category>();
    }
}

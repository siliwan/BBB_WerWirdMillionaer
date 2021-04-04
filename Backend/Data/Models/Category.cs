using System.Collections.Generic;

namespace Backend.Data.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Question> Questions { get; set; } = new HashSet<Question>();
        public ICollection<Highscore> Highscores { get; set; } = new HashSet<Highscore>();
    }
}
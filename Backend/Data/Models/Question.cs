using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }

        public QuestionStatistic QuestionStatistic { get; set; }
        public Category Category { get; set; }
        public ICollection<Answer> Answers { get; set; } = new HashSet<Answer>();
    }
}

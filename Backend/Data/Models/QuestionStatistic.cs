namespace Backend.Data.Models
{
    public class QuestionStatistic
    {
        public int Id { get; set; }
        public int AnsweredCorrect { get; set; }
        public int AnsweredWrong { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
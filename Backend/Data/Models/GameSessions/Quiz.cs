using System;
using System.Collections.Generic;

namespace Backend.Data.Models.GameSessions
{
    public class Quiz
    {
        private const int QUESTION_VALUE = 30;

        public int Points { get { return QuestionsAnswered.Count * QUESTION_VALUE; } }
        public bool HasJoker { get; set; }
        public int Round { get; set; }
        public int DurationQuiz { get; set; }
        public ICollection<Category> SelectedCategories { get; set; }
        public ICollection<Question> QuestionsAnswered { get; set; }
        public DateTime RoundStartedAt { get; set; }
        public Question CurrentQuestion { get; set; }
        public bool JokerUsedThisRound { get; set; }
        public GameSession Session { get; set; }
        public PlayState State { get; set; }
    }
}
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Repositories
{
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        private const int MAX_QUESTIONS = 4;
        private const int MAX_CORRECT_QUESTIONS = 1;

        public QuestionRepository(DatabaseContext context) 
            : base(context)
        { }

        public void AddQuestion(string questionText, Category category, IEnumerable<(string Answer, bool IsCorrect)> answers)
        {
            if (answers.Count() != MAX_QUESTIONS) throw new ArgumentException($"Only {MAX_QUESTIONS} questions are allowed, but received {answers.Count()}!");
            if (answers.Count(answers => answers.IsCorrect) != MAX_CORRECT_QUESTIONS) throw new ArgumentException($"Only {MAX_CORRECT_QUESTIONS} correct questions are allowed, but received {answers.Count(answers => answers.IsCorrect)}!");

            var question = new Question
            {
                QuestionText = questionText,
                Category = category,

                QuestionStatistic = new()
            };

            Add(question);

            _context.Set<Answer>().AddRange(answers.Select(answer => new Answer
            {
                AnswerText = answer.Answer,
                IsCorrect = answer.IsCorrect,
                Question = question
            }));
        }

        public void AnsweredCorrect(Question question)
        {
            question.QuestionStatistic.AnsweredCorrect++;
        }

        public void AnsweredWrong(Question question)
        {
            question.QuestionStatistic.AnsweredWrong++;
        }
    }
}

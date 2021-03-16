using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public new IEnumerable<Question> GetAll()
        {
            return _context.Set<Question>()
                           .Include(q => q.Answers)
                           .Include(q => q.QuestionStatistic)
                           .Include(q => q.Category)
                           .ToList();
        }

        public new Question GetById(int id)
        {
            return _context.Set<Question>()
                           .Include(q => q.Answers)
                           .Include(q => q.QuestionStatistic)
                           .Include(q => q.Category)
                           .FirstOrDefault(q => q.Id == id);
        }

        public new IEnumerable<Question> Find(Expression<Func<Question, bool>> expression)
        {
            return _context.Set<Question>().Where(expression);
        }

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

        public Question GetRandomQuestion(ICollection<Question> questionsToExclude)
        {
            var QuestionKeys = questionsToExclude.Select(x => x.Id);
            return _context.Set<Question>()
                           .Include(q => q.Answers)
                           .Include(q => q.QuestionStatistic)
                           .Include(q => q.Category)
                           .Where(q => !QuestionKeys.Contains(q.Id))
                           .OrderBy(o => Guid.NewGuid())
                           .FirstOrDefault();
        }
    }
}

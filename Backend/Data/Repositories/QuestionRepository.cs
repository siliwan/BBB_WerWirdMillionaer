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
            if (questionText == null) throw new ArgumentException($"The question text must not be null!", nameof(questionText));
            if (category == null) throw new ArgumentException($"The category must not be null!", nameof(category));
            if (answers.Count() != MAX_QUESTIONS) throw new ArgumentException($"Only {MAX_QUESTIONS} questions are allowed, but received {answers.Count()}!", nameof(answers));
            if (answers.Count(answers => answers.IsCorrect) != MAX_CORRECT_QUESTIONS) throw new ArgumentException($"Only {MAX_CORRECT_QUESTIONS} correct questions are allowed, but received {answers.Count(answers => answers.IsCorrect)}!", nameof(answers));
            if (answers.Any(answers => answers.Answer == null)) throw new ArgumentException($"All answer texts must not be null!", nameof(answers));

            _context.Database.BeginTransaction();

            var question = new Question
            {
                QuestionText = questionText,
                Category = category,

                QuestionStatistic = new()
            };

            Add(question);

            var AnswersToAdd = answers.Select(answer => new Answer
            {
                AnswerText = answer.Answer,
                IsCorrect = answer.IsCorrect,
                Question = question
            });

            _context.Set<Answer>().AddRange(AnswersToAdd);

            _context.Database.CommitTransaction();
        }

        public void AnsweredCorrect(Question question)
        {
            _context.Set<QuestionStatistic>().Where(stat => stat.QuestionId == question.Id)
                                             .FirstOrDefault()
                                             .AnsweredCorrect++;
        }

        public void AnsweredWrong(Question question)
        {
            _context.Set<QuestionStatistic>().Where(stat => stat.QuestionId == question.Id)
                                             .FirstOrDefault()
                                             .AnsweredWrong++;
        }

        public Question GetRandomQuestion(ICollection<Question> questionsToExclude, ICollection<Category> categories)
        {
            var QuestionKeys = questionsToExclude.Select(x => x.Id);
            var CategoryKeys = categories.Select(x => x.Id);
            return _context.Set<Question>()
                           .Include(q => q.Answers)
                           .Include(q => q.QuestionStatistic)
                           .Include(q => q.Category)
                           .Where(q => CategoryKeys.Contains(q.Category.Id))
                           .Where(q => !QuestionKeys.Contains(q.Id))
                           .OrderBy(o => Guid.NewGuid())
                           .FirstOrDefault();
        }

        public new void Remove(Question entity)
        {
            _context.Set<Question>().Remove(entity);
        }

        public new void RemoveRange(IEnumerable<Question> entities)
        {
            _context.Set<Question>().RemoveRange(entities);
        }
    }
}

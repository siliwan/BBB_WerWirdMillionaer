using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Repositories
{
    public class AnswerRepository : Repository<Answer>, IAnswerRepository
    {
        public AnswerRepository(DatabaseContext context)
            : base(context) { }

        public void AddAnswer(Question question, string answerText, bool isCorrect)
        {
            question.Answers.Add(new Answer
            {
                AnswerText = answerText,
                IsCorrect = isCorrect
            });
        }
    }
}

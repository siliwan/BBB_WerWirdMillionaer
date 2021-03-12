using Backend.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Repositories.Interfaces
{
    public interface IQuestionRepository : IRepository<Question>
    {
        void AnsweredCorrect(Question question);
        void AnsweredWrong(Question question);
        void AddQuestion(string questionText, Category category, IEnumerable<(string Answer, bool IsCorrect)> answers);
    }
}

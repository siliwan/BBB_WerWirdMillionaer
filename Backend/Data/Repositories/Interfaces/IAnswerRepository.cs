using Backend.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Repositories.Interfaces
{
    public interface IAnswerRepository : IRepository<Answer>
    {
        public void AddAnswer(Question question, string answerText, bool isCorrect);
    }
}

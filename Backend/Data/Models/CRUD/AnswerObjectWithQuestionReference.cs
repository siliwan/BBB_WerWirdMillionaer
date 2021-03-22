using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Models.CRUD
{
    public class AnswerObjectWithQuestionReference : AnswerObject
    {
        public int QuestionId { get; set; }
    }
}

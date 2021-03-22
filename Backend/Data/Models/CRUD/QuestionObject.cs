﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Models.CRUD
{
    public class QuestionObject
    {
        public string QuestionText { get; set; }
        public int CategoryId { get; set; }
        public AnswerObject[] Answers { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Models.Config
{
    public class QuizSettings
    {
        public const string ConfigPath = "Quiz";

        public int RoundTimeSec { get; set; } = 30;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Models.GameSessions
{
    public class GameSession
    {
        public string SessionId { get; set; }
        public Quiz CurrentQuiz { get; set; }
    }
}

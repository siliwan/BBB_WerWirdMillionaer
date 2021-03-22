using Backend.Data.Models.GameSessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services
{
    public interface IGameSessionAcessor
    {
        GameSession GetOrCreateSession();
        GameSession GetCurrentSession();
        GameSession CreateSession();
        void ResetSession();
        bool HasActiveSession();
        bool HasAnsweredAllQuestions();
        void GoToNextQuestion();
    }
}

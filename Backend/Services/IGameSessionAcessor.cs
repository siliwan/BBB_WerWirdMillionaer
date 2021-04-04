using Backend.Data.Models;
using Backend.Data.Models.GameSessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services
{
    public interface IGameSessionAcessor
    {
        GameSession GetCurrentSession();
        GameSession CreateSession(Category[] categories);
        void ResetSession();
        bool HasActiveSession();
        bool HasAnsweredAllQuestions();
        void GoToNextQuestion();
        void IncreaseWin();
        void IncreaseLost();
        void SubmitHighscore(string name);
    }
}

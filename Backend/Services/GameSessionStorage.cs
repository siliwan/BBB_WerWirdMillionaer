using Backend.Data.Models.GameSessions;
using Backend.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class GameSessionStorage : IGameSessionStorage
    {
        private readonly IDictionary<string, GameSession> cache = new Dictionary<string, GameSession>();

        public GameSessionStorage() { }

        public bool GetSessionById(string sessionId, out GameSession session)
        {
            return cache.TryGetValue(sessionId, out session);
        }

        public void ClearExpiredSessions()
        {
            throw new NotImplementedException();
        }

        public GameSession CreateSession(string sessionId)
        {
            var sessionObject = CreateSessionObject(sessionId);
            if(!cache.TryAdd(sessionId, sessionObject))
            {
                cache[sessionId] = sessionObject;
            }
            return sessionObject;
        }

        private GameSession CreateSessionObject(string sessionId)
        {
            var Quiz = new Quiz
            {
                CurrentQuestion = null,
                HasJoker = true,
                Round = 0,
                DurationQuiz = 0,
                QuestionsAnswered = new HashSet<Data.Models.Question>()
            };

            var Session = new GameSession
            {
                SessionId = sessionId,
                CurrentQuiz = Quiz,
            };

            Quiz.Session = Session;

            return Session;
        }

        public void ResetSession(string sessionId)
        {
            cache[sessionId] = CreateSession(sessionId);
        }
    }
}

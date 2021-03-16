using Backend.Data.Models.GameSessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services
{
    public interface IGameSessionStorage
    {
        public bool GetSessionById(string sessionId, out GameSession session);
        public void ResetSession(string sessionId);
        public void ClearExpiredSessions();
        public GameSession CreateSession(string sessionId);
    }
}

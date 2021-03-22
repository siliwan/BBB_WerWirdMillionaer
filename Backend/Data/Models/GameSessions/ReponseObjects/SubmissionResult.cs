using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Models.GameSessions.ReponseObjects
{
    public enum SubmissionResult
    {
        Won,
        Lost,
        Correct,
        TimeUp,
        Invalid
    }
}

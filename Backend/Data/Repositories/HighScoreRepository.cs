using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Repositories
{
    public class HighScoreRepository : Repository<Highscore>, IHighScoreRepository
    {
        public HighScoreRepository(DatabaseContext context)
            : base(context)
        { }

        public IEnumerable<VHighScore> GetHighscores()
        {
            return _context.Set<VHighScore>().ToList();
        }
    }
}

using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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


        public void AddHighscore(string name, int pointsAchieved, int duration, IEnumerable<Category> categories)
        {
            var CategoryIdKeys = categories.Select(x => x.Id);
            var Categories = _context.Set<Category>()
                                     .Where(x => CategoryIdKeys.Contains(x.Id))
                                     .ToList();
            Add(new Highscore
            {
                Name = name,
                PointsAchieved = pointsAchieved,
                Duration = duration,
                Categories = Categories,
                TimeStamp = DateTime.Now
            });

            SaveChanges();
        }

        public IEnumerable<VHighScore> GetHighscores()
        {
            return _context.VHighScores
                           .ToList();
        }
    }
}

using Backend.Data.Models;
using Backend.Data.Repositories;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Highscore> Highscores { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionStatistic> QuestionStatistics { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<VHighScore> VHighScores {get; set;} 

        public DatabaseContext() : base() { }
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Question>()
                        .HasOne(q => q.QuestionStatistic)
                        .WithOne(qs => qs.Question)
                        .HasForeignKey<QuestionStatistic>(qs => qs.QuestionId);

            modelBuilder.Entity<Highscore>()
                        .Property(h => h.PointsWeighted)
                        .ValueGeneratedOnAddOrUpdate()
                        .HasComputedColumnSql($"[{nameof(Highscore.PointsAchieved)}] / [{nameof(Highscore.Duration)}]");

            modelBuilder.Entity<VHighScore>(
                    eb =>
                    {
                        eb.HasNoKey();
                        eb.ToView(nameof(VHighScore));
                    }
                );
        }
        
    }

    public static class DatabaseContextExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICategoryRepository, CategoryRepository>();
            serviceCollection.AddTransient<IHighScoreRepository, HighScoreRepository>();
            serviceCollection.AddTransient<IQuestionRepository, QuestionRepository>();
            serviceCollection.AddTransient<IUserRepository, UserRepository>();
            serviceCollection.AddTransient<IAnswerRepository, AnswerRepository>();
            return serviceCollection;
        }
    }
}

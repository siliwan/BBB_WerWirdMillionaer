using Backend.Data.Models;
using Backend.Data.Models.GameSessions;
using Backend.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class GameSessionAcessor : IGameSessionAcessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IGameSessionStorage storage;
        private readonly IQuestionRepository questionRepository;

        private string SessionId { 
            get {
                if (httpContextAccessor.HttpContext.Request.Headers.TryGetValue("x-quiz-session-id", out var sessIdHeader))
                {
                    return sessIdHeader;
                }

                return null;
            } 
        }

        public GameSessionAcessor(IHttpContextAccessor httpContextAccessor,
                                  IGameSessionStorage storage,
                                  IQuestionRepository questionRepository)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.storage = storage;
            this.questionRepository = questionRepository;
        }

        public GameSession GetCurrentSession()
        {
            storage.GetSessionById(SessionId, out GameSession gameSession);
            return gameSession;
        }

        public bool HasActiveSession()
        {
            return storage.GetSessionById(SessionId, out _);
        }

        public GameSession CreateSession(Category[] categories)
        {
            var SessionCreated = storage.CreateSession(SessionId);
            SessionCreated.CurrentQuiz.SelectedCategories = categories;
            SessionCreated.CurrentQuiz.CurrentQuestion = questionRepository.GetRandomQuestion(new List<Data.Models.Question>(), SessionCreated.CurrentQuiz.SelectedCategories);
            SessionCreated.CurrentQuiz.RoundStartedAt = DateTime.Now;
            return SessionCreated;
        }

        public void ResetSession()
        {
            if (HasActiveSession())
            {
                storage.ResetSession(SessionId);
            }
        }

        public bool HasAnsweredAllQuestions()
        {
            var Session = GetCurrentSession();
            return questionRepository.GetRandomQuestion(Session.CurrentQuiz.QuestionsAnswered, Session.CurrentQuiz.SelectedCategories) == null;
        }

        public void GoToNextQuestion()
        {
            var Session = GetCurrentSession();
            Session.CurrentQuiz.QuestionsAnswered.Add(Session.CurrentQuiz.CurrentQuestion);
            Session.CurrentQuiz.CurrentQuestion = questionRepository.GetRandomQuestion(Session.CurrentQuiz.QuestionsAnswered, Session.CurrentQuiz.SelectedCategories);
            Session.CurrentQuiz.RoundStartedAt = DateTime.Now;
        }

        public void IncreaseWin()
        {
            var Session = GetCurrentSession();
            questionRepository.AnsweredCorrect(Session.CurrentQuiz.CurrentQuestion);
            questionRepository.SaveChanges();
        }

        public void IncreaseLost()
        {
            var Session = GetCurrentSession();
            questionRepository.AnsweredWrong(Session.CurrentQuiz.CurrentQuestion);
            questionRepository.SaveChanges();
        }
    }

    public static class GameSessionAcessorExtensions { 
        public static IServiceCollection AddGameSession(this IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddSingleton<IGameSessionStorage, GameSessionStorage>();
            serviceDescriptors.AddScoped<IGameSessionAcessor, GameSessionAcessor>();
            
            //To work, sessions have to be pinned!
            serviceDescriptors.AddScoped<PinSessionActionFilter>();

            return serviceDescriptors;
        }
    }
}

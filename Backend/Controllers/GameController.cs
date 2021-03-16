using Backend.Data.Models.GameSessions.ReponseObjects;
using Backend.Data.Models.GameSessions;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data.Models;
using Backend.Tools;

namespace Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> logger;
        private readonly IGameSessionAcessor game;

        private const int ROUND_TIME_SEC = 30;

        private GameSession _session { get; set; }
        private GameSession Session { 
            get { 
                if(_session == null)
                {
                    _session = game.GetCurrentSession();
                }
                return _session;
            } 
        }
        private Quiz Quiz { get { return Session.CurrentQuiz; } }

        public GameController(ILogger<GameController> logger,
                              IGameSessionAcessor game)
        {
            this.logger = logger;
            this.game = game;
        }

        [HttpGet]
        public ActionResult<bool> HasJoker()
        {
            if (!game.HasActiveSession() || Quiz.State != PlayState.Playing)
            {
                return Unauthorized();
            }

            return Quiz.HasJoker;
        }

        [HttpPost]
        public ActionResult UseJoker()
        {
            if (!game.HasActiveSession() || Quiz.State != PlayState.Playing)
            {
                return Unauthorized();
            }

            if(Quiz.HasJoker)
            {
                Quiz.HasJoker = false;
                return Ok();
            } else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        public ActionResult<PlayState> CurrentState()
        {
            return !game.HasActiveSession() ? 
                PlayState.Menu 
                : Quiz.State;
        }

        [HttpPost]
        public void StartGame()
        {
            game.GetOrCreateSession();
            Quiz.State = PlayState.Playing;
        }

        [HttpGet]
        public ActionResult<CurrentQuestion> CurrentQuestion()
        {
            if(!game.HasActiveSession() || Quiz.State != PlayState.Playing)
            {
                return Unauthorized();
            }

            var CurrentQuestion = Quiz.CurrentQuestion;

            return new CurrentQuestion
            {
                Question = CurrentQuestion,
                PercentCorrect = CalculatePercentageCorrect(CurrentQuestion),
                TimeLeftUntil = Quiz.RoundStartedAt
                                    .AddSeconds(ROUND_TIME_SEC)
                                    .Ceiling(DateTimeRoundLevel.Miliseconds)
            };
        }

        private double CalculatePercentageCorrect(Question question)
        {
            var AnsweredWrong   = (double) question.QuestionStatistic.AnsweredWrong;
            var AnsweredCorrect = (double) question.QuestionStatistic.AnsweredCorrect;

            if(AnsweredWrong <= 0.00)
            {
                return AnsweredCorrect == 0.00 ? 
                        0.00 
                    : 100.00;
            }

            if(AnsweredCorrect <= 0)
            {
                return 0.00;
            }

            return AnsweredCorrect / AnsweredWrong;
        }

    }

}

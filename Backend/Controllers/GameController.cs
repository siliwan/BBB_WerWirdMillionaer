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
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    [ServiceFilter(typeof(PinSessionActionFilter))]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> logger;
        private readonly IGameSessionAcessor game;

        private const int ROUND_TIME_SEC = 30 * 100;

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
                Quiz.JokerUsedThisRound = true;
                return Ok();
            } else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        public ActionResult<PlayState> CurrentState()
        {
            if (game.HasActiveSession() && DateTime.Now >= Quiz.RoundStartedAt
                                                               .AddSeconds(ROUND_TIME_SEC)
                                                               .Ceiling(DateTimeRoundLevel.Miliseconds))
            {
                Quiz.State = PlayState.Lost;
            }

            return !game.HasActiveSession() ? 
                PlayState.Menu 
                : Quiz.State;
        }

        [HttpPost]
        public void StartGame([FromBody] Category[] categories)
        {
            game.CreateSession(categories);
            Quiz.State = PlayState.Playing;
        }

        [HttpPost]
        public IActionResult CashIn()
        {
            if (!game.HasActiveSession() || Quiz.State != PlayState.Playing)
            {
                return Unauthorized();
            }
            else
            {
                Quiz.State = PlayState.Won;
                return Ok();
            }
        }

        [HttpPost]
        public void SubmitHighscore([FromBody] string Name)
        {
            if (!game.HasActiveSession() || Quiz.State != PlayState.Won)
            {
                Unauthorized();
            } else
            {
                game.SubmitHighscore(Name);
                Quiz.State = PlayState.Menu;
                Ok();
            }
        }

        [HttpPost]
        public ActionResult<ObjectResponseWithMessage<SubmissionResult>> SubmitAnswer([FromBody] int answerId)
        {
            if (!game.HasActiveSession() || Quiz.State != PlayState.Playing)
            {
                return Unauthorized();
            }

            if(DateTime.Now >= Quiz.RoundStartedAt
                                   .AddSeconds(ROUND_TIME_SEC)
                                   .Ceiling(DateTimeRoundLevel.Miliseconds))
            {
                Quiz.State = PlayState.Lost;
                return new ObjectResponseWithMessage<SubmissionResult> {
                    Object = SubmissionResult.TimeUp,
                    Message = "Time is up!"
                };
            }

            var Answers = Session.CurrentQuiz.CurrentQuestion.Answers;

            if(!Answers.Any(x => x.Id == answerId))
            {
                return BadRequest($"An invalid answer has been submitted with id {answerId}!");
            }

            var CorrectAnswer = Answers.First(x => x.IsCorrect);
            if (CorrectAnswer.Id == answerId)
            {
                Quiz.Round++;
                Quiz.JokerUsedThisRound = false;
                int TimeLeft = (int) Math.Round(ROUND_TIME_SEC - (DateTime.Now - Quiz.RoundStartedAt).TotalSeconds, MidpointRounding.AwayFromZero);
                Quiz.DurationQuiz += TimeLeft >= 0 ? (int)Math.Round((DateTime.Now - Quiz.RoundStartedAt).TotalSeconds) : 0;
                game.IncreaseWin();
                Session.CurrentQuiz.QuestionsAnswered.Add(Session.CurrentQuiz.CurrentQuestion);
                //Check if won else next round
                if(game.HasAnsweredAllQuestions())
                {
                    Quiz.State = PlayState.Won;
                    return new ObjectResponseWithMessage<SubmissionResult>
                    {
                        Object = SubmissionResult.Won
                    };
                } else
                {
                    game.GoToNextQuestion();
                    return new ObjectResponseWithMessage<SubmissionResult>
                    {
                        Object = SubmissionResult.Correct
                    };
                }
            }
            else
            {
                game.IncreaseLost();
                //Bad luck, set to lost
                Quiz.State = PlayState.Lost;
                return new ObjectResponseWithMessage<SubmissionResult>
                {
                    Object = SubmissionResult.Lost,
                    Message = $"Wrong answer. The correct answer was: {CorrectAnswer.AnswerText}"
                };
            }
        }

        [HttpGet]
        public ActionResult<CurrentQuestion> CurrentQuestion()
        {
            if(!game.HasActiveSession() || Quiz.State != PlayState.Playing)
            {
                return Unauthorized();
            }

            var CurrentQuestion = Quiz.CurrentQuestion;

            if (DateTime.Now >= Quiz.RoundStartedAt
                                    .AddSeconds(ROUND_TIME_SEC)
                                    .Ceiling(DateTimeRoundLevel.Miliseconds))
            {
                Quiz.State = PlayState.Lost;
            }

            if(Quiz.JokerUsedThisRound)
            {
                var CorrectAnswer = CurrentQuestion.Answers.Where(a => a.IsCorrect);
                var OneWrongAnswerWithOneCorrectAnswer = CurrentQuestion.Answers.Where(a => !a.IsCorrect)
                                                                                .Take(1)
                                                                                .Concat(CorrectAnswer);
                var QuestionModified = CurrentQuestion;
                CurrentQuestion.Answers = OneWrongAnswerWithOneCorrectAnswer.ToList();
                return new CurrentQuestion
                {
                    Question = QuestionModified,
                    PercentCorrect = CalculatePercentageCorrect(CurrentQuestion),
                    TimeLeftUntil = Quiz.RoundStartedAt
                                    .AddSeconds(ROUND_TIME_SEC)
                                    .Ceiling(DateTimeRoundLevel.Miliseconds)
                };
            }

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

            return Math.Round((AnsweredWrong / (AnsweredCorrect + AnsweredWrong)) * 100, 2);
        }

    }

}

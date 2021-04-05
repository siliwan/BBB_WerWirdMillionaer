using Backend.Data.Models;
using Backend.Data.Models.CRUD;
using Backend.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly ILogger<QuestionController> logger;
        private readonly IQuestionRepository questionRepository;
        private readonly ICategoryRepository categoryRepository;

        public QuestionController(
                ILogger<QuestionController> logger,
                IQuestionRepository questionRepository,
                ICategoryRepository categoryRepository)
        {
            this.logger = logger;
            this.questionRepository = questionRepository;
            this.categoryRepository = categoryRepository;
        }

        // GET: api/<QuestionController>
        [HttpGet]
        public IEnumerable<Question> Get()
        {
            var questions = questionRepository.GetAll().ToList();
            questions.ForEach(q =>
            {
                q.Category.Questions = new List<Question>();
                q.Category.Highscores = new List<Highscore>();
            });

            return questions;
        }

        // GET api/<QuestionController>/5
        [HttpGet("{id}")]
        public ActionResult<Question> Get(int id)
        {
            var QuestionToReturn = questionRepository.GetById(id);
            if(QuestionToReturn != null)
            {
                QuestionToReturn.Category.Questions = new List<Question>();
                QuestionToReturn.Category.Highscores = new List<Highscore>();
                return QuestionToReturn;
            } else
            {
                return BadRequest($"Question with id {id} does not exist.");
            }
            
        }

        // POST api/<QuestionController>
        [HttpPost]
        public ActionResult Post([FromBody] QuestionObject question)
        {
            if(!TryValidateModel(question))
            {
                return BadRequest();
            }

            var Category = categoryRepository.GetById(question.CategoryId);

            if(Category == null)
            {
                return BadRequest("You must provide an existing category!");
            }

            try
            {
                questionRepository.AddQuestion(question.QuestionText,
                                           Category,
                                           question.Answers.Select(
                                               q => (q.AnswerText, q.IsCorrect))
                                           );

                questionRepository.SaveChanges();
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        // PUT api/<QuestionController>/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] QuestionSimple question)
        {
            var QuestionToModify = questionRepository.GetById(id);
            var Category = categoryRepository.GetById(question.CategoryId);

            if(Category == null)
            {
                BadRequest($"Category with id {question.CategoryId} does not exist.");
            } else
            {
                if (QuestionToModify != null)
                {
                    QuestionToModify.QuestionText = question.QuestionText;
                    QuestionToModify.Category = Category;
                    await questionRepository.SaveChangesAsync();
                    NoContent();
                }
                else
                {
                    BadRequest($"Question with id {id} does not exist.");
                }
            }

        }

        // DELETE api/<QuestionController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var QuestionToRemove = questionRepository.GetById(id);
            
            if(QuestionToRemove != null)
            {
                questionRepository.Remove(QuestionToRemove);
                await questionRepository.SaveChangesAsync();
                Ok();
            } else
            {
                BadRequest($"Question with id {id} does not exist.");
            }
        }
    }
}

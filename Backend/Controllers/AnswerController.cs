using Backend.Data.Models;
using Backend.Data.Models.CRUD;
using Backend.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly ILogger<AnswerController> logger;
        private readonly IAnswerRepository answerRepository;
        private readonly IQuestionRepository questionRepository;

        public AnswerController(ILogger<AnswerController> logger,
                                IAnswerRepository answerRepository,
                                IQuestionRepository questionRepository)
        {
            this.logger = logger;
            this.answerRepository = answerRepository;
            this.questionRepository = questionRepository;
        }

        // GET: api/<AnswerController>
        [HttpGet]
        public IEnumerable<Answer> Get()
        {
            return answerRepository.GetAll();
        }

        // GET api/<AnswerController>/5
        [HttpGet("{id}")]
        public Answer Get(int id)
        {
            return answerRepository.GetById(id);
        }

        // POST api/<AnswerController>
        [HttpPost]
        public ActionResult Post([FromBody] AnswerObjectWithQuestionReference answerObject)
        {
            if (!TryValidateModel(answerObject))
            {
                return BadRequest();
            }

            var Question = questionRepository.GetById(answerObject.QuestionId);

            if(Question == null)
            {
                return BadRequest($"Question with id {answerObject.QuestionId} does not exist!");
            }

            answerRepository.AddAnswer(Question, answerObject.AnswerText, answerObject.IsCorrect);
            answerRepository.SaveChanges();

            return Ok();
        }

        // PUT api/<AnswerController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] AnswerObject answerObject)
        {
            if(!TryValidateModel(answerObject))
            {
                return BadRequest();
            }

            var Answer = answerRepository.GetById(id);

            if(Answer == null)
            {
                return BadRequest($"Answer with id {id} does not exist.");
            }

            Answer.AnswerText = answerObject.AnswerText;
            Answer.IsCorrect = answerObject.IsCorrect;

            answerRepository.SaveChanges();

            return Ok(Answer);
        }

        // DELETE api/<AnswerController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var Answer = answerRepository.GetById(id);

            if (Answer == null)
            {
                return BadRequest($"Answer with id {id} does not exist.");
            }

            answerRepository.Remove(Answer);
            answerRepository.SaveChanges();

            return Ok();
        }
    }
}

using Backend.Data.Models;
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

        public QuestionController(
                ILogger<QuestionController> logger,
                IQuestionRepository questionRepository)
        {
            this.logger = logger;
            this.questionRepository = questionRepository;
        }

        // GET: api/<QuestionController>
        [HttpGet]
        public IEnumerable<Question> Get()
        {
            return questionRepository.GetAll();
        }

        // GET api/<QuestionController>/5
        [HttpGet("{id}")]
        public ActionResult<Question> Get(int id)
        {
            var QuestionToReturn = questionRepository.GetById(id);
            if(QuestionToReturn != null)
            {
                return QuestionToReturn;
            } else
            {
                return NotFound();
            }
            
        }

        // POST api/<QuestionController>
        [HttpPost]
        public void Post([FromForm] string value)
        {
            throw new NotImplementedException();
        }

        // PUT api/<QuestionController>/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] string questionText)
        {
            var QuestionToModify = questionRepository.GetById(id);

            if(QuestionToModify != null)
            {
                QuestionToModify.QuestionText = questionText;
                await questionRepository.SaveChangesAsync();
                NoContent();
            } else
            {
                NotFound();
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
                NotFound();
            }
        }
    }
}

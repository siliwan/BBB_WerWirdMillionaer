using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    public class HighScoreController : ControllerBase
    {
        private readonly ILogger<HighScoreController> logger;
        private readonly IHighScoreRepository highScoreRepositoy;

        public HighScoreController(ILogger<HighScoreController> logger,
                                   IHighScoreRepository highScoreRepositoy)
        {
            this.logger = logger;
            this.highScoreRepositoy = highScoreRepositoy;
        }

        // GET api/<HighScoreController>
        [HttpGet]
        public IEnumerable<VHighScore> Get()
        {
            return highScoreRepositoy.GetHighscores();
        }

        // GET api/<HighScoreController>/5
        [HttpGet("{id}")]
        public ActionResult<VHighScore> Get(int id)
        {
            var HighScore = highScoreRepositoy.GetHighscores()
                                              .FirstOrDefault(x => x.Id == id);
            if(HighScore == null)
            {
                return BadRequest($"Highscore with id {id} does not exist.");
            }

            return HighScore;
        }

        // DELETE api/<HighScoreController>/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var HighScore = highScoreRepositoy.GetById(id);

            if(HighScore == null)
            {
                BadRequest($"Highscore with id {id} does not exist.");
            } else
            {
                highScoreRepositoy.Remove(HighScore);
                await highScoreRepositoy.SaveChangesAsync();
                NoContent();
            }
        }
    }
}

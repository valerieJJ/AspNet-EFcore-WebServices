using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neverland.Data;
using Neverland.Domain;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MovieService.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly DataContext _context;

        public ValuesController(ILogger<ValuesController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }



        [HttpGet]
        private IEnumerable<Movie> Query()
        {
            var movies = _context.Movies.ToArray();
            return movies;
        }

        [HttpGet]
        public async Task<IEnumerable<Movie>> Index()
        {
            var movies = _context.Movies.ToListAsync().Result;
            //foreach (var order in orders)
            //{
            //    order.Movie = movies.Where(m => m.Id == order.MovieId).FirstOrDefault();
            //    order.User = users.Where(u => u.Id == order.UserId).FirstOrDefault();
            //}

            return movies;
        }

        // GET: api/values
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            var movies = _context.Movies.ToArray();
            return movies.ToString();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}


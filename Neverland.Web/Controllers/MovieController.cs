using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neverland.Data;
using Neverland.Domain;
using Neverland.Web.ViewModels;
namespace Neverland.Web.Controllers
{
    public class MovieController : Controller
    {
        public readonly DataContext _context;
        
        public MovieController(DataContext context)
        {
            _context = context;
        }


        // GET: MovieController
        public async Task<IActionResult> Index()
        {
            //var movies = _context.Movies
            //    .Include(x=>x.MovieDetail)
            //    .ToList();

            //var movies = await _context.Movies.ToListAsync();

            var movies = _context.Movies.ToList(); 

            var vm = new MovieIndexViewModel
            {
                Movies = movies
            };

            return View(vm);
        }

        // GET: MovieController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var movieDs = _context.MovieDetails.ToList();
           
            var movieDetail = _context.MovieDetails.Where(x => x.MovieId == id).FirstOrDefault();
            var movie = _context.Movies.Where(x => x.Id == id).FirstOrDefault();
            
            var movieViewModel = new MovieViewModel
            {
                Movie = movie,
                MovieDetail = movieDetail
            };
            return View(movieViewModel);
        }

        // GET: MovieController/Create
        public ActionResult Create()
        {
            //_context.Actors.Add(new Actor
            //{
            //    Id = 1,
            //    Name = "mike",
            //    Country = "USA",
            //    Gender = Gender.male
            //});

            var actor = _context.Actors.Single(x => x.Name == "mike");

            var movie = new Movie
            {
                Name = "Moon Walker",
                Type = MovieType.action,
                Language = "English",
                Actors = new List<Actor> { actor }
            };
            var moviedetail = new MovieDetail
            {
                Description = "annie are you ok?",
                Movie = movie
            };

            movie.MovieDetail = moviedetail;

            _context.AddRange(movie, moviedetail);
            ////context.Users.Add(user1);
            ////context.Actors.Add(actor);
            var count = _context.SaveChanges(); //在同一个事务里，针对它发生的变化，执行相应的sql语句，如果有一个执行失败就整体回滚
            //Console.WriteLine(count);

            return RedirectToAction(nameof(Index));
            //return View();
        }

        // POST: MovieController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {

                var actor = _context.Actors.Single(x => x.Name == "mike");


                var movie = new Movie
                {
                    Name = "Moon Walker",
                    Type = MovieType.action,
                    Language = "English",
                    Actors = new List<Actor> { actor }
                };
                var moviedetail = new MovieDetail
                {
                    Description = "annie are you ok?",
                    Movie = movie
                };

                movie.MovieDetail = moviedetail;

                _context.AddRange(movie, moviedetail);
                //context.Users.Add(user1);
                //context.Actors.Add(actor);
                var count = _context.SaveChanges(); //在同一个事务里，针对它发生的变化，执行相应的sql语句，如果有一个执行失败就整体回滚
                Console.WriteLine(count);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MovieController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MovieController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MovieController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MovieController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neverland.Data;
using Neverland.Domain;
using Neverland.Web.Utils;
using Neverland.Web.ViewModels;
using System.Net.NetworkInformation;

namespace Neverland.Web.Controllers
{

    //[Area("MovieController")]
    public class MovieController : Controller
    {
        public readonly DataContext _context;
        
        public MovieController(DataContext context)
        {
            _context = context;
        }


        // GET: MovieController
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var movies = _context.Movies
                .Include(x => x.MovieDetail)
                .ToList();

            //var movies = await _context.Movies.ToListAsync();
            //var movies = _context.Movies.ToListAsync();

            var vm = new MovieIndexViewModel
            {
                Movies = movies //Movies = movies.Result
            };

            return View(vm);
        }

        // GET: MovieController/Details/5
        [HttpGet]
        [MovieResourceFilterAttribute]
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            //var movieDs = _context.MovieDetails.ToListAsync();
           
            var movieDetail = _context.MovieDetails.Where(x => x.MovieId == id).FirstOrDefault();
            var movie = _context.Movies.Where(x => x.Id == id).FirstOrDefault();
            var movieScore = _context.MovieScores.Where(x=>x.MovieId == id).FirstOrDefault();
            if(movieScore == null)
            {
                movieScore = new MovieScore
                {
                    MovieId = movie.Id,
                    Score = 0.0
                };
            }
            
            var movieViewModel = new MovieViewModel
            {
                Movie = movie,
                MovieDetail = movieDetail,
                MovieScore = movieScore
            };

            HttpContext.Session.SetString("MovieID", movie.Id.ToString());
            return View(movieViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details (MovieViewModel movieViewModel)
        {
            Console.WriteLine("Modify movie score {0}", movieViewModel.MovieScore.Score);
            int movieID = Int32.Parse(HttpContext.Session.GetString("MovieID"));
            
            MovieScore movieScore = _context.MovieScores.Where(m=>m.MovieId==movieID).FirstOrDefault();
            if( movieScore == null)
            {
                _context.MovieScores.Add(new MovieScore
                {
                    Id = movieID,
                    Score = movieViewModel.MovieScore.Score
                });
            }
            else
            {
                movieScore.Score = movieViewModel.MovieScore.Score;
                _context.MovieScores.Update(movieScore);
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Details));
        }


        // GET: MovieController/Create
        [HttpGet]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<ActionResult> Create()
        {
            var actor = _context.Actors.Single(x => x.Name == "mike");

            var movie = new Movie
            {
                Name = "Moon Walker",
                Type = MovieType.action,
                Language = "English",
                Actors = new List<Actor> { new Actor {  Name = "mike",
                                                        Country = "USA",
                                                        Gender = Gender.male
                                                        } }
            };
            var moviedetail = new MovieDetail
            {
                Description = "annie are you ok?",
                Movie = movie
            };

            movie.MovieDetail = moviedetail;

            _context.AddRange(movie, moviedetail);
            //await _context.SaveChangesAsync(); //在同一个事务里，针对它发生的变化，执行相应的sql语句，如果有一个执行失败就整体回滚

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "admin")]
        public ActionResult Create(IFormCollection collction)
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
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<ActionResult> Edit(int id)
        {
            int failed = 0;
            var tasks = new List<Task>();
            String[] urls = { "www.adatum.com", "www.cohovineyard.com",
                        "www.cohowinery.com", "www.northwindtraders.com",
                        "www.contoso.com" };

            foreach(var url in urls)
            {
                tasks.Add(Task.Run(() =>
                {
                    var png = new Ping();
                    try
                    {
                        var reply = png.Send(url);
                        if(!(reply.Status == IPStatus.Success))
                        {
                            Interlocked.Increment(ref failed);
                            throw new TimeoutException($"unable to reach {url}");
                        }
                    }catch (Exception ex)
                    {
                        Interlocked.Increment(ref failed);
                    }
                }));
            }

            Task t = Task.WhenAll(tasks);

            try
            {
                t.Wait();
            }
            catch (Exception ex) { }

            if(t.Status == TaskStatus.RanToCompletion)
            {
                Console.WriteLine("All tasks finished");
            }else if(t.Status == TaskStatus.Faulted)
            {
                Console.WriteLine($"{failed} tasks failed");
            }

            return View();
        }

        // POST: MovieController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
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
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "admin")]
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

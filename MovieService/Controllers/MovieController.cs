using Microsoft.AspNetCore.Mvc;
using Neverland.Domain;
using Neverland.Data;
using Neverland.Domain.ViewModels;
using Microsoft.EntityFrameworkCore;
using MovieService.Utils;
using System.Net.NetworkInformation;

namespace MovieService.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class MovieController : ControllerBase
{
    private readonly ILogger<MovieController> _logger;
    private readonly DataContext _context;

    public MovieController(ILogger<MovieController> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }



    [HttpGet]
    public async Task<IEnumerable<Movie>> Index()
    {
        var movies = _context.Movies.ToListAsync().Result;

        return movies;
    }

    [HttpGet("{mid}")]
    public MovieViewModel QueryMovie(int mid)
    {
        var movie = _context.Movies.Where(m => m.Id == mid).FirstOrDefault();
        var movieDetail = _context.MovieDetails.Where(m => m.MovieId == mid).FirstOrDefault();
        var movieScore = _context.MovieScores.Where(m => m.MovieId == mid).FirstOrDefault();
        movieDetail.Movie = null;
        movieScore.Movie = null;
        movie.MovieDetail = movieDetail;
        movie.MovieScore = movieScore;


        MovieViewModel movieViewModel = new MovieViewModel
        {
            Movie = movie,
            MovieDetail = movie.MovieDetail,
            MovieScore = movie.MovieScore
        };
        return movieViewModel;
    }


    // GET: MovieController/Details/5
    [HttpGet("{mid}")]
    public MovieDetail GetDetail(int mid)
    {
        var movieDetail = _context.MovieDetails.Where(x => x.MovieId == mid).FirstOrDefault();
        return movieDetail;
    }

    //[HttpGet]
    [HttpGet("{id}")]
    public Movie GetMovie(int id)
    {
        Movie movie = _context.Movies.Where(x => x.Id==id).FirstOrDefault();
        var movieDetail = _context.MovieDetails.Where(x => x.MovieId == id).FirstOrDefault();
        movieDetail.Movie = null;

        var movieScore = _context.MovieScores.Where(m=>m.MovieId==id).FirstOrDefault(); // _context.MovieScores.Where(x => x.MovieId == id).FirstOrDefault();
        if (movieScore == null)
        {
            movieScore = new MovieScore
            {
                MovieId = movie.Id,
                Score = 0.0
            };
        }
        movie.MovieScore = movieScore;
        movieScore.Movie = null;
        return movie;
    }

    // GET: MovieController/Details/5
    [HttpGet("{id}")]
    [MovieResourceFilterAttribute]
    public async Task<MovieViewModel> GetMovieViewModel(int id)
    {
        //var movieDs = _context.MovieDetails.ToListAsync();

        Movie movie = _context.Movies.Where(x => x.Id == id).FirstOrDefault();
        var movieDetail = _context.MovieDetails.Where(x => x.MovieId == id).FirstOrDefault();
        
        var movieScore = _context.MovieScores.Where(m => m.MovieId == id).FirstOrDefault(); // _context.MovieScores.Where(x => x.MovieId == id).FirstOrDefault();
        movieDetail.Movie = null;
        movieScore.Movie = null;

        var movieViewModel = new MovieViewModel
        {
            Movie = movie,
            MovieDetail = movieDetail,
            MovieScore = movieScore
        };

        return movieViewModel;
    }

    [HttpPost]
    public async Task<IActionResult> Details(MovieViewModel movieViewModel)
    {
        Console.WriteLine("Modify movie score {0}", movieViewModel.MovieScore.Score);
        int movieID = Int32.Parse(HttpContext.Session.GetString("MovieID"));

        MovieScore movieScore = _context.MovieScores.Where(m => m.MovieId == movieID).FirstOrDefault();
        if (movieScore == null)
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
    public async Task<ActionResult> Creating()
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
        //context.Users.Add(user1);

        //await _context.SaveChangesAsync(); //在同一个事务里，针对它发生的变化，执行相应的sql语句，如果有一个执行失败就整体回滚
        //Console.WriteLine(count);

        return RedirectToAction(nameof(Index));
        //return View();
    }

    // POST: MovieController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<Movie> Create(IFormCollection collction)
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
            return movie;
        }
        catch
        {
            return null;
        }
    }

    // GET: MovieController/Edit/5
    [HttpGet]
    public Task<string> Edit(int id)
    {
        int failed = 0;
        var tasks = new List<Task>();
        String[] urls = { "www.baidu.com", "www.northwindtraders.com",
                        "www.contoso.com" };

        foreach (var url in urls)
        {
            tasks.Add(Task.Run(() =>
            {
                var png = new Ping();
                try
                {
                    var reply = png.Send(url);
                    if (!(reply.Status == IPStatus.Success))
                    {
                        Interlocked.Increment(ref failed);
                        throw new TimeoutException($"unable to reach {url}");
                    }
                }
                catch (Exception ex)
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

        if (t.Status == TaskStatus.RanToCompletion)
        {
            Console.WriteLine("All tasks finished");
            return Task.FromResult("All tasks finished");
        }
        else // (t.Status == TaskStatus.Faulted)
        {
            Console.WriteLine($"{failed} tasks failed");
            return Task.FromResult($"{failed} tasks failed");
        }

    }

    // POST: MovieController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public Movie Edit(Movie movie)
    {
        _context.Movies.Update(movie);
        _context.SaveChanges();
        return movie;
    }

    // POST: MovieController/Delete/5
    [HttpPost]
    public async Task<String> Delete(int id)
    {
        var movie = _context.Movies.Where(m => m.Id == id).FirstOrDefaultAsync().Result;
        _context.Remove(movie);
        _context.SaveChanges();
        return $"{movie.Id} {movie.Name} deleted";
    }
}


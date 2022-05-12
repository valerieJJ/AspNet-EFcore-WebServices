using Microsoft.AspNetCore.Mvc;
using Neverland.Domain;
using Neverland.Data;
using Neverland.Web.ViewModels;

namespace MovieService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
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
    private IEnumerable<Movie> GetMovies()
    {
        var movies = _context.Movies.ToArray();
        return movies;
    }

    [HttpGet]
    private MovieViewModel QueryMovie(int mid)
    {
        var movie = _context.Movies.Where(m => m.Id == mid).FirstOrDefault();
        var movieDetail = _context.MovieDetails.Where(m => m.MovieId == mid).FirstOrDefault();
        var movieScore = _context.MovieScores.Where(m => m.MovieId == mid).FirstOrDefault();
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
}


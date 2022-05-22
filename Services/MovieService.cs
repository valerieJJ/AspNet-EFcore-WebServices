using Interface;
using Neverland.Domain.ViewModels;

namespace Services;
public class MovieServiceImp : IMovieService
{
    public string QueryMovie(int mid)
    {
        return $"get service: movieid={mid}";
    }
}


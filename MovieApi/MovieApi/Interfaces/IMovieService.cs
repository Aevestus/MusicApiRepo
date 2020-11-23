using MovieApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApi.Interfaces
{
    public interface IMovieService
    {
        Task<MovieMetadata> SaveMovieMetaData(MovieMetadata movieData);
        Task<List<MovieMetadata>> GetMovieMetadata(int movieId);
        Task<dynamic> GetAllStats();
    }
}

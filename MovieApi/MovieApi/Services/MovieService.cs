using MovieApi.Interfaces;
using MovieApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MovieApi.Services
{
    //This Database prop will simulate a storage
    //Service methods are tasks because a storage is a 3rd party
    public class MovieService : IMovieService
    {
        private IEnumerable<MovieMetadata> Database { get; set; } = new List<MovieMetadata>();

        public MovieService()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\metadata.csv");

            this.Database = File.ReadAllLines(path)
                                .Skip(1)
                                .Select(v => MovieMetadata.FromCsv(v))
                                .ToList();
        }

        //Normally this would be a DTO, but for simplicity, we've used dynamic
        public async Task<dynamic> GetAllStats()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\stats.csv");
            List<MovieStats> stats = GetOrderedMovieStats(path);

            //Process analtics, like average watch duration, watch count, for each movie
            var watches = stats.GroupBy(x => x.Id).Select(x => new
            {
                MovieId = x.FirstOrDefault().Id,
                Count = x.Count(),
                AvgDurationS = Enumerable.Average(x.Select(y => y.WatchDurationMs)) / 1000
            });

            //extract ids for easier usage
            var movieIds = watches.Select(x => x.MovieId).ToList();

            //process the aggregate between movie metadata fields and watches 
            var processedMetadataAndStats = Database
                .Where(x => movieIds.Contains(x.Id.Value))
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    AverageWatchDurationS = watches.FirstOrDefault(y => y.MovieId == x.Id.Value).AvgDurationS,
                    Watches = watches.FirstOrDefault(y => y.MovieId == x.Id.Value).Count,
                    x.ReleaseYear
                });

            //Because a movie can be in different languages, we take the most watched language version
            //We cannot aggregate here the watches of different languages into 1, because we have a different movie record 
            //for each provided ID. 
            var finalStats = processedMetadataAndStats
                .GroupBy(x => x.Title)
                .Select(x => x.OrderByDescending(y=>y.AverageWatchDurationS).FirstOrDefault());

            return await Task.FromResult(finalStats);
        }

        private static List<MovieStats> GetOrderedMovieStats(string path)
        {
            return File.ReadAllLines(path)
                            .Skip(1)
                            .Select(v => MovieStats.FromCsv(v))
                            .OrderByDescending(x => x.WatchDurationMs)
                            .ToList();
        }

        public async Task<List<MovieMetadata>> GetMovieMetadata(int movieId)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\metadata.csv");


            var filteredItems = Database.Where(x => x.MovieId == movieId)
                                        .Where(v => v.AllFieldsHaveData())
                                        .OrderBy(x => x.Language)
                                        .GroupBy(x=>x.Language)
                                        .Select(x => x.OrderByDescending(y=>y.Id).FirstOrDefault())
                                        .ToList();

            return await Task.FromResult(filteredItems);
        }

        public async Task<MovieMetadata> SaveMovieMetaData(MovieMetadata movieData)
        {
            this.Database.Append(movieData);
            return await Task.FromResult(movieData);
        }
    }
}

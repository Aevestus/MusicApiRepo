using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Models
{
    public class MovieMetadata
    {
        public int? Id { get; set; }
        public int? MovieId { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string Duration { get; set; }
        public int? ReleaseYear { get; set; }

        public static MovieMetadata FromCsv(string csvLine)
        {
            try
            {
                string[] values = csvLine.Split(',');
                int.TryParse(values[0], out int id);
                int.TryParse(values[1], out int movieId);
                int.TryParse(values[5], out int year);

                var movieMetadata = new MovieMetadata
                {
                    Id = id,
                    MovieId = movieId,
                    Title = values[2],
                    Language = values[3],
                    Duration = values[4],
                    ReleaseYear = year,
                };
                return movieMetadata;
            }
            catch(Exception e)
            {
                throw e;
            }
            
        }

        public bool AllFieldsHaveData()
        {
            return Id != null && 
                MovieId != null && 
                Title != null && 
                Language != null && 
                Duration != null && 
                ReleaseYear != null;
        }
    }
}

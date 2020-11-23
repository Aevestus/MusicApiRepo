using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Models
{
    public class MovieStats
    {
        public int Id { get; set; }
        public float WatchDurationMs { get; set; }

        public static MovieStats FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            var stats = new MovieStats
            {
                Id = Convert.ToInt32(values[0]),
                WatchDurationMs = float.Parse(values[1])
            };
            return stats;
        }
    }
}

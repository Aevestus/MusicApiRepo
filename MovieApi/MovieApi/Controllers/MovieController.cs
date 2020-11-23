using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieApi.Interfaces;
using MovieApi.Models;

namespace MovieApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetadataController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MetadataController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("{movieId}")]
        public async Task<IActionResult> GetMovieMetadata(int movieId)
        {
            var result = await _movieService.GetMovieMetadata(movieId);
            return result == null || !result.Any() ? NotFound() : (IActionResult)Ok(result);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetAllStats()
        {
            return Ok(await _movieService.GetAllStats());
        }

        [HttpPost]
        public async Task<IActionResult> SaveMovieMetadata(MovieMetadata movieData)
        {
            return Ok(await _movieService.SaveMovieMetaData(movieData));
        }
    }
}

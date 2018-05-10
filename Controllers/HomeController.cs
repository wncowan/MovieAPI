using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace MovieAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbConnector _dbConnector;
        private readonly IOptions<MyApiOptions> MyApiConfig;
        public HomeController(DbConnector connect, IOptions<MyApiOptions> apiConfig)
        {
            MyApiConfig = apiConfig;
            _dbConnector = connect;
        }
        [HttpGet]
        [Route("/search")]
        public IActionResult Search(string search) {
            Console.WriteLine("Entered Search");
            Console.WriteLine(search);
            JObject searchData = new JObject();
            string ApiKey = "6e14bc60883eb3a095408096bbdaf061";
            string url = $"https://api.themoviedb.org/3/search/movie?api_key={ApiKey}&language=en-US&query={search}&page=1&include_adult=false";
            WebRequest.UseMovieApi(url, response => {
                searchData = response;
            }).Wait();
            // Console.WriteLine(searchData);
            JToken firstMovie = new JObject();
            try {
                firstMovie = searchData["results"][0];
            }
            catch {
                return Json(new {error="No result found."});
            }
            string title = firstMovie["title"].ToString();
            string released;
            string rating;
            if (firstMovie["release_date"].ToString().Length > 0) {
                released = firstMovie["release_date"].ToString();
            } else {
                released = "none";
            }
            if (firstMovie["vote_average"].ToString().Length > 0) {
                rating = firstMovie["vote_average"].ToString();
            } else {
                rating = "none";
            }
            string query = $"INSERT INTO movies (title, released, rating) VALUES ('{title}','{released}','{rating}')";
            _dbConnector.Execute(query);
            return Json(_dbConnector.Query("SELECT * FROM movies ORDER BY created_at DESC"));
        }
        public IActionResult Index()
        {
            ViewData["Title"] = "Main";

            return View();
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
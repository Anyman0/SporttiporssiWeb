using Microsoft.AspNetCore.Mvc;
using SporttiporssiWeb.Interfaces;
using SporttiporssiWeb.Models;
using System.Diagnostics;

namespace SporttiporssiWeb.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GamesController _gamesController;
        public HomeController(ILogger<HomeController> logger, ISeriesService seriesService, GamesController gamesController) : base(seriesService)
        {
            _logger = logger;
            _gamesController = gamesController;
        }

        public async Task<IActionResult> Index(string sport)
        {
            IEnumerable<Game> games = null;
            if(sport == "Hockey")
            {
                games = await _gamesController.GetHockeyGames();
                return View("HomePage_Hockey", games);
            }
            else
            {
                var sportFromCookie = HttpContext.Request.Cookies["sport"];
                if (!string.IsNullOrEmpty(sportFromCookie))
                {
                    if(sportFromCookie == "Hockey")
                    {
                        games = await _gamesController.GetHockeyGames();
                    }
                    return View($"HomePage_{sportFromCookie}", games);
                }
                else
                {
                    return View("Index");
                }               
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

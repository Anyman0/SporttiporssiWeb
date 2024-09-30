using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SporttiporssiWeb.Interfaces;
using System.Reflection;

namespace SporttiporssiWeb.Controllers
{
    public class SeriesController : BaseController
    {
        private readonly ISeriesService _seriesService;

        public SeriesController(ISeriesService seriesService) : base(seriesService)
        {
            _seriesService = seriesService;
        }

        [HttpPost]
        public IActionResult SetSeries(string selectedSeries)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            if (!string.IsNullOrEmpty(selectedSeries))
            {
                // store serie in cookie, so user doesn't need to choose it each time coming to the page
                Response.Cookies.Append("serie", selectedSeries, cookieOptions);
                HttpContext.Session.SetString("SelectedSeries", selectedSeries);
            }
            // Indicates success without redirection
            return NoContent();
        }

        [HttpPost]
        public IActionResult SetSport(string selectedSport)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            if (!string.IsNullOrEmpty(selectedSport))
            {
                // store sport in cookie, so user doesn't need to choose it each time coming to the page
                Response.Cookies.Append("sport", selectedSport, cookieOptions);
                HttpContext.Session.SetString("SelectedSport", selectedSport);
                HttpContext.Session.Remove("SelectedSeries");
            }
            return RedirectToAction("Index", "Home", new { sport = selectedSport });
        }

    }
}

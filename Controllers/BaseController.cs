using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SporttiporssiWeb.Interfaces;

namespace SporttiporssiWeb.Controllers
{
    public class BaseController : Controller
    {
        private readonly ISeriesService _seriesService;
        
        public BaseController(ISeriesService seriesService)
        {
            _seriesService = seriesService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sportListTask = _seriesService.GetSportListAsync();
            sportListTask.Wait();
            ViewBag.SportList = sportListTask.Result;      
            var selectedSport = HttpContext.Request.Cookies["sport"];
            var seriesListTask = _seriesService.GetSeriesListAsync(selectedSport);
            seriesListTask.Wait();
            ViewBag.SeriesList = seriesListTask.Result;
            base.OnActionExecuting(context);
        }
    }
}

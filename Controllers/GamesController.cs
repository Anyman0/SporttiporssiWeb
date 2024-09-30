using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SporttiporssiWeb.Configurations;
using SporttiporssiWeb.Interfaces;
using SporttiporssiWeb.Models;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace SporttiporssiWeb.Controllers
{
    public class GamesController : BaseController
    {
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GamesController(IOptions<ApiSettings> apiSettings, HttpClient httpClient, ISeriesService seriesService, IHttpContextAccessor httpContextAccessor) : base(seriesService)
        {
            _apiBaseUrl = apiSettings.Value.DevBaseUrl;
            //_httpClient = httpClient;
            var unsafeHttpClient = new UnsafeHttpClientHandler();
            _httpClient = new HttpClient(unsafeHttpClient);
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Game>> GetHockeyGames()
        {
            var authToken = _httpContextAccessor.HttpContext?.Request.Cookies["auth_token"];
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Retrieve selected date from the session, default to today if not set
            var contextDate = _httpContextAccessor.HttpContext?.Session.GetString("SelectedDate");
            DateTime date;

            if (string.IsNullOrEmpty(contextDate))
            {
                date = DateTime.UtcNow.AddDays(1).Date;
                _httpContextAccessor.HttpContext?.Session.SetString("SelectedDate", date.ToString("yyyy-MM-dd"));
            }
            else
            {
                date = DateTime.Parse(contextDate);
            }

            var selectedSeries = _httpContextAccessor.HttpContext?.Request.Cookies["serie"];
            if (selectedSeries == null)
            {
                return new List<Game>();
            }
            var formattedDate = date.ToString("MM.dd.yyyy");
            try
            {
                Debug.WriteLine($"Making api request to {_apiBaseUrl}Games?date={formattedDate}&serie={selectedSeries}");
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}Games?date={formattedDate}&serie={selectedSeries}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var games = JsonConvert.DeserializeObject<List<Game>>(json);

                ViewBag.Date = date.ToString("yyyy-MM-dd");
                return games;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching games: {ex.Message}");
                return new List<Game>();
            }
        }
    }
}

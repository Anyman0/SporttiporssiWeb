using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using SporttiporssiWeb.Configurations;
using SporttiporssiWeb.Interfaces;
using SporttiporssiWeb.Models;
using System.Diagnostics;

namespace SporttiporssiWeb.Controllers
{
    public class UserController : BaseController
    {
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;
        private ISeriesService _seriesService;

        public UserController(IOptions<ApiSettings> apiSettings, HttpClient httpClient, ISeriesService seriesService) : base(seriesService)
        {
            _apiBaseUrl = apiSettings.Value.DevBaseUrl;
            _seriesService = seriesService;
            //_httpClient = httpClient;
            var unsafeHttpClient = new UnsafeHttpClientHandler();
            _httpClient = new HttpClient(unsafeHttpClient);
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var request = new { email = model.Username, password = model.Password };
                Debug.WriteLine($"Making API call to: {_apiBaseUrl}User/login", request);
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}User/login", request);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    var token = result.Token;
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(30)
                    };
                    // Store token in cookie
                    Response.Cookies.Append("auth_token", token, cookieOptions);
                    // store email to display for user
                    Response.Cookies.Append("user", model.Username, cookieOptions);
                    return RedirectToAction("Index", "Home");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    ModelState.AddModelError("Login failed", "Forbidden");
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("Login failed", "Invalid email or password");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in login: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Server error. Please try again.");
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("auth_token");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}

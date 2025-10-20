using System.Diagnostics;
using group_13_YenTing_Favour__Lab_3.Models;
using group_13_YenTing_Favour__Lab_3.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace group_13_YenTing_Favour__Lab_3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Lab3Context _db;

  
        public HomeController(ILogger<HomeController> logger, Lab3Context db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? filter)
        {
            var viewModel = new HomeViewModel();
            searchTerm = searchTerm?.Trim().ToLower();

            // ?? 1. Popular episodes (top 5)
            viewModel.MostPopularEpisodes = await _db.Episodes
                .Include(e => e.Podcast)
                .OrderByDescending(e => e.NumberOfViews)
                .Take(5)
                .ToListAsync();

            // ?? 2. Recent episodes (latest 5)
            viewModel.RecentEpisodes = await _db.Episodes
                .Include(e => e.Podcast)
                .OrderByDescending(e => e.ReleaseDate)
                .Take(5)
                .ToListAsync();

            // ?? 3. Search podcasts by topic or host
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var query = _db.Podcasts
                    .Include(p => p.Creator)
                    .Where(p => (p.Title.ToLower().Contains(searchTerm) ||
                                 p.Description.ToLower().Contains(searchTerm)));

                // If filtering by "host"
                if (filter == "host")
                {
                    query = _db.Podcasts
                        .Include(p => p.Creator)
                        .Where(p => p.Creator.UserName.ToLower().Contains(searchTerm));
                }

                viewModel.SearchResults = await query.ToListAsync();
                viewModel.SearchTerm = searchTerm;
            }

            return View(viewModel);
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

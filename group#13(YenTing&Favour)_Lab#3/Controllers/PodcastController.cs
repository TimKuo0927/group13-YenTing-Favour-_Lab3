using group_13_YenTing_Favour__Lab_3.Models;
using group_13_YenTing_Favour__Lab_3.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace group_13_YenTing_Favour__Lab_3.Controllers
{
    public class PodcastController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
            private readonly Lab3Context _db;
        public PodcastController(UserManager<IdentityUser> userManager,Lab3Context db)
        {
            _userManager = userManager;
            _db = db;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserPodcast()
        {

            var userIdString = _userManager.GetUserId(User);
            if (String.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }
            var podcasts = await _db.Podcasts
           .Where(p => p.CreatorId == userIdString && (p.IsHidden == false || p.IsHidden == null))
           .OrderByDescending(p => p.CreatedDate)
           .ToListAsync();
            return View(podcasts);
            
            
        }

        [Authorize]
        [HttpGet]
        public IActionResult CreatePodcastPage()
        {
            Podcast podcast = new Podcast();
            podcast.CreatedDate = DateTime.Now;
            podcast.IsHidden = false;
            podcast.CreatorId = _userManager.GetUserId(User) != null ? _userManager.GetUserId(User) : "";
            return View(podcast);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePodcast(Podcast podcast)
        {
            podcast.CreatedDate = DateTime.Now;
            podcast.IsHidden = false;
            podcast.CreatorId = _userManager.GetUserId(User) != null ? _userManager.GetUserId(User) : "";

            await _db.Podcasts.AddAsync(podcast);
            await _db.SaveChangesAsync();
            return RedirectToAction("UserPodcast");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PodcastDetailPage(int PodcastId)
        {

            var userIdString = _userManager.GetUserId(User);
            if (String.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }
            var podcasts = await _db.Podcasts
                            .Where(p => p.CreatorId == userIdString
                                        && p.PodcastId == PodcastId
                                        && (p.IsHidden == false || p.IsHidden == null))
                            .Include(p => p.Episodes)
                            .FirstOrDefaultAsync();
            return View(podcasts);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditPodcastPage(int PodcastId)
        {

            var userIdString = _userManager.GetUserId(User);
            if (String.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }
            var podcasts = await _db.Podcasts
                            .Where(p => p.CreatorId == userIdString
                                        && p.PodcastId == PodcastId
                                        && (p.IsHidden == false || p.IsHidden == null))
                            .FirstOrDefaultAsync();
            return View(podcasts);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditPodcast(Podcast model)
        {

            var userIdString = _userManager.GetUserId(User);
            if (String.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            var podcast =  _db.Podcasts.Where(x=>x.PodcastId==model.PodcastId).FirstOrDefault();
            if (podcast == null || podcast.CreatorId != userIdString)
            {
                return Unauthorized();
            }

            // Update only editable fields
            podcast.Title = model.Title;
            podcast.Description = model.Description;

            await _db.SaveChangesAsync(); // This will generate UPDATE
            return RedirectToAction("UserPodcast");
        }

    }
}

using Amazon.S3;
using Amazon.S3.Model;
using group_13_YenTing_Favour__Lab_3.Models;
using group_13_YenTing_Favour__Lab_3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace group_13_YenTing_Favour__Lab_3.Controllers
{
    public class EpisodeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Lab3Context _db;
        public EpisodeController(UserManager<IdentityUser> userManager, Lab3Context db)
        {
            _userManager = userManager;
            _db = db;
        }

        [Authorize]
        [HttpGet]
        public IActionResult CreateEposidePage(int PodcastId)
        {

            var userIdString = _userManager.GetUserId(User);
            if (String.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }
            Episode episode = new Episode();
            episode.PodcastId = PodcastId;
            return View(episode);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEpisode(Episode episode, IFormFile VideoFile)
        {
            Console.WriteLine("PodcastId: " + episode.PodcastId);
            if (VideoFile == null || VideoFile.Length == 0)
            {
                ModelState.AddModelError("", "Please provide a video file.");
                return View(episode);
            }

            // 1. Get the username
            var userName = User.Identity?.Name ?? "unknown";

            // 2. Generate a unique filename
            var fileExtension = Path.GetExtension(VideoFile.FileName);
            var fileKey = $"{userName}/{Guid.NewGuid()}{fileExtension}";

            // 3. Upload to S3
            using (var stream = VideoFile.OpenReadStream())
            {
                var putRequest = new Amazon.S3.Model.PutObjectRequest
                {
                    BucketName = AWSUtil.bucketName,
                    Key = fileKey,
                    InputStream = stream,
                    ContentType = VideoFile.ContentType,
                };

                await AWSUtil.s3Client.PutObjectAsync(putRequest);
            }

            // 4. Save Episode info to DB
            episode.AudioFileUrl = fileKey;
            episode.PlayCount = 0;
            episode.ReleaseDate = DateTime.UtcNow;
            episode.Duration = VideoFile.Length; // assuming duration is in bytes for simplicity
            episode.NumberOfViews = 0;

            await _db.Episodes.AddAsync(episode);
            await _db.SaveChangesAsync();

            return RedirectToAction("PodcastDetailPage", "Podcast",new { PodcastId = episode.PodcastId }); 
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EposideDetailPage(int EpisodeId)
        {

            var userIdString = _userManager.GetUserId(User);
            if (String.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }
            var episode = await _db.Episodes
               .Include(e => e.Podcast)
               .FirstOrDefaultAsync(e => e.EpisodeId == EpisodeId);

            var request = new GetPreSignedUrlRequest
            {
                BucketName = AWSUtil.bucketName,
                Key = episode.AudioFileUrl,
                Expires = DateTime.UtcNow.AddHours(1)
            };
            var url = AWSUtil.s3Client.GetPreSignedURL(request);
            episode.AudioFileUrl = url;
            return View(episode);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditEpisodePage(int EpisodeId)
        {
            var userIdString = _userManager.GetUserId(User);
            if (String.IsNullOrEmpty(userIdString))
                return RedirectToAction("Login", "Account");

            var episode = await _db.Episodes
                .Include(e => e.Podcast)
                .FirstOrDefaultAsync(e => e.EpisodeId == EpisodeId);


            // only generate pre-signed URL if we have a stored key
            if (!string.IsNullOrEmpty(episode.AudioFileUrl))
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = AWSUtil.bucketName,
                    Key = episode.AudioFileUrl, // this is the S3 key
                    Expires = DateTime.UtcNow.AddHours(1)
                };

                var presignedUrl = AWSUtil.s3Client.GetPreSignedURL(request);
                ViewBag.VideoUrl = presignedUrl; // store temporary URL separately
            }

            return View(episode);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEpisode(Episode episode, IFormFile? VideoFile)
        {
            var existing = await _db.Episodes.FindAsync(episode.EpisodeId);
            if (existing == null)
                return NotFound();

            existing.Title = episode.Title;

            // if new file uploaded, replace in S3
            if (VideoFile != null && VideoFile.Length > 0)
            {
                var userName = User.Identity?.Name ?? "unknown";
                var fileExtension = Path.GetExtension(VideoFile.FileName);
                var fileKey = $"{userName}/{Guid.NewGuid()}{fileExtension}";

                await AWSUtil.s3Client.DeleteObjectAsync(new Amazon.S3.Model.DeleteObjectRequest
                {
                    BucketName = AWSUtil.bucketName,
                    Key = existing.AudioFileUrl
                });

                using var stream = VideoFile.OpenReadStream();
                var putRequest = new Amazon.S3.Model.PutObjectRequest
                {
                    BucketName = AWSUtil.bucketName,
                    Key = fileKey,
                    InputStream = stream,
                    ContentType = VideoFile.ContentType,
                };
                await AWSUtil.s3Client.PutObjectAsync(putRequest);

                existing.AudioFileUrl = fileKey; // store key
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("PodcastDetailPage", "Podcast", new { PodcastId = existing.PodcastId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteEpisode(int EpisodeId)
        {
            var userIdString = _userManager.GetUserId(User);
            if (String.IsNullOrEmpty(userIdString)) {
                return RedirectToAction("Login", "Account");
            }
                

            
            var existing = await _db.Episodes
                .Include(e => e.Podcast)
                .FirstOrDefaultAsync(e => e.EpisodeId == EpisodeId);

            if(existing.Podcast.CreatorId != userIdString)
            {
                return RedirectToAction("Login", "Account");
            }

            await AWSUtil.s3Client.DeleteObjectAsync(new Amazon.S3.Model.DeleteObjectRequest
            {
                BucketName = AWSUtil.bucketName,
                Key = existing.AudioFileUrl
            });
            _db.Remove(existing);
            await _db.SaveChangesAsync();
            return RedirectToAction("PodcastDetailPage", "Podcast", new { PodcastId = existing.PodcastId });
        }

    }
}

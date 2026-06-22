using JobBoard.Models.DTOs.Applications;
using JobBoard.Models.Entities;
using JobBoard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace JobBoard.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly IApplicationService _appService;
        private readonly IJobService _jobService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ApplicationsController(IApplicationService appService,
            IJobService jobService, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _appService = appService;
            _jobService = jobService;
            _userManager = userManager;
            _env = env;
        }

        [HttpPost]
        [Authorize(Roles = "Candidate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int jobId, ApplyDto dto)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Details", "Jobs", new { id = jobId });

            var userId = _userManager.GetUserId(User)!;
            string relativePath = "";
            string originalFileName = "";

            if (dto.UseProfileCv)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();

                if (string.IsNullOrEmpty(user.CvPath) || string.IsNullOrEmpty(user.CvFileName))
                {
                    TempData["Error"] = "You do not have a default CV uploaded in your profile. Please upload one first or choose to upload a file.";
                    return RedirectToAction("Details", "Jobs", new { id = jobId });
                }

                relativePath = user.CvPath;
                originalFileName = user.CvFileName;
            }
            else
            {
                if (dto.CvFile == null || dto.CvFile.Length == 0)
                {
                    TempData["Error"] = "Please upload your CV.";
                    return RedirectToAction("Details", "Jobs", new { id = jobId });
                }

                var extension = Path.GetExtension(dto.CvFile.FileName).ToLowerInvariant();
                if (extension != ".pdf" && extension != ".docx")
                {
                    TempData["Error"] = "Only PDF and DOCX files are allowed.";
                    return RedirectToAction("Details", "Jobs", new { id = jobId });
                }

                if (dto.CvFile.Length > 1024 * 1024)
                {
                    TempData["Error"] = "CV file size cannot exceed 1MB.";
                    return RedirectToAction("Details", "Jobs", new { id = jobId });
                }

                try
                {
                    // Ensure uploads directory exists
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "cvs");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.CvFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.CvFile.CopyToAsync(fileStream);
                    }

                    relativePath = $"/uploads/cvs/{uniqueFileName}";
                    originalFileName = dto.CvFile.FileName;
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"File upload error: {ex.Message}";
                    return RedirectToAction("Details", "Jobs", new { id = jobId });
                }
            }

            try
            {
                await _appService.ApplyAsync(dto, userId, jobId, relativePath, originalFileName);
                TempData["Success"] = "Application submitted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", "Jobs", new { id = jobId });
        }

        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> MyApplications()
        {
            var userId = _userManager.GetUserId(User)!;
            var apps = await _appService.GetCandidateAppsAsync(userId);
            return View(apps);
        }

        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Applicants(int id)
        {
            var job = await _jobService.GetByIdAsync(id);
            if (job == null) return NotFound();

            var userId = _userManager.GetUserId(User)!;
            if (job.EmployerId != userId) return Forbid();

            var applicants = await _appService.GetJobApplicantsAsync(id);
            ViewBag.JobTitle = job.Title;
            ViewBag.JobId = id;
            return View(applicants);
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, ApplicationStatus status, int jobId)
        {
            await _appService.UpdateStatusAsync(id, status);
            TempData["Success"] = "Application status updated.";
            return RedirectToAction(nameof(Applicants), new { id = jobId });
        }
    }
}

using JobBoard.Data;
using JobBoard.Models.DTOs.Jobs;
using JobBoard.Models.Entities;
using JobBoard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Controllers
{
    public class JobsController : Controller
    {
        private readonly IJobService _jobService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;

        public JobsController(IJobService jobService, UserManager<ApplicationUser> userManager, AppDbContext db)
        {
            _jobService = jobService;
            _userManager = userManager;
            _db = db;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search, int? categoryId, JobType? type, int page = 1)
        {
            var filter = new JobFilterDto
            {
                Search = search,
                CategoryId = categoryId,
                Type = type,
                Page = page,
                PageSize = 10
            };

            var result = await _jobService.GetJobsAsync(filter);
            var categories = await _db.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);
            return View(result);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var job = await _jobService.GetByIdAsync(id);
            if (job == null) return NotFound();

            // Check if current candidate already applied
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Candidate"))
            {
                var userId = _userManager.GetUserId(User)!;
                var applied = await _db.JobApplications
                    .AnyAsync(a => a.CandidateId == userId && a.JobId == id);
                ViewBag.HasApplied = applied;

                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    ViewBag.DefaultCvPath = user.CvPath;
                    ViewBag.DefaultCvFileName = user.CvFileName;
                }
            }

            return View(job);
        }

        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Create()
        {
            var categories = await _db.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateJobDto dto)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _db.Categories.ToListAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", dto.CategoryId);
                return View(dto);
            }

            var userId = _userManager.GetUserId(User)!;
            var id = await _jobService.CreateJobAsync(dto, userId);
            TempData["Success"] = "Job listing created successfully!";
            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Edit(int id)
        {
            var job = await _jobService.GetByIdAsync(id);
            if (job == null) return NotFound();

            var userId = _userManager.GetUserId(User)!;
            if (job.EmployerId != userId) return Forbid();

            var categories = await _db.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", job.CategoryId);

            var dto = new UpdateJobDto
            {
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                SalaryRange = job.SalaryRange,
                Type = job.Type,
                Status = job.Status,
                CategoryId = job.CategoryId
            };

            ViewBag.JobId = id;
            return View(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateJobDto dto)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _db.Categories.ToListAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", dto.CategoryId);
                ViewBag.JobId = id;
                return View(dto);
            }

            var job = await _jobService.GetByIdAsync(id);
            if (job == null) return NotFound();

            var userId = _userManager.GetUserId(User)!;
            if (job.EmployerId != userId) return Forbid();

            await _jobService.UpdateJobAsync(id, dto);
            TempData["Success"] = "Job updated successfully!";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var job = await _jobService.GetByIdAsync(id);
            if (job == null) return NotFound();

            var userId = _userManager.GetUserId(User)!;
            if (job.EmployerId != userId) return Forbid();

            await _jobService.DeleteJobAsync(id);
            TempData["Success"] = "Job listing deleted.";
            return RedirectToAction(nameof(MyJobs));
        }

        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> MyJobs()
        {
            var userId = _userManager.GetUserId(User)!;
            var jobs = await _jobService.GetEmployerJobsAsync(userId);
            return View(jobs);
        }
    }
}

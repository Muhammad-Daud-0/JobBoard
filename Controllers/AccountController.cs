using JobBoard.Models.DTOs.Auth;
using JobBoard.Models.DTOs.Account;
using JobBoard.Models.Entities;
using JobBoard.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JobBoard.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _uow;
        private readonly IWebHostEnvironment _env;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IUnitOfWork uow, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _uow = uow;
            _env = env;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            if (dto.Role == "Employer" && string.IsNullOrWhiteSpace(dto.CompanyName))
            {
                ModelState.AddModelError("CompanyName", "Company name is required for employers");
                return View(dto);
            }

            // Check duplicate username
            var existingUser = await _userManager.FindByNameAsync(dto.UserName);
            if (existingUser != null)
            {
                ModelState.AddModelError("UserName", "Username is already taken by another account.");
                return View(dto);
            }

            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.UserName,
                Address = dto.Address,
                CompanyName = dto.CompanyName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(dto);
            }

            await _userManager.AddToRoleAsync(user, dto.Role);
            await _signInManager.SignInAsync(user, isPersistent: false);
            TempData["Success"] = $"Welcome, {user.FullName}! Your account has been created.";
            return RedirectToAction("Index", "Jobs");
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(dto);

            // Find user by email first, then sign in using their UserName
            var userByEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (userByEmail == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(dto);
            }

            var result = await _signInManager.PasswordSignInAsync(
                userByEmail.UserName!, dto.Password, dto.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(dto);
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Jobs");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Jobs");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Role = roles.FirstOrDefault() ?? "User";
            ViewBag.ChangePasswordDto = new ChangePasswordDto();

            if (User.IsInRole("Candidate"))
            {
                var apps = await _uow.Applications.GetCandidateApplicationsAsync(user.Id);
                ViewBag.TotalApplications = apps.Count();
                ViewBag.PendingApplications = apps.Count(a => a.Status == ApplicationStatus.Pending);
                ViewBag.ShortlistedApplications = apps.Count(a => a.Status == ApplicationStatus.Shortlisted);
            }
            else if (User.IsInRole("Employer"))
            {
                var jobs = await _uow.Jobs.GetByEmployerAsync(user.Id);
                ViewBag.TotalJobs = jobs.Count();
                ViewBag.ActiveJobs = jobs.Count(j => j.Status == JobStatus.Open);
            }

            var model = new UpdateProfileDto
            {
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                ProfileImageUrl = user.ProfileImageUrl,
                CompanyName = user.CompanyName,
                Bio = user.Bio,
                Skills = user.Skills,
                CvPath = user.CvPath,
                CvFileName = user.CvFileName,
                CompanyDescription = user.CompanyDescription,
                WebsiteUrl = user.WebsiteUrl,
                Location = user.Location
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Failed to update profile. Please verify your details.";
                return RedirectToAction(nameof(Profile));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (User.IsInRole("Employer") && string.IsNullOrWhiteSpace(dto.CompanyName))
            {
                TempData["Error"] = "Company Name is required for Employers.";
                return RedirectToAction(nameof(Profile));
            }

            // Check if username changed and is unique
            if (!string.IsNullOrWhiteSpace(dto.UserName) && user.UserName != dto.UserName)
            {
                var existingUserName = await _userManager.FindByNameAsync(dto.UserName);
                if (existingUserName != null)
                {
                    TempData["Error"] = "Username is already taken by another account.";
                    return RedirectToAction(nameof(Profile));
                }

                user.UserName = dto.UserName;
                user.NormalizedUserName = dto.UserName.ToUpperInvariant();
            }

            // Check if email changed and is taken
            if (!string.IsNullOrWhiteSpace(dto.Email) && user.Email != dto.Email)
            {
                var existingEmailUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingEmailUser != null)
                {
                    TempData["Error"] = "Email address is already in use by another account.";
                    return RedirectToAction(nameof(Profile));
                }

                user.Email = dto.Email;
                user.NormalizedEmail = dto.Email.ToUpperInvariant();
            }

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                user.FullName = dto.FullName;
            }

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                user.PhoneNumber = dto.PhoneNumber;
            }

            if (!string.IsNullOrWhiteSpace(dto.Address))
            {
                user.Address = dto.Address;
            }

            // Handle Profile Image Upload
            if (dto.ProfileImageFile != null && dto.ProfileImageFile.Length > 0)
            {
                var imgExtension = Path.GetExtension(dto.ProfileImageFile.FileName).ToLowerInvariant();
                if (imgExtension != ".jpg" && imgExtension != ".jpeg" && imgExtension != ".png")
                {
                    TempData["Error"] = "Only JPG, JPEG, and PNG files are allowed for profile image.";
                    return RedirectToAction(nameof(Profile));
                }

                if (dto.ProfileImageFile.Length > 1024 * 1024)
                {
                    TempData["Error"] = "Profile image file size cannot exceed 1MB.";
                    return RedirectToAction(nameof(Profile));
                }

                try
                {
                    var avatarsFolder = Path.Combine(_env.WebRootPath, "uploads", "avatars");
                    if (!Directory.Exists(avatarsFolder))
                    {
                        Directory.CreateDirectory(avatarsFolder);
                    }

                    // Delete old profile image if exists
                    if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                    {
                        var oldImgPath = Path.Combine(_env.WebRootPath, user.ProfileImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }

                    var uniqueImgName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.ProfileImageFile.FileName)}";
                    var imgPath = Path.Combine(avatarsFolder, uniqueImgName);

                    using (var fileStream = new FileStream(imgPath, FileMode.Create))
                    {
                        await dto.ProfileImageFile.CopyToAsync(fileStream);
                    }

                    user.ProfileImageUrl = $"/uploads/avatars/{uniqueImgName}";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Profile image upload error: {ex.Message}";
                    return RedirectToAction(nameof(Profile));
                }
            }

            if (User.IsInRole("Candidate"))
            {
                if (!string.IsNullOrWhiteSpace(dto.Bio))
                {
                    user.Bio = dto.Bio;
                }

                if (!string.IsNullOrWhiteSpace(dto.Skills))
                {
                    user.Skills = dto.Skills;
                }

                if (dto.CvFile != null && dto.CvFile.Length > 0)
                {
                    var extension = Path.GetExtension(dto.CvFile.FileName).ToLowerInvariant();
                    if (extension != ".pdf" && extension != ".docx")
                    {
                        TempData["Error"] = "Only PDF and DOCX files are allowed for CV.";
                        return RedirectToAction(nameof(Profile));
                    }

                    if (dto.CvFile.Length > 1024 * 1024)
                    {
                        TempData["Error"] = "CV file size cannot exceed 1MB.";
                        return RedirectToAction(nameof(Profile));
                    }

                    try
                    {
                        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "cvs");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Delete old CV if exists
                        if (!string.IsNullOrEmpty(user.CvPath))
                        {
                            var oldFilePath = Path.Combine(_env.WebRootPath, user.CvPath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.CvFile.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await dto.CvFile.CopyToAsync(fileStream);
                        }

                        user.CvPath = $"/uploads/cvs/{uniqueFileName}";
                        user.CvFileName = dto.CvFile.FileName;
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = $"File upload error: {ex.Message}";
                        return RedirectToAction(nameof(Profile));
                    }
                }
            }
            else if (User.IsInRole("Employer"))
            {
                if (!string.IsNullOrWhiteSpace(dto.CompanyName))
                {
                    user.CompanyName = dto.CompanyName;
                }

                if (!string.IsNullOrWhiteSpace(dto.CompanyDescription))
                {
                    user.CompanyDescription = dto.CompanyDescription;
                }

                if (!string.IsNullOrWhiteSpace(dto.WebsiteUrl))
                {
                    user.WebsiteUrl = dto.WebsiteUrl;
                }

                if (!string.IsNullOrWhiteSpace(dto.Location))
                {
                    user.Location = dto.Location;
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Profile details updated successfully!";
            }
            else
            {
                TempData["Error"] = string.Join(" ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Profile));
        }

        [Authorize(Roles = "Candidate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDefaultCv()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(user.CvPath))
            {
                var filePath = Path.Combine(_env.WebRootPath, user.CvPath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                user.CvPath = null;
                user.CvFileName = null;
                await _userManager.UpdateAsync(user);
                TempData["Success"] = "Default resume removed successfully.";
            }
            else
            {
                TempData["Error"] = "No default resume to delete.";
            }

            return RedirectToAction(nameof(Profile));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Failed to change password. Please verify requirements.";
                return RedirectToAction(nameof(Profile));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (result.Succeeded)
            {
                TempData["Success"] = "Password changed successfully!";
            }
            else
            {
                TempData["Error"] = string.Join(" ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Profile));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProfileImage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                var filePath = Path.Combine(_env.WebRootPath, user.ProfileImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                user.ProfileImageUrl = null;
                await _userManager.UpdateAsync(user);
                TempData["Success"] = "Profile image removed successfully.";
            }
            else
            {
                TempData["Error"] = "No profile image to delete.";
            }

            return RedirectToAction(nameof(Profile));
        }

        public IActionResult AccessDenied() => View();
    }
}

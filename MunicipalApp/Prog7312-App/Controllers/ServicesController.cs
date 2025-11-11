using Microsoft.AspNetCore.Mvc;
using Prog7312_App.Models;

  
// Author: Microsoft Docs Contributors  
// Reference: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions

namespace Prog7312_App.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ILogger<ServicesController> _logger;
        private static List<ServiceRequest> _serviceRequests = new();
        private static int _nextId = 1;

        public ServicesController(ILogger<ServicesController> logger)
        {
            _logger = logger;
        }

        public IActionResult ReportIssue()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReportIssue(ServiceRequest request, List<IFormFile> attachments)
        {
            if (ModelState.IsValid)
            {
                request.Id = _nextId++;
                request.ReferenceNumber = $"SR{DateTime.Now:yyyyMMdd}{request.Id:D4}";
                request.CreatedAt = DateTime.Now;
                request.SubmittedByEmail = HttpContext.Session.GetString("UserEmail");

                // Handle file uploads
                if (attachments != null && attachments.Count > 0)
                {
                    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    Directory.CreateDirectory(uploadsPath);

                    foreach (var file in attachments)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                            var filePath = Path.Combine(uploadsPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            request.AttachmentPaths.Add($"/uploads/{fileName}");
                        }
                    }
                }

                _serviceRequests.Add(request);

                TempData["SuccessMessage"] = $"Your service request has been submitted successfully. Reference number: {request.ReferenceNumber}";
                return RedirectToAction("ReportSuccess", new { id = request.Id });
            }

            return View(request);
        }

        public IActionResult ReportSuccess(int id)
        {
            var request = _serviceRequests.FirstOrDefault(r => r.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        public IActionResult GetRequestByReference(string referenceNumber)
        {
            var request = _serviceRequests.FirstOrDefault(r => r.ReferenceNumber == referenceNumber);
            if (request != null)
            {
                return View("ReportSuccess", request);
            }
            return NotFound();
        }

        public IActionResult GetRecentRequests(int count = 5)
        {
            var recentRequests = _serviceRequests
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToList();
            return Json(recentRequests);
        }

        public IActionResult GetAllRequests()
        {
            return Json(_serviceRequests);
        }

        public IActionResult ViewRequests()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var userRequests = _serviceRequests.Where(r => r.SubmittedByEmail == userEmail).ToList();
            return View(userRequests);
        }

    }
}

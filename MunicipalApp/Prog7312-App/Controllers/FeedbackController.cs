using Microsoft.AspNetCore.Mvc;
using Prog7312_App.Models;
using Prog7312_App.Models.DataStructures;

// FeedbackController: Manages user feedback submission and analytics in an ASP.NET Core MVC application.  
// Author: Microsoft Docs Contributors  
// Reference: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions

namespace Prog7312_App.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ILogger<FeedbackController> _logger;
        private static CustomDynamicArray<ServiceFeedback> _feedbackList = new();
        private static int _nextFeedbackId = 1;

        public FeedbackController(ILogger<FeedbackController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitFeedback(ServiceFeedback feedback)
        {
            // Handle checkbox binding - when unchecked, the property comes as false from model binding
            // When checked, it should come as true. No additional handling needed.

            if (ModelState.IsValid)
            {
                feedback.Id = _nextFeedbackId++;
                feedback.CreatedAt = DateTime.Now;
                feedback.Stage = FeedbackStage.Submission;

                _feedbackList.Add(feedback);

                // Return JSON response for AJAX calls
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Thank you for your feedback!" });
                }

                TempData["FeedbackSuccess"] = "Thank you for your feedback! It helps us improve our services.";
                return RedirectToAction("Index", "Home");
            }

            // Return validation errors for AJAX
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var errors = new CustomDynamicArray<string>();
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    errors.Add(modelError.ErrorMessage);
                }
                return Json(new { success = false, errors = errors.ToArray() });
            }

            return View(feedback);
        }

        // Admin view to see all feedback (for municipal staff)
        public IActionResult Analytics()
        {
            double CalculateAverage(Func<ServiceFeedback, int> selector)
            {
                if (_feedbackList.Count == 0) return 0;
                double sum = 0;
                foreach (var feedback in _feedbackList)
                {
                    sum += selector(feedback);
                }
                return sum / _feedbackList.Count;
            }

            int CountRecommendations()
            {
                int count = 0;
                foreach (var feedback in _feedbackList)
                {
                    if (feedback.WouldRecommend) count++;
                }
                return count;
            }

            var recentFeedback = new CustomDynamicArray<ServiceFeedback>();
            // Get recent feedback (simple implementation without LINQ)
            var allFeedback = new CustomDynamicArray<ServiceFeedback>();
            foreach (var feedback in _feedbackList)
            {
                allFeedback.Add(feedback);
            }

            // Take up to 10 most recent (simplified - would need sorting for proper implementation)
            int takeCount = Math.Min(10, allFeedback.Count);
            for (int i = Math.Max(0, allFeedback.Count - takeCount); i < allFeedback.Count; i++)
            {
                recentFeedback.Add(allFeedback[i]);
            }

            var analytics = new FeedbackAnalytics
            {
                TotalFeedback = _feedbackList.Count,
                AverageOverallRating = CalculateAverage(f => f.OverallRating),
                AverageEaseOfReporting = CalculateAverage(f => f.EaseOfReporting),
                AverageWebsiteUsability = CalculateAverage(f => f.WebsiteUsability),
                AverageInformationClarity = CalculateAverage(f => f.InformationClarity),
                RecommendationRate = _feedbackList.Count > 0 ?
                    (CountRecommendations() * 100.0 / _feedbackList.Count) : 0,
                RecentFeedback = recentFeedback
            };

            return View(analytics);
        }
    }

    // Analytics model for dashboard
    public class FeedbackAnalytics
    {
        public int TotalFeedback { get; set; }
        public double AverageOverallRating { get; set; }
        public double AverageEaseOfReporting { get; set; }
        public double AverageWebsiteUsability { get; set; }
        public double AverageInformationClarity { get; set; }
        public double RecommendationRate { get; set; }
        public CustomDynamicArray<ServiceFeedback> RecentFeedback { get; set; } = new();
    }
}

using System.ComponentModel.DataAnnotations;


//Author: Microsoft Docs Contributors
//Reference: Microsoft Docs (2025). "Data Annotations in ASP.NET Core."
//Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation


namespace Prog7312_App.Models
{
    public class ServiceFeedback
    {
        public int Id { get; set; }

        [Required]
        public int ServiceRequestId { get; set; }

        [Required]
        public string ReferenceNumber { get; set; } = string.Empty;

        // Overall experience rating (1-5 stars)
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int OverallRating { get; set; }

        // Specific aspect ratings
        [Range(1, 5)]
        public int EaseOfReporting { get; set; }

        [Range(1, 5)]
        public int WebsiteUsability { get; set; }

        [Range(1, 5)]
        public int InformationClarity { get; set; }

        // Optional feedback text
        [StringLength(1000, ErrorMessage = "Feedback cannot exceed 1000 characters")]
        public string? Comments { get; set; }

        // What went well / suggestions
        [StringLength(500)]
        public string? WentWell { get; set; }

        [StringLength(500)]
        public string? Improvements { get; set; }

        // Would they recommend the service?
        public bool WouldRecommend { get; set; }

        // Feedback stage (submission, progress, resolution)
        public FeedbackStage Stage { get; set; } = FeedbackStage.Submission;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // User info (optional, for follow-up)
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
    }

    public enum FeedbackStage
    {
        Submission,     // Right after submitting request
        Progress,       // During processing (if we implement status updates)
        Resolution      // After issue is resolved
    }
}

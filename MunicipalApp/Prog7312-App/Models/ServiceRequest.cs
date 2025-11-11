using System.ComponentModel.DataAnnotations;

 
// Author: Microsoft Docs Contributors
//Reference: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation

namespace Prog7312_App.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Location")]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Category")]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        public List<string> AttachmentPaths { get; set; } = new List<string>();

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Submitted;

        public string? ReferenceNumber { get; set; }

        [Display(Name = "Submitted By")]
        public string? SubmittedByEmail { get; set; }

        // New Property for POE Part 3 (for Heap/PriorityQueue implementation)
        // Lower number = higher priority (e.g., 1 is urgent, 5 is low)
        public int Priority { get; set; } = 5; 
    }

    public enum ServiceRequestStatus
    {
        Submitted,
        InProgress,
        Resolved,
        Closed
    }

    public static class ServiceCategories
    {
        public static readonly List<(string Value, string Label)> Categories = new()
        {
            ("sanitation", "Sanitation"),
            ("roads", "Roads"),
            ("utilities", "Utilities"),
            ("safety", "Public Safety"),
            ("parks", "Parks")
        };
    }
}
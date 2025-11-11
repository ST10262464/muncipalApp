using System.ComponentModel.DataAnnotations;
using Prog7312_App.Models.DataStructures;

namespace Prog7312_App.Models
{
    //Author: Microsoft Docs Contributors
    //Reference: Microsoft Docs (2025). "Data Annotations in ASP.NET Core."
    //Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation
    // Represents a local event or announcement in the municipal system.

    public class Event
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        public string? Organizer { get; set; }

        public int Priority { get; set; } = 3; // 1 = High, 2 = Medium, 3 = Low

        public string? ImageUrl { get; set; }

        public bool IsFeatured { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int ViewCount { get; set; } = 0;

        // Additional metadata for recommendations
        public CustomDynamicArray<string> Tags { get; set; } = new CustomDynamicArray<string>();
    }

    /// <summary>
    /// Represents event categories used in the system.
    /// </summary>
    public static class EventCategories
    {
        public const string Community = "Community";
        public const string Sports = "Sports";
        public const string Culture = "Culture";
        public const string Education = "Education";
        public const string Health = "Health";
        public const string Environment = "Environment";
        public const string Safety = "Safety";
        public const string Infrastructure = "Infrastructure";
        public const string Recreation = "Recreation";
        public const string Government = "Government";

        public static CustomDynamicArray<string> GetAll()
        {
            var categories = new CustomDynamicArray<string>();
            categories.Add(Community);
            categories.Add(Sports);
            categories.Add(Culture);
            categories.Add(Education);
            categories.Add(Health);
            categories.Add(Environment);
            categories.Add(Safety);
            categories.Add(Infrastructure);
            categories.Add(Recreation);
            categories.Add(Government);
            return categories;
        }
    }
}

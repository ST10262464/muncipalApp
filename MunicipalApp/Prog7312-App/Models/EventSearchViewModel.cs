using Prog7312_App.Models.DataStructures;

namespace Prog7312_App.Models
{
    
    // View model for event search functionality with filters and results.
    
    public class EventSearchViewModel
    {
        public string? SearchQuery { get; set; }
        public string? SelectedCategory { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SortBy { get; set; } = "date"; // date, priority, title

        // Results
        public CustomDynamicArray<Event> Events { get; set; } = new CustomDynamicArray<Event>();
        public CustomDynamicArray<Event> RecommendedEvents { get; set; } = new CustomDynamicArray<Event>();
        
        // Available categories for filter
        public CustomDynamicArray<string> Categories { get; set; } = EventCategories.GetAll();

        // Search statistics
        public int TotalResults { get; set; }
        public bool HasSearched { get; set; }
    }

    
    // Tracks user search patterns for recommendation engine.
    
    public class UserSearchPattern
    {
        public string UserId { get; set; } = string.Empty;
        public CustomQueue<string> RecentSearches { get; set; } = new CustomQueue<string>();
        public CustomHashTable<string, int> CategoryPreferences { get; set; } = new CustomHashTable<string, int>();
        public CustomSet<string> ViewedEventIds { get; set; } = new CustomSet<string>();
        public DateTime LastSearchDate { get; set; } = DateTime.Now;
    }
}

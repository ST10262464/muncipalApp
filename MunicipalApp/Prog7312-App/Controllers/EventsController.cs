using Microsoft.AspNetCore.Mvc;
using Prog7312_App.Models;
using Prog7312_App.Models.DataStructures;

namespace Prog7312_App.Controllers
{
    // Controller for managing local events and announcements using advanced data structures.
    // Implements search functionality, event organization, and recommendations.
    // Author: Microsoft Docs Contributors  
    // Reference: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions
    public class EventsController : Controller
    {
        // Storage using custom data structures
        private static CustomHashTable<int, Event> _eventStore = new CustomHashTable<int, Event>();
        private static CustomSortedDictionary<DateTime, CustomDynamicArray<Event>> _eventsByDate = new CustomSortedDictionary<DateTime, CustomDynamicArray<Event>>();
        private static CustomHashTable<string, CustomDynamicArray<Event>> _eventsByCategory = new CustomHashTable<string, CustomDynamicArray<Event>>();
        private static CustomPriorityQueue<Event> _priorityEvents = new CustomPriorityQueue<Event>();
        private static CustomSet<string> _allCategories = new CustomSet<string>();
        private static CustomSet<string> _allTags = new CustomSet<string>();

        // User search patterns for recommendations
        private static CustomHashTable<string, UserSearchPattern> _userSearchPatterns = new CustomHashTable<string, UserSearchPattern>();

        // Navigation history using stack
        private static CustomStack<string> _navigationHistory = new CustomStack<string>();

        private static int _nextEventId = 1;
        private static bool _isInitialized = false;

        private readonly ILogger<EventsController> _logger;

        public EventsController(ILogger<EventsController> logger)
        {
            _logger = logger;
            if (!_isInitialized)
            {
                InitializeSampleData();
                _isInitialized = true;
            }
        }

        // Displays the main events page with upcoming events organized by priority.
        public IActionResult Index()
        {
            TrackNavigation("Index");

            var viewModel = new EventSearchViewModel
            {
                Categories = EventCategories.GetAll()
            };

            // Get all upcoming events sorted by date
            var upcomingEvents = GetUpcomingEvents();
            viewModel.Events = upcomingEvents;
            viewModel.TotalResults = upcomingEvents.Count;

            // Get recommendations based on user's session
            var userId = GetUserId();
            viewModel.RecommendedEvents = GetRecommendedEvents(userId);

            return View(viewModel);
        }

        // Handles event search with category and date filters.
        [HttpGet]
        public IActionResult Search(string? searchQuery, string? category, DateTime? startDate, DateTime? endDate, string? sortBy)
        {
            TrackNavigation($"Search: {searchQuery ?? "All"}");

            var userId = GetUserId();
            TrackUserSearch(userId, searchQuery, category);

            var viewModel = new EventSearchViewModel
            {
                SearchQuery = searchQuery,
                SelectedCategory = category,
                StartDate = startDate,
                EndDate = endDate,
                SortBy = sortBy ?? "date",
                Categories = EventCategories.GetAll(),
                HasSearched = true
            };

            var filteredEvents = new CustomDynamicArray<Event>();

            // Filter by category
            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                if (_eventsByCategory.TryGetValue(category, out var categoryEvents) && categoryEvents != null)
                {
                    foreach (var evt in categoryEvents)
                        filteredEvents.Add(evt);
                }
            }
            else
            {
                foreach (var kvp in _eventStore)
                    filteredEvents.Add(kvp.Value);
            }

            // Filter by search query
            if (!string.IsNullOrEmpty(searchQuery))
                filteredEvents = FilterBySearchQuery(filteredEvents, searchQuery);

            // Filter by date range
            if (startDate.HasValue || endDate.HasValue)
                filteredEvents = FilterByDateRange(filteredEvents, startDate, endDate);

            // Sort results
            filteredEvents = SortEvents(filteredEvents, sortBy ?? "date");

            viewModel.Events = filteredEvents;
            viewModel.TotalResults = filteredEvents.Count;
            viewModel.RecommendedEvents = GetRecommendedEvents(userId);

            return View("Index", viewModel);
        }

        // Displays detailed information about a specific event.
        [HttpGet]
        public IActionResult Details(int id)
        {
            TrackNavigation($"Details: {id}");

            if (_eventStore.TryGetValue(id, out var evt) && evt != null)
            {
                var userId = GetUserId();
                TrackEventView(userId, id.ToString());
                evt.ViewCount++;

                return View(evt);
            }

            return NotFound();
        }

        // Returns events by category.
        [HttpGet]
        public IActionResult Category(string category)
        {
            TrackNavigation($"Category: {category}");

            var viewModel = new EventSearchViewModel
            {
                SelectedCategory = category,
                Categories = EventCategories.GetAll(),
                HasSearched = true
            };

            if (_eventsByCategory.TryGetValue(category, out var events) && events != null)
            {
                viewModel.Events = events;
                viewModel.TotalResults = events.Count;
            }

            var userId = GetUserId();
            viewModel.RecommendedEvents = GetRecommendedEvents(userId);

            return View("Index", viewModel);
        }

        #region Helper Methods

        // Gets upcoming events sorted by date.
        private CustomDynamicArray<Event> GetUpcomingEvents()
        {
            var upcomingEvents = new CustomDynamicArray<Event>();
            var today = DateTime.Today;

            foreach (var kvp in _eventsByDate)
            {
                if (kvp.Key >= today)
                {
                    foreach (var evt in kvp.Value)
                        upcomingEvents.Add(evt);
                }
            }

            return upcomingEvents;
        }

        // Filters events by search query (title, description, tags).
        private CustomDynamicArray<Event> FilterBySearchQuery(CustomDynamicArray<Event> events, string query)
        {
            var filtered = new CustomDynamicArray<Event>();
            var lowerQuery = query.ToLower();

            for (int i = 0; i < events.Count; i++)
            {
                var evt = events[i];
                if (evt.Title.ToLower().Contains(lowerQuery) ||
                    evt.Description.ToLower().Contains(lowerQuery) ||
                    evt.Location.ToLower().Contains(lowerQuery) ||
                    ContainsTag(evt.Tags, lowerQuery))
                {
                    filtered.Add(evt);
                }
            }

            return filtered;
        }

        // Checks if event tags contain the search query.
        private bool ContainsTag(CustomDynamicArray<string> tags, string query)
        {
            for (int i = 0; i < tags.Count; i++)
            {
                if (tags[i].ToLower().Contains(query))
                    return true;
            }
            return false;
        }

        // Filters events by date range.
        private CustomDynamicArray<Event> FilterByDateRange(CustomDynamicArray<Event> events, DateTime? startDate, DateTime? endDate)
        {
            var filtered = new CustomDynamicArray<Event>();

            for (int i = 0; i < events.Count; i++)
            {
                var evt = events[i];
                bool includeEvent = true;

                if (startDate.HasValue && evt.EventDate.Date < startDate.Value.Date)
                    includeEvent = false;

                if (endDate.HasValue && evt.EventDate.Date > endDate.Value.Date)
                    includeEvent = false;

                if (includeEvent)
                    filtered.Add(evt);
            }

            return filtered;
        }

        // Sorts events based on the specified criteria.
        private CustomDynamicArray<Event> SortEvents(CustomDynamicArray<Event> events, string sortBy)
        {
            var eventArray = new Event[events.Count];
            for (int i = 0; i < events.Count; i++)
                eventArray[i] = events[i];

            switch (sortBy.ToLower())
            {
                case "priority":
                    Array.Sort(eventArray, (a, b) => a.Priority.CompareTo(b.Priority));
                    break;
                case "title":
                    Array.Sort(eventArray, (a, b) => string.Compare(a.Title, b.Title, StringComparison.OrdinalIgnoreCase));
                    break;
                case "date":
                default:
                    Array.Sort(eventArray, (a, b) => a.EventDate.CompareTo(b.EventDate));
                    break;
            }

            var sorted = new CustomDynamicArray<Event>();
            foreach (var evt in eventArray)
                sorted.Add(evt);

            return sorted;
        }

        // Recommendation algorithm based on user search patterns.
        private CustomDynamicArray<Event> GetRecommendedEvents(string userId)
        {
            var recommendations = new CustomDynamicArray<Event>();

            if (!_userSearchPatterns.TryGetValue(userId, out var pattern) || pattern == null)
                return GetFeaturedEvents();

            var scoredEvents = new CustomHashTable<int, double>();

            foreach (var kvp in _eventStore)
            {
                var evt = kvp.Value;
                double score = 0;

                if (pattern.ViewedEventIds.Contains(evt.Id.ToString()))
                    continue;

                if (pattern.CategoryPreferences.TryGetValue(evt.Category, out var categoryCount))
                    score += categoryCount * 10;

                score += (4 - evt.Priority) * 5;

                var daysUntilEvent = (evt.EventDate - DateTime.Now).TotalDays;
                if (daysUntilEvent >= 0 && daysUntilEvent <= 30)
                    score += (30 - daysUntilEvent) / 3;

                score += evt.ViewCount * 0.5;

                scoredEvents.Add(evt.Id, score);
            }

            var eventScores = new CustomDynamicArray<(Event evt, double score)>();
            foreach (var kvp in scoredEvents)
            {
                if (_eventStore.TryGetValue(kvp.Key, out var evt) && evt != null)
                    eventScores.Add((evt, kvp.Value));
            }

            // Simple bubble sort for small dataset
            for (int i = 0; i < eventScores.Count - 1; i++)
            {
                for (int j = 0; j < eventScores.Count - i - 1; j++)
                {
                    if (eventScores[j].score < eventScores[j + 1].score)
                    {
                        var temp = eventScores[j];
                        eventScores[j] = eventScores[j + 1];
                        eventScores[j + 1] = temp;
                    }
                }
            }

            int count = Math.Min(5, eventScores.Count);
            for (int i = 0; i < count; i++)
                recommendations.Add(eventScores[i].evt);

            return recommendations;
        }

        // Gets featured events when no user preferences are available.
        private CustomDynamicArray<Event> GetFeaturedEvents()
        {
            var featured = new CustomDynamicArray<Event>();

            foreach (var kvp in _eventStore)
            {
                if (kvp.Value.IsFeatured && kvp.Value.EventDate >= DateTime.Today)
                    featured.Add(kvp.Value);
            }

            return featured;
        }

        // Tracks user search patterns.
        private void TrackUserSearch(string userId, string? query, string? category)
        {
            if (!_userSearchPatterns.TryGetValue(userId, out var pattern))
            {
                pattern = new UserSearchPattern { UserId = userId };
                _userSearchPatterns.Add(userId, pattern);
            }

            if (!string.IsNullOrEmpty(query))
            {
                pattern.RecentSearches.Enqueue(query);
                if (pattern.RecentSearches.Count > 10)
                    pattern.RecentSearches.Dequeue();
            }

            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                if (pattern.CategoryPreferences.TryGetValue(category, out var count))
                    pattern.CategoryPreferences[category] = count + 1;
                else
                    pattern.CategoryPreferences.Add(category, 1);
            }

            pattern.LastSearchDate = DateTime.Now;
        }

        // Tracks when a user views an event.
        private void TrackEventView(string userId, string eventId)
        {
            if (!_userSearchPatterns.TryGetValue(userId, out var pattern))
            {
                pattern = new UserSearchPattern { UserId = userId };
                _userSearchPatterns.Add(userId, pattern);
            }

            pattern.ViewedEventIds.Add(eventId);
        }

        // Tracks navigation history.
        private void TrackNavigation(string page)
        {
            _navigationHistory.Push($"{page} - {DateTime.Now:HH:mm:ss}");

            if (_navigationHistory.Count > 50)
            {
                var temp = new CustomStack<string>();
                for (int i = 0; i < 50; i++)
                {
                    if (!_navigationHistory.IsEmpty)
                        temp.Push(_navigationHistory.Pop());
                }
                _navigationHistory.Clear();
                while (!temp.IsEmpty)
                    _navigationHistory.Push(temp.Pop());
            }
        }

        // Gets or creates a user ID from session.
        private string GetUserId()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                userId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("UserId", userId);
            }
            return userId;
        }

        // Initializes sample event data for demonstration.
        private void InitializeSampleData()
        {
            var sampleEvents = new CustomDynamicArray<Event>
            {
                new Event
                {
                    Id = _nextEventId++,
                    Title = "Community Clean-Up Day",
                    Description = "Join us for a community-wide clean-up initiative. We'll be cleaning parks, streets, and public spaces. Refreshments and cleaning supplies provided.",
                    Category = "Community",
                    EventDate = DateTime.Today.AddDays(7),
                    Location = "Central Park, Main Entrance",
                    Organizer = "City Environmental Department",
                    Priority = 2,
                    IsFeatured = true,
                    Tags = CreateTags("volunteer", "environment", "community")
                },
                new Event
                {
                    Id = _nextEventId++,
                    Title = "Annual Sports Festival",
                    Description = "A day filled with various sports activities including soccer, basketball, and athletics. Open to all ages. Prizes for winners!",
                    Category = "Sports",
                    EventDate = DateTime.Today.AddDays(14),
                    Location = "Municipal Stadium",
                    Organizer = "Sports & Recreation Department",
                    Priority = 2,
                    IsFeatured = true,
                    Tags = CreateTags("sports", "competition", "family")
                },
                new Event
                {
                    Id = _nextEventId++,
                    Title = "Cultural Heritage Exhibition",
                    Description = "Explore our city's rich cultural heritage through art, music, and traditional performances. Free admission for all residents.",
                    Category = "Culture",
                    EventDate = DateTime.Today.AddDays(10),
                    EndDate = DateTime.Today.AddDays(12),
                    Location = "City Cultural Center",
                    Organizer = "Department of Arts & Culture",
                    Priority = 3,
                    IsFeatured = false,
                    Tags = CreateTags("culture", "art", "heritage", "free")
                },
                new Event
                {
                    Id = _nextEventId++,
                    Title = "Public Health Awareness Campaign",
                    Description = "Free health screenings, vaccinations, and health education sessions. Medical professionals will be available for consultations.",
                    Category = "Health",
                    EventDate = DateTime.Today.AddDays(5),
                    Location = "Community Health Center",
                    Organizer = "Department of Health",
                    Priority = 1,
                    IsFeatured = true,
                    Tags = CreateTags("health", "wellness", "free", "medical")
                },
                new Event
                {
                    Id = _nextEventId++,
                    Title = "Youth Education Workshop",
                    Description = "Interactive workshops for students covering STEM subjects, career guidance, and skill development. Registration required.",
                    Category = "Education",
                    EventDate = DateTime.Today.AddDays(21),
                    Location = "Municipal Library",
                    Organizer = "Education Department",
                    Priority = 2,
                    IsFeatured = false,
                    Tags = CreateTags("education", "youth", "workshop", "STEM")
                },
                new Event
                {
                    Id = _nextEventId++,
                    Title = "Tree Planting Initiative",
                    Description = "Help us plant 1000 trees across the city. All equipment provided. Contribute to a greener future!",
                    Category = "Environment",
                    EventDate = DateTime.Today.AddDays(28),
                    Location = "Various locations citywide",
                    Organizer = "Environmental Services",
                    Priority = 2,
                    IsFeatured = true,
                    Tags = CreateTags("environment", "trees", "volunteer", "green")
                },
                new Event
                {
                    Id = _nextEventId++,
                    Title = "Emergency Preparedness Training",
                    Description = "Learn essential emergency response skills including first aid, fire safety, and disaster preparedness. Certificates provided.",
                    Category = "Safety",
                    EventDate = DateTime.Today.AddDays(15),
                    Location = "Fire Department Training Center",
                    Organizer = "Emergency Services",
                    Priority = 1,
                    IsFeatured = false,
                    Tags = CreateTags("safety", "training", "emergency", "firstaid")
                },
                new Event
                {
                    Id = _nextEventId++,
                    Title = "Infrastructure Development Forum",
                    Description = "Public consultation on upcoming infrastructure projects. Share your input on roads, water, and electricity improvements.",
                    Category = "Infrastructure",
                    EventDate = DateTime.Today.AddDays(12),
                    Location = "City Hall Auditorium",
                    Organizer = "Department of Public Works",
                    Priority = 2,
                    IsFeatured = false,
                    Tags = CreateTags("infrastructure", "development", "consultation", "public")
                },
                new Event
                {
                    Id = _nextEventId++,
                    Title = "Summer Music Festival",
                    Description = "Enjoy live performances from local and international artists. Food stalls and entertainment for the whole family.",
                    Category = "Recreation",
                    EventDate = DateTime.Today.AddDays(45),
                    EndDate = DateTime.Today.AddDays(47),
                    Location = "Riverside Park",
                    Organizer = "Parks & Recreation",
                    Priority = 3,
                    IsFeatured = true,
                    Tags = CreateTags("music", "festival", "entertainment", "family")
                },
                new Event
                {
                    Id = _nextEventId++,
                    Title = "Town Hall Meeting",
                    Description = "Meet with city officials to discuss local issues, ask questions, and learn about new policies and initiatives.",
                    Category = "Government",
                    EventDate = DateTime.Today.AddDays(8),
                    Location = "City Hall Main Chamber",
                    Organizer = "Office of the Mayor",
                    Priority = 1,
                    IsFeatured = true,
                    Tags = CreateTags("government", "meeting", "community", "policy")
                },
                new Event
                {
                    Id = _nextEventId++,
                    Title = "Farmers Market Opening",
                    Description = "Fresh local produce, artisanal goods, and handmade crafts. Support local farmers and businesses every Saturday.",
                    Category = "Community",
                    EventDate = DateTime.Today.AddDays(3),
                    Location = "Market Square",
                    Organizer = "Economic Development Office",
                    Priority = 3,
                    IsFeatured = false,
                    Tags = CreateTags("market", "local", "food", "community")
                },
                new Event
                {
                    Id = _nextEventId++,
                    Title = "Youth Soccer League Registration",
                    Description = "Register your children for the upcoming youth soccer season. Ages 6-16. Coaching and equipment provided.",
                    Category = "Sports",
                    EventDate = DateTime.Today.AddDays(18),
                    Location = "Sports Complex Office",
                    Organizer = "Youth Sports Program",
                    Priority = 2,
                    IsFeatured = false,
                    Tags = CreateTags("sports", "youth", "soccer", "registration")
                },
                new Event
                {
                    Id = _nextEventId++,
                    Title = "Small Business Development Workshop",
                    Description = "Learn essential skills for starting and growing your small business. Topics include business planning, marketing, and financial management. Free for all attendees.",
                    Category = "Education",
                    EventDate = DateTime.Today.AddDays(25),
                    Location = "Business Development Center",
                    Organizer = "Economic Development Office",
                    Priority = 2,
                    IsFeatured = true,
                    Tags = CreateTags("business", "entrepreneurship", "workshop", "free")
                },
                new Event
                {
                    Id = _nextEventId++,
                    Title = "Water Conservation Awareness Day",
                    Description = "Learn practical tips for conserving water at home and in your community. Free water-saving devices will be distributed to attendees.",
                    Category = "Environment",
                    EventDate = DateTime.Today.AddDays(16),
                    Location = "Water Treatment Facility",
                    Organizer = "Water & Sanitation Department",
                    Priority = 1,
                    IsFeatured = true,
                    Tags = CreateTags("water", "conservation", "environment", "sustainability")
                },
                new Event
                {
                    Id = _nextEventId++,
                    Title = "Community Art Fair",
                    Description = "Showcase of local artists featuring paintings, sculptures, photography, and crafts. Live art demonstrations and workshops for children.",
                    Category = "Culture",
                    EventDate = DateTime.Today.AddDays(30),
                    EndDate = DateTime.Today.AddDays(31),
                    Location = "Community Arts Center",
                    Organizer = "Department of Arts & Culture",
                    Priority = 3,
                    IsFeatured = false,
                    Tags = CreateTags("art", "culture", "community", "family")
                }
            };

            for (int i = 0; i < sampleEvents.Count; i++)
                AddEventToDataStructures(sampleEvents[i]);
        }

        // Adds an event to all relevant data structures.
        private void AddEventToDataStructures(Event evt)
        {
            _eventStore.Add(evt.Id, evt);

            var eventDate = evt.EventDate.Date;
            if (_eventsByDate.TryGetValue(eventDate, out var dateEvents))
                dateEvents.Add(evt);
            else
            {
                var newList = new CustomDynamicArray<Event>();
                newList.Add(evt);
                _eventsByDate.Add(eventDate, newList);
            }

            if (_eventsByCategory.TryGetValue(evt.Category, out var categoryEvents))
                categoryEvents.Add(evt);
            else
            {
                var newList = new CustomDynamicArray<Event>();
                newList.Add(evt);
                _eventsByCategory.Add(evt.Category, newList);
            }

            _priorityEvents.Enqueue(evt, evt.Priority);
            _allCategories.Add(evt.Category);

            for (int i = 0; i < evt.Tags.Count; i++)
                _allTags.Add(evt.Tags[i]);
        }

        // Helper method to create tags array.
        private CustomDynamicArray<string> CreateTags(params string[] tags)
        {
            var tagArray = new CustomDynamicArray<string>();
            foreach (var tag in tags)
                tagArray.Add(tag);
            return tagArray;
        }

        #endregion
    }
}

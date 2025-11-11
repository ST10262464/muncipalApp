namespace Prog7312_App.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public AnnouncementPriority Priority { get; set; }
    }
    
    public enum AnnouncementPriority
    {
        Low,
        Medium,
        High
    }
}

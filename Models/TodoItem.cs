namespace TodoApp.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.Now;  // Default to current time
        public DateTime? DateCompleted { get; set; } = DateTime.Now; // Date of completion
        public bool IsDeleted { get; set; } = false;  // For soft delete
    }
}

namespace Application01_GitAction_Deployment.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
        public DateTime HiredAt { get; set; } = DateTime.UtcNow;
    }
}

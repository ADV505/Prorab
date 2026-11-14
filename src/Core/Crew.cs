namespace Prorab.Core.Models;

public class Crew
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ForemanName { get; set; }
    public string? Phone { get; set; }
    public bool IsArchived { get; set; }
}

namespace Prorab.Core.Models;

/// <summary>Этаж объекта.</summary>
public class Floor
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public Site? Site { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsArchived { get; set; }

    public List<Room> Rooms { get; set; } = [];
}

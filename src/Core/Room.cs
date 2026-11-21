namespace Prorab.Core.Models;

/// <summary>Помещение. FloorId == null — общее помещение объекта (лестничный марш и т.п.).</summary>
public class Room
{
    public int Id { get; set; }

    public int SiteId { get; set; }
    public Site? Site { get; set; }

    public int? FloorId { get; set; }
    public Floor? Floor { get; set; }

    public string Name { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
}

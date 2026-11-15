namespace Prorab.Core.Models;

public class WorkType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>Единица измерения: м², м³, шт., смена...</summary>
    public string? Unit { get; set; }

    public bool IsArchived { get; set; }
}

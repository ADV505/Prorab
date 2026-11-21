namespace Prorab.Core.Models;

public class Crew
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int? ForemanWorkerId { get; set; }
    public Worker? Foreman { get; set; }

    public List<CrewWorker> Members { get; set; } = [];
    public List<CrewSite> Sites { get; set; } = [];

    public bool IsArchived { get; set; }

    public string ForemanName => Foreman?.FullName ?? "Не назначен";

    public string MembersSummary => string.Join(", ",
    Members.Select(m => m.Worker?.FullName ?? "—"));

    public string SitesSummary => string.Join(", ",
        Sites.Select(s => s.Site?.Name ?? "—"));


    /// <summary>Строка «объекты · состав» для карточки.</summary>
    public string Info
    {
        get
        {
            var parts = new List<string>();

            var sites = Sites.Count == 1 ? "1 объект" : $"{Sites.Count} объектов";
            if (Sites.Count > 0) parts.Add(sites);

            var members = Members.Count == 1 ? "1 человек" : $"{Members.Count} человек";
            if (Members.Count > 0) parts.Add(members);

            return string.Join(" · ", parts);
        }
    }

    public bool HasInfo => Info.Length > 0;
}

/// <summary>Состав бригады — многие-ко-многим с работниками.</summary>
public class CrewWorker
{
    public int CrewId { get; set; }
    public Crew? Crew { get; set; }
    public int WorkerId { get; set; }
    public Worker? Worker { get; set; }
}

/// <summary>Привязка бригады к объекту — многие-ко-многим с объектами.</summary>
public class CrewSite
{
    public int CrewId { get; set; }
    public Crew? Crew { get; set; }
    public int SiteId { get; set; }
    public Site? Site { get; set; }
}

namespace gigu_back_end.Briefcases.Interfaces.REST.Resources;

public record BriefcaseResource(int Id, string Name, string Description, DateTime PublishDate, int SellerId, List<ProjectResource> Projects)
{}
;
namespace gigu_back_end.Briefcases.Domain.Models.Commands;

public record CreateBriefcaseCommand(string Name, string Description, DateTime PublishDate, int SellerId, List<ProjectCommand> Projects);
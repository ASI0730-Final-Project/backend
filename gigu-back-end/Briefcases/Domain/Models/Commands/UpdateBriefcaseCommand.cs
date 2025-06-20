namespace gigu_back_end.Briefcases.Domain.Models.Commands;

public record UpdateBriefcaseCommand(int Id, String Name, String Description, DateTime PublishDate);
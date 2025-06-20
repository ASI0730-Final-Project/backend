namespace gigu_back_end.Briefcases.Domain.Models.Commands;

public record ProjectCommand(string Title, string Description, string Price, string Time, string GigLink);
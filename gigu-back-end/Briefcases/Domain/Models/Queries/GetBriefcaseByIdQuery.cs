namespace gigu_back_end.Briefcases.Domain.Models.Queries;

public record GetBriefcaseByIdQuery
{
    public GetBriefcaseByIdQuery(int briefcaseId)
    {
        BriefcaseId = briefcaseId;
    }
    
    public int BriefcaseId { get; init; }
};
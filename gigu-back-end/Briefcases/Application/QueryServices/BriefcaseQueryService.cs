using gigu_back_end.Shared.Domain; // cuidado 
using gigu_back_end.Briefcases.Domain;
using gigu_back_end.Briefcases.Domain.Models.Entities;
using gigu_back_end.Briefcases.Domain.Models.Queries; // cuidado
using gigu_back_end.Briefcases.Domain.Services; // cuidado

namespace gigu_back_end.Briefcases.Application.QueryServices;

public class BriefcaseQueryService (IBriefcaseRepository briefcaseRepository) : IBriefcaseQueryService  
{
    private readonly IBriefcaseRepository _briefcaseRepository = briefcaseRepository ?? throw new ArgumentNullException(nameof(briefcaseRepository));
    
    
    
    public async Task<IEnumerable<Briefcase>> Handle(GetAllBriefcasesQuery query)
    {
        //var briefcases = await _briefcaseRepository.ListAsync();
        var briefcases = await _briefcaseRepository.GetAllWithProjectsAsync();
        return briefcases?.Where(briefcase => briefcase.IsActive) ?? Enumerable.Empty<Briefcase>();
    }

    public async Task<Briefcase?> Handle(GetBriefcaseByIdQuery query)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));

        var briefcase = await _briefcaseRepository.FindByIdAsync(query.BriefcaseId);
        return briefcase?.IsActive == true ? briefcase : null;
    }
}
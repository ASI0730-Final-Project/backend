using Gigs.Domain.Models.Entities;
using Gigs.Domain.Models.Exceptions;
using Gigs.Domain.Models.Queries;
using Gigs.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gigs.Application.QueryService
{
    public class GigQueryService
    {
        private readonly IGigRepository _gigRepository;

        public GigQueryService(IGigRepository gigRepository)
        {
            _gigRepository = gigRepository;
        }

        public async Task<Gig> GetGigByIdAsync(GetGigByIdQuery query)
        {
            var gig = await _gigRepository.GetByIdAsync(query.Id);
            if (gig == null)
            {
                throw new GigNotFoundException(query.Id);
            }
            return gig;
        }

        public async Task<IEnumerable<Gig>> GetGigsByUserIdAsync(GetGigsByUserIdQuery query)
        {
            return await _gigRepository.GetByUserIdAsync(query.UserId);
        }

        public async Task<IEnumerable<Gig>> GetGigsByCategoryAsync(GetGigsByCategoryQuery query)
        {
            return await _gigRepository.GetByCategoryAsync(query.Category, query.Page, query.PageSize);
        }

        public async Task<IEnumerable<Gig>> GetAllGigsAsync(GetAllGigsQuery query)
        {
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                return await _gigRepository.SearchAsync(query.SearchTerm, query.Page, query.PageSize);
            }
            
            return await _gigRepository.GetAllAsync();
        }

        public async Task<bool> GigExistsAsync(int gigId)
        {
            return await _gigRepository.ExistsAsync(gigId);
        }

        public async Task<int> GetGigCountByCategoryAsync(string category)
        {
            return await _gigRepository.CountByCategoryAsync(category);
        }

        public async Task<int> GetGigCountByUserIdAsync(int userId)
        {
            return await _gigRepository.CountByUserIdAsync(userId);
        }
    }
}
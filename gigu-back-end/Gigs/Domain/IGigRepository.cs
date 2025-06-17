using Gigs.Domain.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gigs.Domain
{
    public interface IGigRepository
    {
        Task<Gig> GetByIdAsync(int id);
        Task<IEnumerable<Gig>> GetAllAsync();
        Task<IEnumerable<Gig>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Gig>> GetByCategoryAsync(string category, int page = 1, int pageSize = 10);
        Task<IEnumerable<Gig>> SearchAsync(string searchTerm, int page = 1, int pageSize = 10);
        Task<Gig> CreateAsync(Gig gig);
        Task<Gig> UpdateAsync(Gig gig);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> CountByCategoryAsync(string category);
        Task<int> CountByUserIdAsync(int userId);
    }
}
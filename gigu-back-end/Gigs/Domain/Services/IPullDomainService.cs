using Gigs.Domain.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gigs.Domain.Services
{
    public interface IPullDomainService
    {
        Task OpenPullAsync(Pull pull);

        Task<Pull> UpdatePullAsync(int pullId, decimal? newPrice = null, string? newState = null);

        Task<Pull> ClosePullAsync(int pullId);

        Task<IEnumerable<Pull>> GetAllPullsAsync();
        Task<Pull?> GetPullByIdAsync(int id);
    }
}
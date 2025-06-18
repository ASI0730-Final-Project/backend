using Gigs.Domain.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gigs.Domain.Services
{
    public interface IPullDomainService
    {
        Task OpenPullAsync(Pull pull);
        Task<Pull> UpdatePullPriceAsync(int pullId, decimal newPrice);
        Task<Pull> ClosePullAsync(int pullId);

        // Nuevos métodos para el controlador
        Task<IEnumerable<Pull>> GetAllPullsAsync();
        Task<Pull?> GetPullByIdAsync(int id);
    }
}

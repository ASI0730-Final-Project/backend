using Gigs.Domain.Models.Entities;
using System.Threading.Tasks;

namespace Gigs.Domain.Services
{
    public interface IGigDomainService
    {
        Task<bool> ValidateGigOwnershipAsync(int gigId, int userId);
        Task<bool> IsCategoryValidAsync(string category);
        Task<bool> IsUserActiveFreelancerAsync(int userId);
    }
}
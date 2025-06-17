using Gigs.Domain.Models.Entities;
using Gigs.Domain;
using Gigs.Domain.Services;
using System;
using System.Threading.Tasks;
using gigu_back_end.User.Domain;

namespace Gigs.Domain.Services
{
    public class GigDomainService : IGigDomainService
    {
        private readonly IGigRepository _gigRepository;
        private readonly IUserRepository _userRepository;

        public GigDomainService(IGigRepository gigRepository, IUserRepository userRepository)
        {
            _gigRepository = gigRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> ValidateGigOwnershipAsync(int gigId, int userId)
        {
            var gig = await _gigRepository.GetByIdAsync(gigId);
            return gig?.UserId == userId;
        }

        // Cambiado a método síncrono (eliminado async/await innecesario)
        public Task<bool> IsCategoryValidAsync(string category)
        {
            var validCategories = new[] { "Programming", "Design", "Writing", "Marketing", "Video", "Music" };
            return Task.FromResult(
                Array.Exists(validCategories, c => c.Equals(category, StringComparison.OrdinalIgnoreCase))
            );
        }

        public async Task<bool> IsUserActiveFreelancerAsync(int userId)
        {
            var user = await _userRepository.FindByIdAsync(userId);
            return user != null && user.IsActive; 
        }
    }
}
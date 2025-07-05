using Gigs.Domain.Models.Entities;
using Gigs.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gigs.Domain.Services
{
    public class PullDomainService : IPullDomainService
    {
        private readonly IPullRepository _pullRepo;
        private static readonly HashSet<string> AllowedStates = new()
        {
            "pending",
            "in_process",
            "payed",
            "complete"
        };

        public PullDomainService(IPullRepository pullRepo)
        {
            _pullRepo = pullRepo;
        }

        public async Task OpenPullAsync(Pull pull)
        {
            pull.State = "pending";
            pull.PriceUpdate = pull.PriceInit;
            await _pullRepo.CreateAsync(pull);
            await _pullRepo.SaveChangesAsync();
        }

        // Ahora permite actualizar precio y estado
        public async Task<Pull> UpdatePullAsync(int pullId, decimal? newPrice = null, string? newState = null)
        {
            var pull = await _pullRepo.GetByIdAsync(pullId);
            if (pull == null)
                throw new Exception("Pull not found");

            if (pull.State == "complete")
                throw new Exception("The auction is already complete");

            if (newPrice.HasValue)
            {
                if (newPrice <= 0)
                    throw new Exception("New price must be greater than zero.");

                pull.PriceUpdate = newPrice.Value;
            }

            if (!string.IsNullOrEmpty(newState))
            {
                if (!AllowedStates.Contains(newState))
                    throw new Exception("Invalid state.");

                pull.State = newState;
            }

            await _pullRepo.UpdateAsync(pull);
            await _pullRepo.SaveChangesAsync();

            return pull;
        }

        public async Task<Pull> ClosePullAsync(int pullId)
        {
            var pull = await _pullRepo.GetByIdAsync(pullId);
            if (pull == null)
                throw new Exception("Pull not found");

            if (pull.State == "complete")
                throw new Exception("The auction is already complete");

            pull.State = "complete";
            await _pullRepo.UpdateAsync(pull);
            await _pullRepo.SaveChangesAsync();

            return pull;
        }

        public async Task<IEnumerable<Pull>> GetAllPullsAsync()
        {
            return await _pullRepo.GetAllAsync();
        }

        public async Task<Pull?> GetPullByIdAsync(int id)
        {
            return await _pullRepo.GetByIdAsync(id);
        }
    }
}

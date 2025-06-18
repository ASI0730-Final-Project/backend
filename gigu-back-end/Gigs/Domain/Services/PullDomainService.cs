using Gigs.Domain.Models.Entities;
using Gigs.Domain.Services;
using Gigs.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gigs.Domain.Services
{
    public class PullDomainService : IPullDomainService
    {
        private readonly IPullRepository _pullRepo;

        public PullDomainService(IPullRepository pullRepo)
        {
            _pullRepo = pullRepo;
        }

        public async Task OpenPullAsync(Pull pull)
        {
            pull.State = "abierta";
            pull.PriceUpdate = pull.PriceInit;
            await _pullRepo.CreateAsync(pull);
            await _pullRepo.SaveChangesAsync();
        }

        public async Task<Pull> UpdatePullPriceAsync(int pullId, decimal newPrice)
        {
            var pull = await _pullRepo.GetByIdAsync(pullId);
            if (pull == null) throw new Exception("Pull no encontrado");

            if (pull.State != "abierta")
                throw new Exception("La subasta ya está cerrada");

            if (newPrice <= pull.PriceUpdate)
                throw new Exception("El nuevo precio debe ser mayor al actual");

            pull.PriceUpdate = newPrice;
            await _pullRepo.UpdateAsync(pull);
            await _pullRepo.SaveChangesAsync();

            return pull;
        }

        public async Task<Pull> ClosePullAsync(int pullId)
        {
            var pull = await _pullRepo.GetByIdAsync(pullId);
            if (pull == null) throw new Exception("Pull no encontrado");

            if (pull.State == "cerrada")
                throw new Exception("La subasta ya está cerrada");

            pull.State = "cerrada";
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

using Gigs.Domain;
using Gigs.Domain.Models.Entities;
using gigu_back_end.Shared.Infrastructure.Persistence.Configuration; // ← tu contexto real
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gigs.Infrastructure.Persistence.EFC.Repositories
{
    public class PullRepository : IPullRepository
    {
        private readonly GigUContext _context;

        public PullRepository(GigUContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pull>> GetAllAsync()
        {
            return await _context.Pulls.ToListAsync();
        }

        public async Task<Pull> GetByIdAsync(int id)
        {
            return await _context.Pulls.FindAsync(id);
        }

        public async Task CreateAsync(Pull pull)
        {
            await _context.Pulls.AddAsync(pull);
        }

        public async Task UpdateAsync(Pull pull)
        {
            _context.Pulls.Update(pull);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

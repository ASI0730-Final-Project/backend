using gigu_back_end.Shared.Infraestructure.Persistence.Repositories;
using Gigs.Domain.Models.Entities;
using Gigs.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gigu_back_end.Shared.Infrastructure.Persistence.Configuration;

namespace Gigs.Infrastructure.Persistence.EFC.Repositories
{
    public class GigRepository : BaseRepository<Gig>, IGigRepository
    {
        public GigRepository(GigUContext context) : base(context) { }

        public async Task<Gig> GetByIdAsync(int id)
        {
            return await base.FindByIdAsync(id) ?? throw new KeyNotFoundException($"Gig con ID {id} no encontrado");
        }

        // Resto de los métodos se mantienen exactamente igual...
        public async Task<IEnumerable<Gig>> GetAllAsync()
        {
            return await Context.Set<Gig>()
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Gig>> GetByUserIdAsync(int userId)
        {
            return await Context.Set<Gig>()
                .Where(g => g.UserId == userId)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Gig>> GetByCategoryAsync(string category, int page = 1, int pageSize = 10)
        {
            return await Context.Set<Gig>()
                .Where(g => g.Category.ToLower() == category.ToLower())
                .OrderByDescending(g => g.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Gig>> SearchAsync(string searchTerm, int page = 1, int pageSize = 10)
        {
            var lowerSearchTerm = searchTerm.ToLower();
            
            return await Context.Set<Gig>()
                .Where(g => g.Title.ToLower().Contains(lowerSearchTerm) ||
                           g.Description.ToLower().Contains(lowerSearchTerm) ||
                           g.Category.ToLower().Contains(lowerSearchTerm))
                .OrderByDescending(g => g.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Gig> CreateAsync(Gig gig)
        {
            await AddAsync(gig);
            await Context.SaveChangesAsync();
            return gig;
        }

        public async Task<Gig> UpdateAsync(Gig gig)
        {
            Update(gig);
            await Context.SaveChangesAsync();
            return gig;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var gig = await GetByIdAsync(id);
            Remove(gig);
            await Context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await Context.Set<Gig>().AnyAsync(g => g.Id == id);
        }

        public async Task<int> CountByCategoryAsync(string category)
        {
            return await Context.Set<Gig>()
                .Where(g => g.Category.ToLower() == category.ToLower())
                .CountAsync();
        }

        public async Task<int> CountByUserIdAsync(int userId)
        {
            return await Context.Set<Gig>()
                .Where(g => g.UserId == userId)
                .CountAsync();
        }
    }
}
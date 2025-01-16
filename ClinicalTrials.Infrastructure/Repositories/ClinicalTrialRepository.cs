using ClinicalTrials.Application.Interfaces;
using ClinicalTrials.Domain.Entities;
using ClinicalTrials.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrials.Infrastructure.Repositories
{
    public class ClinicalTrialRepository : IClinicalTrialRepository
    {
        private readonly AppDbContext _context;

        public ClinicalTrialRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ClinicalTrial trial)
        {
            trial.CalculateDurationAndSetEndDate();
            
            _context.ClinicalTrials.Add(trial);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ClinicalTrial>> GetAllAsync()
        {
            return await _context.ClinicalTrials.ToListAsync();
        }

        public async Task<ClinicalTrial?> GetByIdAsync(Guid id)
        {
            var trial = await _context.ClinicalTrials.FindAsync(id);
            if (trial != null)
            {
                trial.CalculateDurationAndSetEndDate();
            }
            return trial;
        }

        public async Task<IEnumerable<ClinicalTrial>> GetFilteredAsync(string? status, string? title, string? trialId)
        {
            var query = _context.ClinicalTrials.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(t => t.Status == status);

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(t => t.Title != null && t.Title.Contains(title));

            if (!string.IsNullOrWhiteSpace(trialId))
                query = query.Where(t => t.TrialId == trialId);

            var trials = await query.ToListAsync();

            foreach (var trial in trials)
            {
                trial.CalculateDurationAndSetEndDate();
            }

            return trials;
        }
    }
}

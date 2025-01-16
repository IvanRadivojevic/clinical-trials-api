using ClinicalTrials.Domain.Entities;

namespace ClinicalTrials.Application.Interfaces
{
    public interface IClinicalTrialRepository
    {
        Task AddAsync(ClinicalTrial clinicalTrial);
        Task<IEnumerable<ClinicalTrial>> GetAllAsync();
        Task<ClinicalTrial?> GetByIdAsync(Guid id);
        Task<IEnumerable<ClinicalTrial>> GetFilteredAsync(string? status, string? title, string? trialId);
    }
}

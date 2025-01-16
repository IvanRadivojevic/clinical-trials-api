using System;

namespace ClinicalTrials.Domain.Entities
{
    public class ClinicalTrial
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? TrialId { get; set; }
        public string? Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty; 
        public int Participants { get; set; }
        public int Duration { get; set; }

        public void CalculateDurationAndSetEndDate()
        {
            if (Status == "Ongoing" && !EndDate.HasValue)
            {
                EndDate = StartDate.AddMonths(1);
            }

            if (EndDate.HasValue)
            {
                Duration = (EndDate.Value - StartDate).Days;
            }
            else
            {
                Duration = 0;
            }
        }
    }
}

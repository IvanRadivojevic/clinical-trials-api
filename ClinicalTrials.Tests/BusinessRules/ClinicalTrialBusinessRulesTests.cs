using ClinicalTrials.Application.Interfaces;
using ClinicalTrials.Domain.Entities;
using ClinicalTrials.Infrastructure.Data;
using ClinicalTrials.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrials.Tests.BusinessRules
{
    public class ClinicalTrialBusinessRulesTests
    {
        private readonly IClinicalTrialRepository _repository;
        private readonly AppDbContext _context;

        public ClinicalTrialBusinessRulesTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _context = new AppDbContext(options);
            _repository = new ClinicalTrialRepository(_context);
        }

        [Fact]
        public async Task OngoingTrial_WithoutEndDate_ShouldSetEndDateToOneMonth()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var trial = new ClinicalTrial
            {
                Id = Guid.NewGuid(),
                TrialId = "TEST001",
                Title = "Test Trial",
                Status = "Ongoing",
                StartDate = startDate,
                EndDate = null
            };

            // Act
            await _repository.AddAsync(trial);

            // Assert
            var savedTrial = await _context.ClinicalTrials.FindAsync(trial.Id);
            Assert.NotNull(savedTrial);
            Assert.Equal(startDate.AddMonths(1), savedTrial.EndDate);
        }

        [Fact]
        public async Task Trial_ShouldCalculateDurationInDays()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 2, 1);
            var trial = new ClinicalTrial
            {
                Id = Guid.NewGuid(),
                TrialId = "TEST002",
                Title = "Test Trial",
                Status = "Completed",
                StartDate = startDate,
                EndDate = endDate
            };

            // Act
            await _repository.AddAsync(trial);

            // Assert
            var savedTrial = await _context.ClinicalTrials.FindAsync(trial.Id);
            Assert.NotNull(savedTrial);
            Assert.Equal(31, savedTrial.Duration);
        }

        [Theory]
        [InlineData("Ongoing", true)]
        [InlineData("Completed", false)]
        [InlineData("Not Started", false)]
        public async Task Trial_EndDateCalculation_ShouldDependOnStatus(string status, bool shouldSetEndDate)
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var trial = new ClinicalTrial
            {
                Id = Guid.NewGuid(),
                TrialId = $"TEST_{status}",
                Title = "Test Trial",
                Status = status,
                StartDate = startDate,
                EndDate = null
            };

            // Act
            await _repository.AddAsync(trial);

            // Assert
            var savedTrial = await _context.ClinicalTrials.FindAsync(trial.Id);
            Assert.NotNull(savedTrial);
            
            if (shouldSetEndDate)
            {
                Assert.Equal(startDate.AddMonths(1), savedTrial.EndDate);
            }
            else
            {
                Assert.Null(savedTrial.EndDate);
            }
        }

        [Fact]
        public async Task Trial_WithExistingEndDate_ShouldNotOverwrite()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var originalEndDate = new DateTime(2024, 3, 1);
            var trial = new ClinicalTrial
            {
                Id = Guid.NewGuid(),
                TrialId = "TEST003",
                Title = "Test Trial",
                Status = "Ongoing",
                StartDate = startDate,
                EndDate = originalEndDate
            };

            // Act
            await _repository.AddAsync(trial);

            // Assert
            var savedTrial = await _context.ClinicalTrials.FindAsync(trial.Id);
            Assert.NotNull(savedTrial);
            Assert.Equal(originalEndDate, savedTrial.EndDate);
            Assert.Equal(60, savedTrial.Duration);
        }
    }
} 
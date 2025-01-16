using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Logging;
using ClinicalTrials.Infrastructure.Repositories;
using ClinicalTrials.Application.Interfaces;
using ClinicalTrials.Infrastructure.Data;
using ClinicalTrials.Domain.Entities;

public class ClinicalTrialRepositoryTests
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly AppDbContext _context;
    private readonly IClinicalTrialRepository _repository;
    private readonly ILogger<ClinicalTrialRepository> _logger;

    public ClinicalTrialRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(_options);
        _logger = Mock.Of<ILogger<ClinicalTrialRepository>>();
        _repository = new ClinicalTrialRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddClinicalTrial()
    {
        // Arrange
        var trial = new ClinicalTrial
        {
            Id = Guid.NewGuid(),
            TrialId = "TEST001",
            Title = "Test Trial",
            Status = "Ongoing"
        };

        // Act
        await _repository.AddAsync(trial);

        // Assert
        var result = await _context.ClinicalTrials.FindAsync(trial.Id);
        Assert.NotNull(result);
        Assert.Equal(trial.TrialId, result.TrialId);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnFilteredResults()
    {
        // Arrange
        await SeedTestData();

        // Act
        var results = await _repository.GetFilteredAsync(
            status: "Ongoing",
            title: null,
            trialId: null
        );

        // Assert
        Assert.NotEmpty(results);
        Assert.All(results, trial => Assert.Equal("Ongoing", trial.Status));
    }

    [Fact]
    public async Task GetFilteredAsync_WithMultipleFilters_ShouldReturnCorrectResults()
    {
        // Arrange
        await SeedTestData();

        // Act
        var results = await _repository.GetFilteredAsync(
            status: "Ongoing",
            title: "Test Trial 1",
            trialId: "T1"
        );

        // Assert
        Assert.Single(results);
        var trial = results.First();
        Assert.Equal("Ongoing", trial.Status);
        Assert.Equal("Test Trial 1", trial.Title);
        Assert.Equal("T1", trial.TrialId);
    }

    private async Task SeedTestData()
    {
        var trials = new List<ClinicalTrial>
        {
            new() 
            { 
                Id = Guid.NewGuid(), 
                TrialId = "T1", 
                Title = "Test Trial 1",
                Status = "Ongoing" 
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                TrialId = "T2", 
                Title = "Test Trial 2",
                Status = "Completed" 
            }
        };

        await _context.ClinicalTrials.AddRangeAsync(trials);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetFilteredAsync_WithNoFilters_ShouldReturnAllResults()
    {
        // Arrange
        await SeedTestData();

        // Act
        var results = await _repository.GetFilteredAsync(
            status: null,
            title: null,
            trialId: null
        );

        // Assert
        Assert.Equal(2, results.Count());
    }
} 
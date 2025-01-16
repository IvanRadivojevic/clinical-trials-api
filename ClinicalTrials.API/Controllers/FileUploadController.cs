using ClinicalTrials.Application.Interfaces;
using ClinicalTrials.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.ComponentModel.DataAnnotations;

namespace ClinicalTrials.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class FileUploadController : ControllerBase
    {
        private readonly IClinicalTrialRepository _repository;
        private readonly ILogger<FileUploadController> _logger;

        public FileUploadController(IClinicalTrialRepository repository, ILogger<FileUploadController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ClinicalTrial), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadFile([Required] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File is required");

                using var reader = new StreamReader(file.OpenReadStream());
                var jsonContent = await reader.ReadToEndAsync();

                try
                {
                    var jToken = JToken.Parse(jsonContent);

                    // Create JSON schema for validation
                    var schema = new JSchema
                    {
                        Type = JSchemaType.Object,
                        Properties =
                        {
                            ["trialId"] = new JSchema { Type = JSchemaType.String },
                            ["title"] = new JSchema { Type = JSchemaType.String },
                            ["startDate"] = new JSchema { Type = JSchemaType.String },
                            ["status"] = new JSchema { Type = JSchemaType.String },
                            ["participants"] = new JSchema { Type = JSchemaType.Integer }
                        },
                        Required = { "trialId", "title", "startDate", "status", "participants" }
                    };

                    // Validate against schema
                    if (!jToken.IsValid(schema, out IList<string> errorMessages))
                    {
                        return BadRequest($"Invalid JSON format: {string.Join(", ", errorMessages)}");
                    }

                    var trial = JsonConvert.DeserializeObject<ClinicalTrial>(jsonContent);

                    // Additional validation
                    if (trial.StartDate < DateTime.Now)
                        return BadRequest("Start date must be in the future");

                    if (trial.Participants <= 0)
                        return BadRequest("Number of participants must be greater than 0");

                    await _repository.AddAsync(trial);

                    return Ok(trial);
                }
                catch (JsonReaderException)
                {
                    return BadRequest("Invalid JSON format");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file upload");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClinicalTrial), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClinicalTrial>> GetById(Guid id)
        {
            var trial = await _repository.GetByIdAsync(id);
            if (trial == null)
                return NotFound();

            return Ok(trial);
        }


        [HttpGet("filter")]
        [ProducesResponseType(typeof(IEnumerable<ClinicalTrial>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClinicalTrial>>> GetFiltered(
            [FromQuery] string? status,
            [FromQuery] string? title,
            [FromQuery] string? trialId)
        {
            var trials = await _repository.GetFilteredAsync(status, title, trialId);
            return Ok(trials);
        }

  
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<ClinicalTrial>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClinicalTrial>>> GetAll()
        {
            var trials = await _repository.GetAllAsync();
            return Ok(trials);
        }
    }
}

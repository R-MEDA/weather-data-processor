using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherDataService.Repository;
using WeatherDataService.Repository.Entities;

namespace WeatherDataService.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherReadingController(ILogger<WeatherReadingController> logger, WeatherDbContext context) : ControllerBase
	{
		private readonly ILogger<WeatherReadingController> _logger = logger;
		private readonly WeatherDbContext _context = context;

		[HttpPost]
		public async Task<IActionResult> CreateReading([FromBody] ReadingCreateCommand weatherReading, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received weather reading: {WeatherReading}", weatherReading);

			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Invalid weather reading data: {ModelState}", ModelState);
				return BadRequest(ModelState);
			}

			_context.WeatherReading.Add(new()
			{
				Timestamp = DateTime.UtcNow,
				Temperature = weatherReading.Temperature,
				Humidity = weatherReading.Humidity,
				Pressure = weatherReading.Pressure,
				Location = weatherReading.Location
			});

			if (weatherReading.Location == "Amsterdam" && weatherReading.Temperature > 40)
			{
				// TO-DO -> publish event to the message queue
			}

			await _context.SaveChangesAsync(cancellationToken);
			_logger.LogInformation("Weather reading saved successfully.");

			return Created();
		}

		[HttpGet]
		public async Task<IActionResult> GetReadingBasedOn([FromQuery] string location, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Fetching weather readings for location: {Location}", location);

			if (string.IsNullOrWhiteSpace(location))
			{
				return BadRequest("Location query parameter is required.");
			}

			var readings = await _context.WeatherReading
				.Where(r => r.Location.Equals(location))
				.Select(r => new WeatherReadingResponse(r))
				.ToListAsync(cancellationToken);

			if (readings.Count == 0)
			{
				return NotFound();
			}

			return Ok(readings);
		}

		#region Commands
		public class ReadingCreateCommand
		{
			public double Temperature { get; set; }
			public double Humidity { get; set; }
			public double Pressure { get; set; }
			public string Location { get; set; }
		}
		#endregion

		#region Resources

		public class WeatherReadingResponse
		{
			public DateTime Timestamp { get; init; }
			public double Temperature { get; init; }
			public double Humidity { get; init; }
			public double Pressure { get; init; }
			public string Location { get; init; }

			public WeatherReadingResponse(WeatherReading reading)
			{
				Timestamp = reading.Timestamp;
				Temperature = reading.Temperature;
				Humidity = reading.Humidity;
				Pressure = reading.Pressure;
				Location = reading.Location;
			}
		}

		#endregion

	}
}
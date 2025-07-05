using WeatherDataService.Repository;
using WeatherDataService.Repository.Entities;

namespace WeatherDataService.IntegrationTests.Helpers
{
	/// <summary>
	/// Used for seeding the database for testing purposes.
	/// </summary>
	public class Seeder(WeatherDbContext context)
	{
		private readonly WeatherDbContext _context = context;

		public async Task SeedAsync()
		{
			await SeedReadings();
			await _context.SaveChangesAsync();
		}

		private async Task SeedReadings()
		{
			var now = DateTime.UtcNow;

			var readings = new List<WeatherReading>
			{
				new() {
					Location = "Amsterdam",
					Humidity = 65.0,
					Pressure = 1013.25,
					Temperature = 18.5,
					Timestamp = now.AddHours(-1)
				},
				new() {
					Location = "Amsterdam",
					Humidity = 65.0,
					Pressure = 1013.25,
					Temperature = 24,
					Timestamp = now.AddHours(-1)
				},
				new() {
					Location = "New York",
					Humidity = 72.3,
					Pressure = 1008.40,
					Temperature = 24.1,
					Timestamp = now.AddHours(-2)
				},
				new() {
					Location = "Tokyo",
					Humidity = 80.0,
					Pressure = 1005.75,
					Temperature = 27.6,
					Timestamp = now.AddHours(-3)
				},
				new() {
					Location = "Cairo",
					Humidity = 35.2,
					Pressure = 1011.60,
					Temperature = 30.8,
					Timestamp = now.AddHours(-4)
				},
				new() {
					Location = "Sydney",
					Humidity = 60.5,
					Pressure = 1020.10,
					Temperature = 22.3,
					Timestamp = now.AddHours(-5)
				}
			};

			await _context.WeatherReading.AddRangeAsync(readings);
			await _context.SaveChangesAsync();
		}
	}
}

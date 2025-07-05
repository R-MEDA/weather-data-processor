using Microsoft.EntityFrameworkCore;
using WeatherDataService.Repository.Entities;

namespace WeatherDataService.Repository
{
	public class WeatherDbContext : DbContext
	{
		public DbSet<WeatherReading> WeatherReading { get; set; }

		public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }
	}
}
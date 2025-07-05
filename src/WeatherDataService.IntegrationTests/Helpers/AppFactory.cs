using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using WeatherDataService.Repository;

namespace WeatherDataService.IntegrationTests.Helpers
{
	/// <summary>
	/// Contains the configruation for setting up the testcontainer
	/// It swaps the production database configured in the Program.cs with the MsSqlContainer
	/// </summary>
	public class AppFactory : WebApplicationFactory<Program>, IAsyncLifetime
	{
		private readonly MsSqlContainer MsSqlContainer;

		public AppFactory()
		{
			MsSqlContainer = new MsSqlBuilder().Build();
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureTestServices(services =>
			{
				// removing the DbContext injected in the Program.cs
				services.Remove(services.Single((s) => s.ServiceType == typeof(DbContextOptions<WeatherDbContext>)));

				// Configure the database connection string to use the Testcontainers instance
				var connectionString = MsSqlContainer.GetConnectionString();
				services.AddDbContext<WeatherDbContext>(options => options.UseSqlServer(connectionString));
			});
		}

		public async Task InitializeAsync()
		{
			Console.WriteLine("Initializing the app factory...");

			await MsSqlContainer.StartAsync();

			// Create scope and seed once
			using var scope = Services.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();

			// Apply migrations
			/**
				Make sure to apply all migrations.
				Other alternatives are:
					1) Running migrations on startup in the App.cs
					2) Generating SQL scripts (STRONGLY RECOMMENDED)-> https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli 
			**/
			if (context.Database.GetPendingMigrations().Any())
			{
				context.Database.Migrate();
			}

			// Seed only once here
			var seeder = new Seeder(context);
			await seeder.SeedAsync();

		}

		public async Task DisposeAsync()
		{
			Console.WriteLine("Disposing the app factory...");

			await MsSqlContainer.DisposeAsync();
		}
	}
}
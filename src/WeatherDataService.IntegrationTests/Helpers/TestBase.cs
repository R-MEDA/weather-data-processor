using Microsoft.Extensions.DependencyInjection;
using WeatherDataService.Repository;

namespace WeatherDataService.IntegrationTests.Helpers
{
	/// <summary>
	/// Wrapper for the integration tests
	/// </summary>
	public class TestBase : IClassFixture<AppFactory>
	{
		public readonly HttpClient _client;
		public readonly IServiceScope _scope;
		public readonly WeatherDbContext _context;

		public TestBase(AppFactory factory)
		{
			_client = factory.CreateClient();
			_scope = factory.Services.CreateScope();
			_context = _scope.ServiceProvider.GetRequiredService<WeatherDbContext>();
		}
	}
}
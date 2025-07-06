using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WeatherDataService.IntegrationTests.Helpers;
using WeatherDataService.IntegrationTests.Helpers.Commands;
using WeatherDataService.IntegrationTests.Helpers.Resources;

namespace WeatherDataService.IntegrationTests
{
	public class WeatherReadingTests(AppFactory factory) : TestBase(factory)
	{

		private const string endpoint = "/weatherreading";

		#region POST /weatherreading

		[Fact]
		public async Task CreateReading_WithValidReading_ReturnsCreatedAndPersists()
		{
			// Arrange
			var payload = new CreateReadingCommand
			{
				Temperature = 23.5,
				Humidity = 55.2,
				Pressure = 1012.3,
				Location = "Barcelona"
			};

			// Act
			var response = await _client.PostAsync(endpoint, Json(payload));

			// Assert
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);

			var saved = await _context.WeatherReading.FirstOrDefaultAsync(r => r.Location == "Barcelona");

			Assert.NotNull(saved);
			Assert.Equal(payload.Temperature, saved.Temperature);
			Assert.Equal(payload.Humidity, saved.Humidity);
			Assert.Equal(payload.Pressure, saved.Pressure);
		}

		[Fact]
		public async Task CreateReading_WithMissingLocation_ReturnsBadRequest()
		{
			// Arrange
			var payload = new CreateReadingCommand
			{
				Temperature = 20.0,
				Humidity = 45.0,
				Pressure = 1009.9
				// Missing Location
			};

			// Act
			var response = await _client.PostAsync(endpoint, Json(payload));

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

		[Fact]
		public async Task CreateReading_WithMalformedJson_ReturnsBadRequest()
		{
			// Arrange
			var content = new StringContent("{ this is not valid json", Encoding.UTF8, "application/json");

			// Act
			var response = await _client.PostAsync(endpoint, content);

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

		#endregion

		#region GET /weatherreading

		[Fact]
		public async Task GetReadingBasedOn_WithValidLocation_ReturnsReadings()
		{
			// Arrange
			var response = await _client.GetAsync("/weatherreading?location=Amsterdam");

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var body = await response.Content.ReadAsStringAsync();

			// Act
			var result = JsonConvert.DeserializeObject<List<WeatherReadingResponse>>(body);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count);
			Assert.All(result, r => Assert.Equal("Amsterdam", r.Location));
		}

		[Fact]
		public async Task GetReadingBasedOn_WithEmptyLocation_ReturnsBadRequest()
		{
			// Act
			var response = await _client.GetAsync("/weatherreading?location=");

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

		[Fact]
		public async Task GetReadingBasedOn_WithMissingLocationParam_ReturnsBadRequest()
		{
			// Act
			var response = await _client.GetAsync(endpoint);

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

		[Fact]
		public async Task GetReadingBasedOn_WithUnknownLocation_ReturnsNotFound()
		{
			// Act
			var response = await _client.GetAsync("/weatherreading?location=NowhereLand");

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		}

		#endregion

		#region Helpers

		private static StringContent Json(object obj) => new(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

		#endregion
	}
}
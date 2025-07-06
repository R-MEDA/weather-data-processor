using System.Net.Http.Json;

class WeatherTcpClient
{
	// HttpClient lifecycle management best practices:
	// https://learn.microsoft.com/dotnet/fundamentals/networking/http/httpclient-guidelines#recommended-use
	private static readonly HttpClient client = new()
	{
		BaseAddress = new Uri("http://localhost:5244"),
	};

	public static async Task Main()
	{
		Random random = new();

		while (true)
		{
			// Simulate weather data
			object data = new
			{
				temperature = random.Next(-5, 36),   // -5°C to 35°C
				humidity = random.Next(40, 101),     // 40% to 100%
				pressure = random.Next(980, 1051),   // 980 hPa to 1050 hPa
				location = "Amsterdam"
			};

			Console.WriteLine($"Sending: {data}");

			try
			{
				var result = await client.PostAsJsonAsync("/weatherreading", data);

				Console.WriteLine($"StatusCode: {result.StatusCode}");
				Console.WriteLine($"Response: {await result.Content.ReadAsStringAsync()}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception occured: {ex.Message}");
			}

			Thread.Sleep(1000);
		}
	}
}
namespace WeatherDataService.IntegrationTests.Helpers.Resources
{
	public class WeatherReadingResponse
	{
		public DateTime Timestamp { get; set; }
		public double Temperature { get; set; }
		public double Humidity { get; set; }
		public double Pressure { get; set; }
		public string Location { get; set; }
	}
}
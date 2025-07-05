namespace WeatherDataService.IntegrationTests.Helpers.Commands
{
	public class CreateReadingCommand
	{
		public double Temperature { get; set; }
		public double Humidity { get; set; }
		public double Pressure { get; set; }
		public string Location { get; set; }
	}
}
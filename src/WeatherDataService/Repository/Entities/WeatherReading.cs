namespace WeatherDataService.Repository.Entities
{
	public class WeatherReading
	{
		public int Id { get; set; }
		public DateTime Timestamp { get; set; }
		public double Temperature { get; set; }
		public double Humidity { get; set; }
		public double Pressure { get; set; }
		public string Location { get; set; }
	}
}
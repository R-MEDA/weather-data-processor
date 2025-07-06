# ✅ Integration Testing with Testcontainers and SQL Server in .NET

This guide walks through how to use **Testcontainers** with **SQL Server** for integration testing in a .NET application.

---

## 📦 1. Install the Testcontainers Package

```bash
dotnet add package Testcontainers.MsSql
```

---

## 🛠 2. Configure and Initialize the Testcontainer

```csharp
private readonly MsSqlContainer MsSqlContainer;

public AppFactory()
{
    MsSqlContainer = new MsSqlBuilder().Build();
}
```

---

## 🔄 3. Swap Real DB with Testcontainer DB in Test Host Configuration

```csharp
protected override void ConfigureWebHost(IWebHostBuilder builder)
{
    builder.ConfigureTestServices(services =>
    {
        // Remove existing DbContext
        services.Remove(services.Single(s => s.ServiceType == typeof(DbContextOptions<WeatherDbContext>)));

        // Inject Testcontainer connection string
        var connectionString = MsSqlContainer.GetConnectionString();
        services.AddDbContext<WeatherDbContext>(options => options.UseSqlServer(connectionString));
    });
}
```

---

## 🔁 4. Implement `IAsyncLifetime` for Setup and Teardown

```csharp
public async Task InitializeAsync()
{
    Console.WriteLine("Initializing the app factory...");
    await MsSqlContainer.StartAsync();
}

public async Task DisposeAsync()
{
    Console.WriteLine("Disposing the app factory...");
    await MsSqlContainer.DisposeAsync();
}
```

---

## 🌱 5. Run Migrations and Seed Database

```csharp
public async Task InitializeAsync()
{
    Console.WriteLine("Initializing the app factory...");
    await MsSqlContainer.StartAsync();

    using var scope = Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();

    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }

    var seeder = new Seeder(context);
    await seeder.SeedAsync();
}
```

---

## ✅ Writing Integration Tests

Here's an example test suite for `WeatherReading` APIs.

```csharp
public class WeatherReadingTests(AppFactory factory) : TestBase(factory)
{
    private const string endpoint = "/weatherreading";

    public class PostReading
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
        public string Location { get; set; }
    }

    public class WeatherReadingResponse
    {
        public DateTime Timestamp { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
        public string Location { get; set; }
    }

    [Fact]
    public async Task Post_WithValidReading_ReturnsCreatedAndPersists()
    {
        var payload = new PostReading
        {
            Temperature = 23.5,
            Humidity = 55.2,
            Pressure = 1012.3,
            Location = "Barcelona"
        };

        var response = await _client.PostAsync(endpoint, Json(payload));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var saved = await _context.WeatherReading.FirstOrDefaultAsync(r => r.Location == "Barcelona");
        Assert.NotNull(saved);
        Assert.Equal(payload.Temperature, saved.Temperature);
        Assert.Equal(payload.Humidity, saved.Humidity);
        Assert.Equal(payload.Pressure, saved.Pressure);
    }

    [Fact]
    public async Task Post_WithMissingLocation_ReturnsBadRequest()
    {
        var payload = new
        {
            Temperature = 20.0,
            Humidity = 45.0,
            Pressure = 1009.9
        };

        var response = await _client.PostAsync(endpoint, Json(payload));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithMalformedJson_ReturnsBadRequest()
    {
        var content = new StringContent("{ this is not valid json", Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(endpoint, content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_WithValidLocation_ReturnsReadings()
    {
        var response = await _client.GetAsync("/weatherreading?location=Amsterdam");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<WeatherReadingResponse>>(body);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal("Amsterdam", r.Location));
    }

    [Fact]
    public async Task Get_WithEmptyLocation_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/weatherreading?location=");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_WithMissingLocationParam_ReturnsBadRequest()
    {
        var response = await _client.GetAsync(endpoint);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_WithUnknownLocation_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/weatherreading?location=NowhereLand");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static StringContent Json(object obj) =>
        new(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
}
```
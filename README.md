# 🌤️ WeatherDataService

A simple .NET Web API for managing and retrieving weather data, with integration testing powered by Testcontainers.

---

## 🧱 Project Structure

* **`WeatherDataService/`** – ASP.NET Core Web API
* **`WeatherDataService.IntegrationTests/`** – Integration test project using Testcontainers

---

## 🚀 Getting Started

### 🔧 Prerequisites

Make sure you have the following installed:

* [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks)
* [Docker](https://www.docker.com/)

---

### 🐳 Running the Application

1. **Start the database container** using Docker Compose:

   ```bash
   docker-compose up -d
   ```

2. **Run the web application**:

   ```bash
   dotnet run --project WeatherDataService
   ```

3. The application should now be available at:

   ```
   http://localhost:5244
   ```

---

## ⚙️ Configuration

* The database connection string is located in:

  ```
  WeatherDataService/appsettings.Development.json
  ```

* If you need to change the connection string, **make sure to update both**:

  * `appsettings.Development.json`
  * `docker-compose.yml`

---

## 📬 Testing the API

You can use the included `WeatherDataService.http` file to send test requests directly from your IDE.

---

## 🧪 Running Tests

The integration tests use **Testcontainers** to automatically spin up a fresh SQL Server container during test execution.

To run the tests:

```bash
dotnet test
```

* ✅ No need for manual DB setup – everything is automated!
* 🧹 The containers are cleaned up after tests finish.

---
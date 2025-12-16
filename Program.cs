var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

var messages = new[]
{
    "Bundle up out there!",
    "Keep a coat handy.",
    "Sweater weather.",
    "Pretty comfortable today.",
    "Enjoy the mild breeze.",
    "Great day for the park.",
    "Perfect evening weather.",
    "Stay hydrated!",
    "Maybe find some shade.",
    "Stay indoors if you can!"
};




WeatherForecast[] CreateForecast() =>
    Enumerable.Range(1, 5).Select(index =>
    {
        var summaryIndex = Random.Shared.Next(summaries.Length);
        return new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[summaryIndex],
            messages[summaryIndex]
        );
    }).ToArray();

app.MapGet("/weatherforecastOutput", () =>
{
    var forecast = CreateForecast();
    var lines = forecast.Select(f =>
        $"{f.Date:dd/MM/yy}: | {f.TemperatureC}°C / {f.TemperatureF}°F | {f.Summary} | {f.Message}\n");

    return Results.Text(string.Join(Environment.NewLine, lines));
})
.WithName("GetWeatherForecastOutput");

app.MapGet("/", () => Results.Content("""
<!doctype html>
<html lang="en">
  <body>

    <h3>Weather Forecast</h3>
    <p>Next 5 days weather forecast:</p>
    <pre id="lines"></pre>

    <script>

      fetch("/weatherforecastOutput")
        .then(r => r.text())
        .then(text => {
          document.getElementById("lines").textContent = text;
        })
        .catch(err => {
          document.getElementById("lines").textContent = String(err);
        });
    </script>
  </body>
</html>
""", "text/html"));



app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary, string? Message)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


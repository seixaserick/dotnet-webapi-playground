namespace Models;

public class WeatherForecast
{
    public DateTime date { get; set; }

    public int temperatureC { get; set; }

    public int temperatureF => 32 + (int)(temperatureC / 0.5556);

    public string? summary { get; set; }

    public DateTime forecastedAt { get; set; } = DateTime.UtcNow; //if cached, you will see this 'freezed date' when forecast returns from redis cache, otherwize it will be DateTime.UtcNow

}

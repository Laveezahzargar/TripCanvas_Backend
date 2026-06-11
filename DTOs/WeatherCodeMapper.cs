namespace P6_Travel_Planner_Backend.DTOs
{
    public static class WeatherCodeMapper
    {
        public static string ToCondition(int code)
        {
            return code switch
            {
                0 => "Clear sky ☀️",
                1 or 2 or 3 => "Partly cloudy ⛅",
                45 or 48 => "Fog 🌫",
                51 or 53 or 55 => "Drizzle 🌦",
                61 or 63 or 65 => "Rain 🌧",
                71 or 73 or 75 => "Snow ❄️",
                95 => "Thunderstorm ⛈",
                _ => "Unknown"
            };
        }
    }
}

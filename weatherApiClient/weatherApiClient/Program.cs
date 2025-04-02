using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class Program
{
    // API Key for WeatherAPI.com 
    private static readonly string apiKey = "a0e3039d410843f8b77135108250204";

    static async Task Main()
    {
        while (true) // Loop to allow multiple city searches until the user exits
        {
            Console.Write("Enter a city (or type 'exit' to quit): ");
            string userCity = Console.ReadLine()?.Trim(); // Read user input and remove extra spaces

            // Check if the user input is empty
            if (string.IsNullOrEmpty(userCity))
            {
                Console.WriteLine("City name cannot be empty. Please try again.");
                continue; // Restart the loop if input is empty
            }

            // Allow user to exit the program
            if (userCity.ToLower() == "exit")
            {
                Console.WriteLine("Goodbye!");
                break; // Exit the loop and end the program
            }

            // Fetch and display weather data for the entered city
            await GetWeatherData(userCity);
        }
    }

    static async Task GetWeatherData(string city)
    {
        // Construct the API URL using the provided city name
        string apiUrl = $"https://api.weatherapi.com/v1/current.json?key={apiKey}&q={city}&aqi=no";
        using HttpClient client = new HttpClient(); // Create an HttpClient instance

        try
        {
            // Send a GET request to the weather API
            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode) // Check if the request was successful (Status Code 200)
            {
                string json = await response.Content.ReadAsStringAsync(); // Read response content as string
                JObject weatherData = JObject.Parse(json); // Parse JSON response into JObject

                // Display weather information
                Console.WriteLine("\nWeather Information for " + city);
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"Temperature: {weatherData["current"]["temp_c"]}°C");
                Console.WriteLine($"Condition: {weatherData["current"]["condition"]["text"]}");
                Console.WriteLine($"Humidity: {weatherData["current"]["humidity"]}%");
                Console.WriteLine($"Wind Speed: {weatherData["current"]["wind_kph"]} km/h\n");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound) // If city is not found
            {
                Console.WriteLine($"Error: City '{city}' not found. Please enter a valid city.\n");
            }
            else // Handle other HTTP response errors
            {
                Console.WriteLine($"Error: Unable to retrieve weather data (Status Code: {response.StatusCode}).\n");
            }
        }
        catch (HttpRequestException) // Handle network-related errors
        {
            Console.WriteLine("Network error: Unable to connect to the weather service. Check your internet connection.\n");
        }
        catch (Exception ex) // Handle unexpected errors
        {
            Console.WriteLine($"Unexpected error: {ex.Message}\n");
        }
    }
}

using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static readonly string[] zodiacSigns = new[]
    {
        "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo",
        "Libra", "Scorpio", "Sagittarius", "Capricorn", "Aquarius", "Pisces"
    };

    static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== HOROSCOPE ===");

            string sign = ChooseZodiacSign();
            string day = ChooseDay();

            await GetHoroscope(sign, day);

            Console.WriteLine("\nWould you like to view another horoscope?");
            Console.WriteLine("1. Yes");
            Console.WriteLine("2. No");
            Console.Write("Your choice (1 or 2): ");
            var repeat = Console.ReadLine();
            if (repeat?.Trim() != "1") break;
        }

        Console.WriteLine("Goodbye!");
    }

    static string ChooseZodiacSign()
    {
        Console.WriteLine("\nChoose your zodiac sign:");

        for (int i = 0; i < zodiacSigns.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {zodiacSigns[i]}");
        }

        while (true)
        {
            Console.Write("Your choice (1–12): ");
            if (int.TryParse(Console.ReadLine(), out int choice) &&
                choice >= 1 && choice <= 12)
            {
                return zodiacSigns[choice - 1];
            }

            Console.WriteLine("Invalid choice. Please try again.");
        }
    }

    static string ChooseDay()
    {
        Console.WriteLine("\nChoose the day:");
        Console.WriteLine("1. Today");
        Console.WriteLine("2. Tomorrow");

        while (true)
        {
            Console.Write("Your choice (1 or 2): ");
            string input = Console.ReadLine()?.Trim();

            if (input == "1")
                return "today";
            else if (input == "2")
                return "tomorrow";

            Console.WriteLine("Invalid choice. Please try again.");
        }
    }

    static async Task GetHoroscope(string sign, string day)
    {
        string url = $"https://horoscope-app-api.vercel.app/api/v1/get-horoscope/daily?sign={sign}&day={day}";

        using HttpClient client = new HttpClient();

        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("data", out JsonElement data))
            {
                string date = data.GetProperty("date").GetString();
                string horoscope = data.GetProperty("horoscope_data").GetString();

                Console.WriteLine($"\nHoroscope for {sign} on {date}:\n{horoscope}\n");
            }
            else
            {
                Console.WriteLine("Could not find horoscope data in the API response.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving horoscope: {ex.Message}");
        }
    }
}

using System.Text;
using System.Text.Json;

namespace Client;

public class Question
{
    public string Id { get; set; }
    public string Answer { get; set; }
    public string Category { get; set; }

    private static readonly string apiUrl = "https://89bf-194-58-31-160.ngrok-free.app/questions";
    private static readonly string preferencesUrl = "https://89bf-194-58-31-160.ngrok-free.app/preferences";

    public static async Task<List<Question>> GetAllQuestions()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Response: {jsonResponse}");

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var questions = JsonSerializer.Deserialize<List<Question>>(jsonResponse, options);
                    
                    if (questions == null)
                    {
                        Console.WriteLine("Deserialization returned null");
                        return new List<Question>();
                    }

                    foreach (var q in questions)
                    {
                        Console.WriteLine($"Question: Id={q.Id ?? "null"}, Answer={q.Answer ?? "null"}, Category={q.Category ?? "null"}");
                    }

                    return questions;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
                    return new List<Question>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetAllQuestions: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<Question>();
            }
        }
    }

    public static async Task<bool> SaveUserPreference(string login, string category, int rating)
    {
        var preference = new
        {
            id = Guid.NewGuid().ToString().ToUpper(),
            login = login,
            answer = rating,
            category = category
        };

        string json = JsonSerializer.Serialize(preference);
        Console.WriteLine($"Sending preference: {json}");

        using (HttpClient client = new HttpClient())
        {
            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(preferencesUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Preference saved successfully. Response: {responseContent}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Error saving preference: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception saving preference: {ex.Message}");
                return false;
            }
        }
    }
}

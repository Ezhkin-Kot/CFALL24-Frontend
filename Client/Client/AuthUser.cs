using System.Text;
using System.Text.Json;

namespace Client;

public class AuthUser
{
    public string Login { get; set; }
    public string Password { get; set; }

    public static AuthUser? CurrentUser { get; private set; }

    private static readonly string apiUrl = "https://830f-194-58-31-160.ngrok-free.app/users";

    public async Task<bool> CreateUser(string loginStr, string passwordStr)
    {
        var user = new
        {
            login = loginStr,
            password = passwordStr
        };

        string json = JsonSerializer.Serialize(user);
        Console.WriteLine(json);

        using (HttpClient client = new HttpClient())
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("User created successfully.");
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }

            return response.IsSuccessStatusCode;
        }
    }

    public static async Task<bool> LoginUser(string loginStr, string passwordStr)
    {
        var loginData = new
        {
            login = loginStr,
            password = passwordStr
        };

        string json = JsonSerializer.Serialize(loginData);

        using (HttpClient client = new HttpClient())
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"{apiUrl}/login", content);

            if (response.IsSuccessStatusCode)
            {
                CurrentUser = new AuthUser { Login = loginStr, Password = passwordStr };
                Console.WriteLine("User logged in successfully.");
                return true;
            }
            else
            {
                Console.WriteLine($"Login failed: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
                return false;
            }
        }
    }

    public static void Logout()
    {
        CurrentUser = null;
    }
}
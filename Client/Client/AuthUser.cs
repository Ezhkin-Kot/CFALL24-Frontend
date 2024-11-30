using System.Text;
using System.Text.Json;

namespace Client;

public class AuthUser
{
    public string Login { get; set; }
    public string Password { get; set; }

    public static AuthUser? CurrentUser { get; private set; }

    private static readonly string apiUrl = "https://830f-194-58-31-160.ngrok-free.app/users";
    private static readonly string loginUrl = "https://830f-194-58-31-160.ngrok-free.app/users";

    public async Task<bool> CreateUser(string loginStr, string passwordStr)
    {
        var user = new
        {
            login = loginStr,
            password = passwordStr
        };

        string json = JsonSerializer.Serialize(user);
        Console.WriteLine($"Creating user with data: {json}");

        using (HttpClient client = new HttpClient())
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(apiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Create user response: {response.StatusCode}, Content: {responseContent}");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("User created successfully.");
            }
            else
            {
                Console.WriteLine($"Error creating user: {response.StatusCode}, {responseContent}");
            }

            return response.IsSuccessStatusCode;
        }
    }

    public static async Task<bool> LoginUser(string loginStr, string passwordStr)
    {
        try
        {
            var loginData = new
            {
                login = loginStr,
                password = passwordStr
            };

            string json = JsonSerializer.Serialize(loginData);
            Console.WriteLine($"Attempting login with data: {json}");

            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(loginUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Login response: {response.StatusCode}, Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    CurrentUser = new AuthUser { Login = loginStr, Password = passwordStr };
                    Console.WriteLine($"User logged in successfully: {loginStr}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Login failed: {response.StatusCode}, {responseContent}");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during login: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    public static void Logout()
    {
        CurrentUser = null;
        Console.WriteLine("User logged out");
    }
}
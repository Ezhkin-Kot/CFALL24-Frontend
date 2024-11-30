using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client;

public class AuthUser
{
    public async Task<bool> CreateUser(string loginStr, string passwordStr)
    {
        string apiUrl = "https://0c50-194-58-31-160.ngrok-free.app/users";

        var user = new
        {
            login = loginStr,
            password = passwordStr
        };

        string json = System.Text.Json.JsonSerializer.Serialize(user);
        Console.WriteLine(json);

        using (HttpClient client = new HttpClient())
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Пользователь успешно добавлен.");
            }
            else
            {
                Console.WriteLine($"Ошибка: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }
            return response.IsSuccessStatusCode;
        }
    }
}
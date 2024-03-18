using System.Text.Json;
using WebClient.Models;

namespace WebClient.Services;

public static class HttpContentDeserializer
{
    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    public static async Task<DeserializationResult<T>> Deserialize<T>(HttpResponseMessage? response)
        where T : class
    {
        if (response is null)
        {
            return DeserializationResult<T>.ConnectionError();
        }

        try
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(string) && content is T failedContent)
                {
                    return DeserializationResult<T>.Failed(failedContent);
                }

                return DeserializationResult<T>.Failed();
            }

            if (typeof(T) == typeof(string) && content is T stringContent)
            {
                return DeserializationResult<T>.Success(stringContent);
            }

            var deserializedResponse = JsonSerializer.Deserialize<T>(content, _options) ?? default!;

            if (deserializedResponse is null)
            {
                return DeserializationResult<T>.Failed();
            }
            else
            {
                return DeserializationResult<T>.Success(deserializedResponse);
            }
        }
        catch
        {
            // FALLTHROUGH
        }

        return DeserializationResult<T>.Failed();
    }
}

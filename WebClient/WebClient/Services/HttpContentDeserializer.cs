using System.Text.Json;
using WebClient.Models;

namespace WebClient.Services;

public static class HttpContentDeserializer
{
    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Deserializes the content of type <typeparamref name="T"/> from <paramref name="response"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <returns></returns>
    public static async Task<DeserializationResult<T>> Deserialize<T>(HttpResponseMessage? response)
        where T : class
    {
        // if there is no response, no connection was made to the service
        if (response is null)
        {
            return DeserializationResult<T>.ConnectionError();
        }

        try
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // a connection was made, the service did not return a 200 reply, but a message was attached
                if (typeof(T) == typeof(string) && content is T failedContent)
                {
                    return DeserializationResult<T>.Failed(failedContent);
                }

                // a connection was made, but there was an unsuccessful connection to the endpoint
                return DeserializationResult<T>.Failed();
            }

            if (typeof(T) == typeof(string) && content is T stringContent)
            {
                return DeserializationResult<T>.Success(stringContent);
            }

            var deserializedResponse = JsonSerializer.Deserialize<T>(content, _options) ?? default!;

            // could not deserialize the response, hence, failed
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

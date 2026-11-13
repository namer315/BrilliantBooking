using System.Text.Json;

namespace BrilliantBooking.Extensions;

public static class SessionExtensions
{
    /// <summary>
    /// Serializes an object to JSON and stores it in the session under the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the object to store in the session.</typeparam>
    /// <param name="session">The session object used to store the data.</param>
    /// <param name="key">The key under which to store the serialized object.</param>
    /// <param name="value">The object to be stored in the session.</param>
    public static void SetObject<T>(this ISession session , string key , T value)
    {
        // Serialize the object to a JSON string and store it in the session
        session.SetString(key , JsonSerializer.Serialize(value));
    }

    /// <summary>
    /// Retrieves an object from the session by its key and deserializes it from JSON.
    /// </summary>
    /// <typeparam name="T">The type of the object to retrieve from the session.</typeparam>
    /// <param name="session">The session object used to retrieve the data.</param>
    /// <param name="key">The key under which the object is stored.</param>
    /// <returns>The deserialized object if it exists in the session; otherwise, the default value of T.</returns>
    public static T GetObject<T>(this ISession session , string key)
    {
        // Retrieve the JSON string from the session
        var value = session.GetString(key);

        // If the key doesn't exist in the session, return the default value for the type
        // Otherwise, deserialize the JSON string back into the object and return it
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}

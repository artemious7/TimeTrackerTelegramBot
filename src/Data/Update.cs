using System.Text.Json.Serialization;

namespace ReportCounterBot.Data;

public class Update
{
    [JsonPropertyName("message")] public Message Message { get; set; } = null!;
}
public class Message
{
    [JsonPropertyName("chat")] public Chat Chat { get; set; } = null!;
    [JsonPropertyName("text")] public string? Text { get; set; }
    [JsonPropertyName("message_id")] public long Id { get; set; }
}
public class Chat
{
    [JsonPropertyName("id")] public long Id { get; set; }
}

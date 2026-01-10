using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;

namespace TodoApi.Services;

public class NotificationService
{
    // Maps UserId -> List of active channels (a user might have multiple tabs open)
    private readonly ConcurrentDictionary<string, List<Channel<string>>> _userChannels = new();

    public ChannelReader<string> Subscribe(string userId)
    {
        var channel = Channel.CreateUnbounded<string>();
        
        _userChannels.AddOrUpdate(userId, 
            new List<Channel<string>> { channel }, 
            (key, list) => {
                lock (list) { list.Add(channel); }
                return list;
            });

        return channel.Reader;
    }

    public void Unsubscribe(string userId, ChannelReader<string> reader)
    {
        if (_userChannels.TryGetValue(userId, out var channels))
        {
            lock (channels)
            {
                // Find the channel associated with this reader and remove it
                var channelToRemove = channels.FirstOrDefault(c => c.Reader == reader);
                if (channelToRemove != null)
                {
                    channels.Remove(channelToRemove);
                    channelToRemove.Writer.TryComplete();
                }
            }
        }
    }

    public async Task NotifyUserAsync(string userId, string eventType, object data)
    {
        if (_userChannels.TryGetValue(userId, out var channels))
        {
            var payload = new { type = eventType, payload = data };
            var json = JsonSerializer.Serialize(payload);
            var sseMessage = $"data: {json}\n\n";

            List<Channel<string>> channelsCopy;
            lock (channels)
            {
                channelsCopy = channels.ToList();
            }

            foreach (var channel in channelsCopy)
            {
                await channel.Writer.WriteAsync(sseMessage);
            }
        }
    }
}
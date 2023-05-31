using Confluent.Kafka;
using MessageProcessor.Models;
using MessageProcessor.Repositories.Interfaces;
using Newtonsoft.Json;
using Serilog;

namespace MessageProcessor.Services;

public class KafkaService : IKafkaService
{
    private readonly IUserSongRepository _userSongRepository;
    
    public KafkaService(IUserSongRepository userSongRepository)
    {
        _userSongRepository = userSongRepository;
    }
    
    public async Task ProcessMessages()
    {
        Log.Logger.Information("Kafka message processor started");

        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "test-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
        {
            string[] topics = { "like", "unlike" };
            consumer.Subscribe(topics);

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                while (true)
                {
                    var consumeResult = consumer.Consume(cts.Token);

                    Log.Logger.Information($"Received message. Topic: {consumeResult.Topic} with value: {consumeResult.Message.Value}");
                    var userSong = JsonConvert.DeserializeObject<UserSong>(consumeResult.Message.Value);

                    switch (consumeResult.Topic)
                    {
                        case "like":
                            await AddLike(userSong);
                            break;
                        case "unlike":
                            await Unlike(userSong);
                            break;
                    }
                }
            }
            catch (OperationCanceledException exception)
            {
                Log.Logger.Error($"Kafka exception: {exception.Message}");
            }
            finally
            {
                consumer.Close();
            }
        }
    }

    private async Task AddLike(UserSong userSong)
    {
        if (await _userSongRepository.GetLike(userSong.UserId, userSong.SongId) == null)
        {
            await _userSongRepository.AddLike(userSong);
        }
    }

    private async Task Unlike(UserSong userSong)
    {
        var usrsong = await _userSongRepository.GetLike(userSong.UserId, userSong.SongId);
        
        if (usrsong != null)
        {
            await _userSongRepository.RemoveLike(usrsong);
        }
    }
}
namespace MessageProcessor.Services;

public interface IKafkaService
{
    Task ProcessMessages();
}
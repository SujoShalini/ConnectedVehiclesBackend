using Azure.Messaging.ServiceBus;
using ConnectedVehicles.Models;
using System.Text;
using System.Threading.Tasks;

public class VehiceStatusService
{
    private readonly ServiceBusClient _client;
    private readonly string _queueName;

    public VehiceStatusService(string connectionString, string queueName)
    {
        _client = new ServiceBusClient(connectionString);
        _queueName = queueName;
    }

    public async Task<List<string>> ReceiveMessagesAsync()
    {
        var messagesList = new List<string>();

        await using var receiver = _client.CreateReceiver(_queueName);
        var messages = await receiver.ReceiveMessagesAsync(maxMessages: 10, maxWaitTime: TimeSpan.FromSeconds(1));

        if (messages != null)
        {
            foreach (var message in messages)
            {
                var body = Encoding.UTF8.GetString(message.Body.ToArray());
                await receiver.CompleteMessageAsync(message);
                messagesList.Add(body);
            }
        }

        return messagesList;
    }
    public async Task SendMessageAsync(VehicleStatus vehicleStatus)
    {
        await using var sender = _client.CreateSender(_queueName);
        var message = new ServiceBusMessage(System.Text.Json.JsonSerializer.Serialize(vehicleStatus));
        await sender.SendMessageAsync(message);
    }
}

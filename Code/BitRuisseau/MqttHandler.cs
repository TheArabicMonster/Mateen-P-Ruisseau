using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitRuisseau
{
    public class MqttHandler
    {
        private IMqttClient mqttClient;
        private MqttClientOptions options;
        private string topic;

        public MqttHandler(string broker, int port, string username, string password, string topic)
        {
            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();
            this.topic = topic;

            options = new MqttClientOptionsBuilder()
                .WithTcpServer(broker, port)
                .WithCredentials(username, password)
                .WithClientId(Guid.NewGuid().ToString())
                .WithCleanSession()
                .Build();
        }

        public async Task ConnectAsync()
        {
            try
            {
                var connectResult = await mqttClient.ConnectAsync(options);

                if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
                {
                    Console.WriteLine("Connected to MQTT broker successfully.");
                    mqttClient.ApplicationMessageReceivedAsync += HandleReceivedMessage;
                    await mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
                        .WithTopicFilter(f => f.WithTopic(topic).WithNoLocal(true))
                        .Build());

                    // Envoyer un message de type 1 (DEMANDE_CATALOGUE)
                    await SendMessageAsync(MessageType.DEMANDE_CATALOGUE);
                }
                else
                {
                    Console.WriteLine($"Failed to connect to MQTT broker: {connectResult.ResultCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to MQTT broker: {ex.Message}");
            }
        }

        private async Task HandleReceivedMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            string receivedMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            Console.WriteLine($"Received message: {receivedMessage}");
        }

        public async Task SendCatalogAsync(string selectedFolderPath)
        {
            var catalog = GetMusicList(selectedFolderPath);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(catalog)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await mqttClient.PublishAsync(message);
        }

        public async Task SendFileAsync(string filePath)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(fileBytes)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await mqttClient.PublishAsync(message);
        }

        public static string GetMusicList(string selectedFolderPath)
        {
            var files = Directory.GetFiles(selectedFolderPath, "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".mp3") || s.EndsWith(".mp4"))
                .ToList();

            return JsonConvert.SerializeObject(files);
        }

        private async Task SendTestMessageAsync()
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("test")
                .WithPayload("Message de test")
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await mqttClient.PublishAsync(message);
        }

        private async Task SendMessageAsync(MessageType messageType)
        {
            var envelope = new
            {
                MessageType = (int)messageType,
                SenderId = Guid.NewGuid().ToString()
            };

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(JsonConvert.SerializeObject(envelope))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false)
                .Build();

            await mqttClient.PublishAsync(message);
            Console.WriteLine(message);
        }
    }
}
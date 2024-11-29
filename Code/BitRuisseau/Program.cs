using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;

namespace BitRuisseau
{
    internal static class Program
    {
        private static IMqttClient mqttClient;
        public static string selectedFolderPath;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        [Obsolete]
        static async Task Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            string broker = "blue.section-inf.ch";
            int port = 1883;
            string clientId = "mateen";
            string topic = "test";
            string username = "ict";
            string password = "321";

            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(broker, port) // MQTT broker address and port
                .WithCredentials(username, password) // Set username and password
                .WithClientId(clientId)
                .WithCleanSession()
                .Build();

            try
            {
                // Connect to MQTT broker
                var connectResult = await mqttClient.ConnectAsync(options);

                if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
                {
                    Console.WriteLine("Connected to MQTT broker successfully.");

                    // Subscribe to a topic
                    await mqttClient.SubscribeAsync(topic);

                    // Callback function when a message is received
                    mqttClient.ApplicationMessageReceivedAsync += e =>
                    {
                        try
                        {
                            string message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                            Console.WriteLine($"Received message: {message}");

                            if (message.StartsWith("hello"))
                            {
                                SendMusicList(topic);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing received message: {ex.Message}");
                        }

                        return Task.CompletedTask;
                    };

                    Application.Run(new Form1());
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

        private static void SendMusicList(string topic)
        {
            try
            {
                if (!string.IsNullOrEmpty(selectedFolderPath) && Directory.Exists(selectedFolderPath))
                {
                    string[] mp3Files = Directory.GetFiles(selectedFolderPath, "*.mp3");
                    string[] mp4Files = Directory.GetFiles(selectedFolderPath, "*.mp4");

                    var musicFiles = mp3Files.Concat(mp4Files).Select(Path.GetFileName).ToList();
                    string json = JsonConvert.SerializeObject(musicFiles);

                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic(topic)
                        .WithPayload(json)
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                        .Build();

                    mqttClient.PublishAsync(message);
                }
                else
                {
                    Console.WriteLine("No folder selected or folder does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending music list: {ex.Message}");
            }
        }
    }
}
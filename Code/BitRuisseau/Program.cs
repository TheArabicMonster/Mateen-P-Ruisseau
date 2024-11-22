using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace BitRuisseau
{
    internal static class Program
    {
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
            string broker = "inf-n510-p301";
            int port = 1883;
            string clientId = Guid.NewGuid().ToString();
            string topic = "test";
            string username = "ict";
            string password = "321";

            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()

            .WithTcpServer(broker, port) // MQTT broker address and port
            .WithCredentials(username, password) // Set username and password
            .WithClientId(clientId)
            .WithCleanSession()
            .WithTls(
                o =>
                {
                    //// The used public broker sometimes has invalid certificates. This sample accepts all
                    //// certificates. This should not be used in live environments.
                    //o.CertificateValidationHandler = _ => true;

                    //// The default value is determined by the OS. Set manually to force version.
                    //o.SslProtocol = SslProtocols.Tls12;

                    //// Please provide the file path of your certificate file. The current directory is /bin.
                    //var certificate = new X509Certificate("/opt/emqxsl-ca.crt", "");
                    //o.Certificates = new List<X509Certificate> { certificate };
                }
            )
            .Build();

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
                    Console.WriteLine($"Received message: {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)}");
                    return Task.CompletedTask;
                };
            }
            else
            {
                Console.WriteLine($"Failed to connect to MQTT broker: {connectResult.ResultCode}");
            }
            Application.Run(new Form1());
        }
    }
}
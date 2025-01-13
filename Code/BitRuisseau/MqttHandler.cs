using MQTTnet;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
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
        private string selectedFolderPath;

        public MqttHandler(string broker, int port, string username, string password, string topic, string selectedFolderPath)
        {
            var factory = new MqttClientFactory();
            mqttClient = factory.CreateMqttClient();
            this.topic = topic;
            this.selectedFolderPath = selectedFolderPath;

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

            try
            {
                // D�s�rialiser le message re�u en un objet JSON
                var envelope = JsonConvert.DeserializeObject<GenericEnvelope>(receivedMessage);

                // G�rer les diff�rents types de messages re�us
                switch (envelope.MessageType)
                {
                    case MessageType.ENVOIE_CATALOGUE:
                        Console.WriteLine("Catalogue re�u.");
                        await SendMessageAsync(MessageType.ENVOIE_CATALOGUE, GetMusicList(selectedFolderPath));
                        break;

                    case MessageType.ENVOIE_FICHIER:
                        Console.WriteLine("Fichier re�u.");
                        HandleFileReceived(envelope.EnveloppeJson);
                        break;

                    case MessageType.DEMANDE_CATALOGUE:
                        Console.WriteLine("Demande de catalogue re�ue.");
                        await SendMessageAsync(MessageType.ENVOIE_CATALOGUE);
                        break;

                    case MessageType.DEMANDE_FICHIER:
                        Console.WriteLine("Demande de fichier re�ue.");
                        HandleFileRequest(envelope.EnveloppeJson);
                        break;

                    default:
                        Console.WriteLine("Message inconnu.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling received message: {ex.Message}");
            }
        }

        // Traiter le catalogue re�u (par exemple, afficher ou utiliser les fichiers dans le catalogue)
        private void HandleCatalogReceived(string catalogJson)
        {
            try
            {
                // D�s�rialiser le catalogue re�u
                var catalog = JsonConvert.DeserializeObject<List<string>>(catalogJson);
                Console.WriteLine("Fichiers dans le catalogue :");
                foreach (var file in catalog)
                {
                    Console.WriteLine(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing catalog: {ex.Message}");
            }
        }

        // Traiter le fichier re�u (par exemple, enregistrer le fichier)
        private void HandleFileReceived(string fileJson)
        {
            try
            {
                // D�s�rialiser les donn�es du fichier re�u
                var fileEnvelope = JsonConvert.DeserializeObject<FileEnvelope>(fileJson);
                byte[] fileBytes = Convert.FromBase64String(fileEnvelope.FileData);

                // Sauvegarder le fichier re�u
                string filePath = Path.Combine(selectedFolderPath, fileEnvelope.FileName);
                File.WriteAllBytes(filePath, fileBytes);
                Console.WriteLine($"Fichier {fileEnvelope.FileName} re�u et sauvegard�.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file: {ex.Message}");
            }
        }

        // Traiter la demande de fichier sp�cifique
        private void HandleFileRequest(string fileRequestJson)
        {
            try
            {
                var fileRequest = JsonConvert.DeserializeObject<FileRequestEnvelope>(fileRequestJson);
                Console.WriteLine($"Demande du fichier {fileRequest.FileName} re�ue.");

                // a compl�ter
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file request: {ex.Message}");
            }
        }

        // Obtenir la liste des fichiers du catalogue
        public static string GetMusicList(string selectedFolderPath)
        {
            var files = Directory.GetFiles(selectedFolderPath, "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".mp3") || s.EndsWith(".mp4"))
                .Select(Path.GetFileName)
                .ToList();

            return JsonConvert.SerializeObject(files);
        }

        // Envoi d'un message g�n�rique
        public async Task SendMessageAsync(MessageType messageType, string enveloppeJson = null)
        {
            var envelope = new
            {
                MessageType = (int)messageType,
                SenderId = Guid.NewGuid().ToString(),
                EnveloppeJson = enveloppeJson
            };

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(JsonConvert.SerializeObject(envelope))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false)
                .Build();

            await mqttClient.PublishAsync(message);
            Console.WriteLine(message);
            Debug.WriteLine(message);   
        }   

        // Envoyer un fichier
        public async Task SendFileAsync(string fileName, string filePath)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string fileData = Convert.ToBase64String(fileBytes);

            var fileEnvelope = new FileEnvelope
            {
                FileName = fileName,
                FileData = fileData
            };

            await SendMessageAsync(MessageType.ENVOIE_FICHIER, JsonConvert.SerializeObject(fileEnvelope));
        }
    }
}
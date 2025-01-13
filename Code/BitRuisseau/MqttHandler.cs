using MQTTnet;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace BitRuisseau
{
    public class MqttHandler
    {
        private IMqttClient mqttClient;
        private MqttClientOptions options;
        private string topic;
        private string selectedFolderPath;
        private string senderID;

        public MqttHandler(string broker, int port, string username, string password, string topic, string selectedFolderPath,string _senderId)
        {
            var factory = new MqttClientFactory();
            mqttClient = factory.CreateMqttClient();
            this.topic = topic;
            this.selectedFolderPath = selectedFolderPath;
            senderID = _senderId;

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
                    await SendMessageAsync();
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
                // Désérialiser le message reçu en un objet JSON
                var envelope = JsonConvert.DeserializeObject<GenericEnvelope>(receivedMessage);

                // Gérer les différents types de messages reçus
                switch (envelope.MessageType)
                {
                    case MessageType.ENVOIE_CATALOGUE:
                        Console.WriteLine("Catalogue reçu.");
                        //await SendMessageAsync(MessageType.ENVOIE_CATALOGUE, GetMusicList(selectedFolderPath));
                        break;

                    case MessageType.ENVOIE_FICHIER:
                        Console.WriteLine("Fichier reçu.");
                        HandleFileReceived(envelope.EnveloppeJson);
                        break;

                    case MessageType.DEMANDE_CATALOGUE:
                        FileSerelizer fileSerelizer = new FileSerelizer();
                        List<FileInfo> files = GetMusicList(this.selectedFolderPath);
                        GenericEnvelope envelop = new GenericEnvelope(MessageType.ENVOIE_CATALOGUE,senderID);
                        string payload = fileSerelizer.SerelizeCatalog(fileSerelizer.SerelizeFilesToTagLibFiles(files),envelop);
                        Console.WriteLine("Demande de catalogue reçue.");
                        await SendMessageAsync();
                        break;

                    case MessageType.DEMANDE_FICHIER:
                        Console.WriteLine("Demande de fichier reçue.");
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

        // Traiter le catalogue reçu (par exemple, afficher ou utiliser les fichiers dans le catalogue)
        private void HandleCatalogReceived(string catalogJson)
        {
            try
            {
                // Désérialiser le catalogue reçu
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

        // Traiter le fichier reçu (par exemple, enregistrer le fichier)
        private void HandleFileReceived(string fileJson)
        {
            try
            {
                // Désérialiser les données du fichier reçu
                var fileEnvelope = JsonConvert.DeserializeObject<FileEnvelope>(fileJson);
                byte[] fileBytes = Convert.FromBase64String(fileEnvelope.FileData);

                // Sauvegarder le fichier reçu
                string filePath = Path.Combine(selectedFolderPath, fileEnvelope.FileName);
                System.IO.File.WriteAllBytes(filePath, fileBytes);
                Console.WriteLine($"Fichier {fileEnvelope.FileName} reçu et sauvegardé.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file: {ex.Message}");
            }
        }

        // Traiter la demande de fichier spécifique
        private void HandleFileRequest(string fileRequestJson)
        {
            try
            {
                var fileRequest = JsonConvert.DeserializeObject<FileRequestEnvelope>(fileRequestJson);
                Console.WriteLine($"Demande du fichier {fileRequest.FileName} reçue.");

                // a compléter
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file request: {ex.Message}");
            }
        }

        // Obtenir la liste des fichiers du catalogue
        public static List<FileInfo> GetMusicList(string selectedFolderPath)
        {
            List<FileInfo> files = new List<FileInfo>();
            DirectoryInfo folder = new DirectoryInfo(selectedFolderPath);
            if (folder.Exists)
            {
                files = folder.GetFiles().ToList();
            }

            return files;
        }

        // Envoi d'un message générique
        public async Task SendMessageAsync(string payload = null)
        {
            FileSerelizer fileSerelizer = new FileSerelizer();
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(JsonConvert.SerializeObject(payload == null ? fileSerelizer.SerelizeCatalog(fileSerelizer.SerelizeFilesToTagLibFiles(GetMusicList(this.selectedFolderPath)), new GenericEnvelope(MessageType.ENVOIE_CATALOGUE,senderID)) : payload))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false)
                .Build();

            await mqttClient.PublishAsync(message);
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }

        // Envoyer un fichier
        /*public async Task SendFileAsync(string fileName, string filePath)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileData = Convert.ToBase64String(fileBytes);

            var fileEnvelope = new FileEnvelope
            {
                FileName = fileName,
                FileData = fileData
            };

            await SendMessageAsync(MessageType.ENVOIE_FICHIER, JsonConvert.SerializeObject(fileEnvelope));
        }*/
    }
}
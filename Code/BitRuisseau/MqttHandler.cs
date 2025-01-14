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
                // Désérialiser le message reçu en un objet JSON
                var envelope = JsonConvert.DeserializeObject<Message>(receivedMessage);
                MessageBox.Show(envelope.MessageType.ToString());
                // Gérer les différents types de messages reçus
                
                if(envelope.MessageType == 0)
                {
                    HandleCatalogReceived(receivedMessage);
                }
                else if (envelope.MessageType == 1)
                {
                    await SendMusicCata(selectedFolderPath);
                }
                else if (envelope.MessageType == 2)
                {
                    HandleFileReceived(envelope.EnveloppeJson);
                }
                else if (envelope.MessageType == 3)
                {
                    FileRequest(envelope.EnveloppeJson);
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
            var deserializedCatalog = DeserializeMessage(catalogJson);
            var catalog = deserializedCatalog.Content.Content;
            var mediaList = new List<MediaData>();

            foreach (var media in catalog)
            {
                Console.WriteLine($"File name: {media.File_name}");
                Console.WriteLine($"File size: {media.File_size}");
                Console.WriteLine($"File artist: {media.File_artist}");
                Console.WriteLine($"File type: {media.File_type}");
                Console.WriteLine($"File duration: {media.File_duration}");
                Debug.WriteLine(media.File_name);
                //stocker les musiques dans une liste pour les afficher dans le listBox
                mediaList.Add(media);
            }
        }

        // Traiter le fichier reçu (par exemple, enregistrer le fichier)
        private void HandleFileReceived(string fileJson)
        {
            
        }

        // Traiter la demande de fichier spécifique
        private void FileRequest(string fileRequestJson)
        {
            
        }

        // Envoyer un fichier
        public async Task SendMusicCata(string selectedFolderPath)
        {
            // Charger les médias depuis le dossier spécifié
            var mediaList = LoadMedia(selectedFolderPath);

            // Construire une liste des médias sous forme d'objets anonymes
            var cataList = mediaList.Select(media => new
            {
                Title = media.File_name,
                Artist = media.File_artist,
                Type = media.File_type,
                Size = media.File_size,
                Duration = media.File_duration
            }).ToList();

            // Construire l'enveloppe contenant le catalogue
            var envelope = new
            {
                Content = cataList.ToArray()
            };

            // Sérialiser l'enveloppe en JSON
            string envelopeJson = JsonConvert.SerializeObject(envelope, Formatting.Indented);

            // Construire le message MQTT
            var message = new
            {
                MessageType = 0, // Type de message (catalogue)
                SenderId = Guid.NewGuid().ToString(),
                EnveloppeJson = envelopeJson
            };

            // Sérialiser le message en JSON
            string payload = JsonConvert.SerializeObject(message, Formatting.Indented);

            // Publier le message via MQTT
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false)
                .Build();

            await mqttClient.PublishAsync(mqttMessage);
            Console.WriteLine("Catalogue envoyé avec succès.");
        }

        // Obtenir la liste des fichiers du catalogue
        public List<MediaData> LoadMedia(string selectedFolderPath)
        {
            var mediaList = new List<MediaData>();
            Debug.WriteLine(selectedFolderPath);
            if (Directory.Exists(selectedFolderPath))
            {
                var files = Directory.GetFiles(selectedFolderPath);

                foreach (string file in files)
                {
                    FileInfo info = new FileInfo(file);

                    string title = Path.GetFileNameWithoutExtension(info.Name);
                    string artist = "Artiste Inconnu";
                    string duration = "Durée Inconnu";

                    try
                    {
                        var fileTag = TagLib.File.Create(file);
                        artist = fileTag.Tag.FirstPerformer ?? "Artiste inconnu";
                        duration = fileTag.Properties.Duration.ToString(@"hh\:mm\:ss");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading tags from file {file}: {ex.Message}");
                    }

                    MediaData media = new MediaData
                    {
                        File_name = title,
                        File_artist = artist,
                        File_type = info.Extension.TrimStart('.'),
                        File_size = info.Length,
                        File_duration = duration
                    };

                    mediaList.Add(media);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Le répertoire spécifié n'existe pas.", "Erreur", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

            return mediaList;
        }

        // Envoi d'un message générique
        public async Task SendMessageAsync(MessageType messageType, string enveloppeJson = null)
        {

            var envelope = new
            {
                MessageType = messageType,
                SenderId = Guid.NewGuid().ToString(),
                EnveloppeJson = enveloppeJson
            };

            // Sérialiser l'enveloppe en JSON et enleve enveloppeJson si null
            var payload = enveloppeJson == null ?
                JsonConvert.SerializeObject(new { envelope.MessageType, envelope.SenderId }) :
                JsonConvert.SerializeObject(envelope);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false)
                .Build();

            await mqttClient.PublishAsync(message);
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }

        // deserialaser un message
        public static Message DeserializeMessage(string message)
        {
            return JsonConvert.DeserializeObject<Message>(message);
        }
    }
}
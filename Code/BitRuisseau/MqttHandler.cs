using MQTTnet;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace BitRuisseau
{
    public class MqttHandler
    {
        private IMqttClient mqttClient;
        private MqttClientOptions options;
        private string topic;
        private string selectedFolderPath;
        public List<MediaData> mediaList = new List<MediaData>();
        private string clientId = Guid.NewGuid().ToString();

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
                var envelope = JsonConvert.DeserializeObject<Message>(receivedMessage);
                MessageBox.Show(envelope.MessageType.ToString());
                // G�rer les diff�rents types de messages re�us
                
                
                if(envelope.MessageType == 0)
                {
                    HandleCatalogReceived(envelope.EnveloppeJson);
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

        // Traiter le catalogue re�u (par exemple, afficher ou utiliser les fichiers dans le catalogue)
        private void HandleCatalogReceived(string catalogJson)
        {
            var catalog = DeserializeEnevloppeJson(catalogJson);
            
            mediaList = catalog.Content;
            //foreach (var media in catalog)
            //{
            //    Console.WriteLine($"File name: {media.File_name}");
            //    Console.WriteLine($"File size: {media.File_size}");
            //    Console.WriteLine($"File artist: {media.File_artist}");
            //    Console.WriteLine($"File type: {media.File_type}");
            //    Console.WriteLine($"File duration: {media.File_duration}");
            //    Debug.WriteLine(media.File_name);
            //    //stocker les musiques dans une liste pour les afficher dans le listBox
            //    mediaList.Add(media);
            //}
        }

        // Traiter le fichier re�u (par exemple, enregistrer le fichier)
        private void HandleFileReceived(string fileJson)
        {
            var envelope = JsonConvert.DeserializeObject<dynamic>(fileJson);

            string fileBytes = envelope.Content;

            byte[] file = Convert.FromBase64String(fileBytes);

            string fileName = envelope.Content[0].Title.ToString();

            string filePath = Path.Combine(selectedFolderPath, fileName);

            File.WriteAllBytes(filePath, file);

            MessageBox.Show($"Fichier {fileName} t�l�charg� avec succ�s dans le r�pertoire : {selectedFolderPath}", "T�l�chargement r�ussi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        

        // Traiter la demande de fichier sp�cifique
        private async void FileRequest(string fileRequestJson)
        {
            var deserializedMessage = DeserializeMessage(fileRequestJson);

            string fileName = deserializedMessage.Content.Content[0].Title;
            string filePath = Path.Combine(selectedFolderPath, fileName);
            byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
            string encodedBytes = Convert.ToBase64String(fileBytes);

            var envelope = new
            {
                Content = encodedBytes
            };
            string envelopeJson = System.Text.Json.JsonSerializer.Serialize(envelope, new JsonSerializerOptions { WriteIndented = true });

            var message = new
            {
                MessageType = 2,
                SenderId = clientId,
                EnveloppeJson = envelopeJson
            };

            string payload = System.Text.Json.JsonSerializer.Serialize(message, new JsonSerializerOptions { WriteIndented = true });
            
            var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag(false)
            .Build();

            await mqttClient.PublishAsync(mqttMessage);

            MessageBox.Show($"Fichier \"{fileName}\" envoy� avec succ�s.", "Envoi r�ussi", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        // Envoyer un fichier
        public async Task SendMusicCata(string selectedFolderPath)
        {
            // Charger les m�dias depuis le dossier sp�cifi�
            var mediaList = LoadMedia(selectedFolderPath);

            // Construire une liste des m�dias sous forme d'objets anonymes
            var cataList = mediaList.Select(media => new
            {
                Title = media.Title,
                Artist = media.Artist,
                Type = media.Type,
                Size = media.Size,
                Duration = media.Duration
            }).ToList();

            // Construire l'enveloppe contenant le catalogue
            var envelope = new
            {
                Content = cataList.ToArray()
            };

            // S�rialiser l'enveloppe en JSON
            string envelopeJson = JsonConvert.SerializeObject(envelope, Formatting.Indented);

            // Construire le message MQTT
            var message = new
            {
                MessageType = 0, // Type de message (catalogue)
                SenderId = Guid.NewGuid().ToString(),
                EnveloppeJson = envelopeJson
            };

            // S�rialiser le message en JSON
            string payload = JsonConvert.SerializeObject(message, Formatting.Indented);

            // Publier le message via MQTT
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false)
                .Build();

            await mqttClient.PublishAsync(mqttMessage);
            Console.WriteLine("Catalogue envoy� avec succ�s.");
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
                    string duration = "Dur�e Inconnu";

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
                        Title = title,
                        Artist = artist,
                        Type = info.Extension.TrimStart('.'),
                        Size = info.Length,
                        Duration = duration
                    };

                    mediaList.Add(media);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Le r�pertoire sp�cifi� n'existe pas.", "Erreur", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

            return mediaList;
        }

        // Envoi d'un message g�n�rique
        public async Task SendMessageAsync(MessageType messageType, string enveloppeJson = null)
        {

            var envelope = new
            {
                MessageType = messageType,
                SenderId = Guid.NewGuid().ToString(),
                EnveloppeJson = enveloppeJson
            };

            // S�rialiser l'enveloppe en JSON et enleve enveloppeJson si null
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

        public async Task SendFile(int type, string fileName)
        {
            try
            {
                // Cr�er le contenu du message
                var title = new
                {
                    Title = fileName
                };

                var titleList = new List<dynamic> { title };

                var envelope = new
                {
                    Content = titleList.ToArray()
                };

                var envelopetoJson = System.Text.Json.JsonSerializer.Serialize(envelope, new JsonSerializerOptions { WriteIndented = true });

                var templateMessage = new
                {
                    MessageType = type,  // Type 3 pour la demande de fichier
                    SenderId = options.ClientId, // Utilise le clientId d�fini lors de la connexion
                    EnveloppeJson = envelopetoJson
                };

                string payload = System.Text.Json.JsonSerializer.Serialize(templateMessage, new JsonSerializerOptions { WriteIndented = true });

                // Cr�er et envoyer le message MQTT
                var mqttMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .WithRetainFlag(false)
                    .Build();

                await mqttClient.PublishAsync(mqttMessage);
                Console.WriteLine("Message de demande de fichier envoy� avec succ�s.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'envoi de la demande de fichier : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // deserialaser un message
        public static Message DeserializeMessage(string message)
        {
            return JsonConvert.DeserializeObject<Message>(message);
        }
        public static Enveloppe DeserializeEnevloppeJson(string message)
        {
            return JsonConvert.DeserializeObject<Enveloppe>(message);
        }
    }
}
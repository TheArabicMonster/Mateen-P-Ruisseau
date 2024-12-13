using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace BitRuisseau
{
    public partial class Form1 : Form
    {
        private FileSystemWatcher fileWatcher;
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        private static IMqttClient mqttClient;
        public static string selectedFolderPath;
        string broker = "blue.section-inf.ch";
        int port = 1883;
        string topic = "test";
        string username = "ict";
        string password = "321";

        public Form1()
        {
            InitializeComponent();
            //SetupPanel();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(broker, port) // MQTT broker address and port
                .WithCredentials(username, password) // Set username and password
                .WithClientId(Guid.NewGuid().ToString()) // Unique client ID
                .WithCleanSession()
                .Build();
            try
            {
                // Connect to MQTT broker
                var connectResult = mqttClient.ConnectAsync(options).Result;

                if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
                {
                    Console.WriteLine("Connected to MQTT broker successfully.");

                    // Callback function when a message is received
                    mqttClient.ApplicationMessageReceivedAsync += async e =>
                    {
                        Console.WriteLine(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                        try
                        {
                            string message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                            Console.WriteLine($"Received message: {message}");

                            // Process the message as needed
                            if (message.Contains("HELLO"))
                            {
                                // Get the music list to send
                                string musicList = GetMusicList();

                                // Construct the response message
                                string response = $"{Guid.NewGuid()} (Mateen) possède les musiques suivantes :\n{musicList}";

                                // Ensure the client is connected
                                if (mqttClient == null || !mqttClient.IsConnected)
                                {
                                    Console.WriteLine("Client not connected. Reconnecting...");
                                    await mqttClient.ConnectAsync(options);
                                }

                                // Create and send the response message
                                var responseMessage = new MqttApplicationMessageBuilder()
                                    .WithTopic(topic)
                                    .WithPayload(response)
                                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                                    .WithRetainFlag(false)
                                    .Build();

                                await mqttClient.PublishAsync(responseMessage);
                                Console.WriteLine("Message sent successfully!");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing received message: {ex.Message}");
                        }
                    };

                    // Subscribe to a topic
                    var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                        .WithTopicFilter(f => f.WithTopic(topic).WithNoLocal(true))
                        .Build();
                    mqttClient.SubscribeAsync(subscribeOptions).Wait();
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

        private string GetMusicList()
        {
            if (!string.IsNullOrEmpty(selectedFolderPath) && Directory.Exists(selectedFolderPath))
            {
                string[] mp3Files = Directory.GetFiles(selectedFolderPath, "*.mp3");
                string[] mp4Files = Directory.GetFiles(selectedFolderPath, "*.mp4");

                var musicFiles = mp3Files.Concat(mp4Files).Select(Path.GetFileName).ToList();
                return JsonConvert.SerializeObject(musicFiles);
            }
            else
            {
                return "No folder selected or folder does not exist.";
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

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;

            Color colorLeft = Color.Blue;
            Color colorRight = Color.LightBlue;

            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle, colorLeft, colorRight, LinearGradientMode.Horizontal))
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }
        private void SetupPanel()
        {
            Panel panel = new Panel
            {
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(400, 200)
            };
            panel.Paint += panel3_Paint;
            this.Controls.Add(panel);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Image logo = Properties.Resources.bitruisseau_logo;
            int x = 250;
            int y = 10;
            int width = logo.Width;
            int height = logo.Height;
            Color colorLeft = Color.Blue;
            Color colorRight = Color.LightBlue;

            using (LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle, colorLeft, colorRight, LinearGradientMode.Horizontal))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
            e.Graphics.DrawImage(logo, x, y, width, height);

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Button clicked!");
            //folderBrowserDialog1.ShowDialog();
            // Crée et configure le FolderBrowserDialog

            folderBrowserDialog1.Description = "Veuillez sélectionner un dossier.";

            folderBrowserDialog1.ShowNewFolderButton = true; // Permet la création de nouveaux dossiers

            // Affiche le dialogue et vérifie si l'utilisateur a cliqué sur OK
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog1.SelectedPath; // Chemin du dossier sélectionné
                MessageBox.Show($"Dossier sélectionné : {selectedPath}", "Dossier choisi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                selectedFolderPath = selectedPath; // Met à jour le chemin du dossier sélectionné
                DisplayMedia(selectedPath);
                WatchFolder(selectedPath);
            }
            else
            {
                Debug.WriteLine("Folder selection was canceled.");
            }
        }

        private void DisplayMedia(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                listBox1.Items.Clear();

                string[] mp3Files = Directory.GetFiles(folderPath, "*.mp3");
                string[] mp4Files = Directory.GetFiles(folderPath, "*.mp4");

                Debug.WriteLine(Directory.GetFiles(folderPath));

                string[] mediaFiles = mp4Files.Concat(mp3Files).ToArray();

                Debug.Write(mediaFiles);

                foreach (string file in mediaFiles)
                {
                    listBox1.Items.Add(Path.GetFileName(file));
                }
            }
            else
            {
                MessageBox.Show("Le dossier spécifié n'existe pas.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WatchFolder(string folderPath)
        {
            if (fileWatcher != null)
            {
                fileWatcher.Dispose(); // Arrête la surveillance précédente si elle existait
            }

            fileWatcher = new FileSystemWatcher(folderPath)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                Filter = "*.*", // Surveille tous les fichiers
                EnableRaisingEvents = true // Active la surveillance
            };

            fileWatcher.Created += (s, e) => OnFolderChanged(folderPath);
            fileWatcher.Deleted += (s, e) => OnFolderChanged(folderPath);
            fileWatcher.Renamed += (s, e) => OnFolderChanged(folderPath);
        }

        private void OnFolderChanged(string folderPath)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => DisplayMedia(folderPath)));
            }
            else
            {
                DisplayMedia(folderPath);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("boutton config broker cliquer");

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }
    }
}

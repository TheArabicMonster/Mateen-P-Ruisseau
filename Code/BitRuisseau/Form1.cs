using MQTTnet;
using MQTTnet.Protocol;
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
        private MqttHandler mqttHandler;
        public static string selectedFolderPath = @"C:\Users\chats\Creative Cloud Files\asset\music fond";
        string broker = "mqtt.blue.section-inf.ch";
        int port = 1883;
        string topic = "test";
        string username = "ict";
        string password = "321";

        public Form1()
        {
            InitializeComponent();
            mqttHandler = new MqttHandler(broker, port, username, password, topic, selectedFolderPath);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await mqttHandler.ConnectAsync();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Button clicked!");
            folderBrowserDialog1.Description = "Veuillez sélectionner un dossier.";
            folderBrowserDialog1.ShowNewFolderButton = true;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog1.SelectedPath;
                MessageBox.Show($"Dossier sélectionné : {selectedPath}", "Dossier choisi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                selectedFolderPath = selectedPath;
                DisplayMedia(selectedPath);
                WatchFolder(selectedPath);
                await mqttHandler.SendMusicCata(selectedPath);
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
                fileWatcher.Dispose();
            }

            fileWatcher = new FileSystemWatcher(folderPath)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                Filter = "*.*",
                EnableRaisingEvents = true
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
            ConfigBroker configBroker = new ConfigBroker
            {
                Broker = broker,
                Port = port,
                Topic = topic,
                Username = username,
                Password = password
            };

            if (configBroker.ShowDialog() == DialogResult.OK)
            {
                broker = configBroker.Broker;
                port = configBroker.Port;
                topic = configBroker.Topic;
                username = configBroker.Username;
                password = configBroker.Password;

                mqttHandler = new MqttHandler(broker, port, username, password, topic, selectedFolderPath);
                mqttHandler.ConnectAsync();
            }
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await mqttHandler.SendMessageAsync(MessageType.DEMANDE_CATALOGUE);
        }
    }
}
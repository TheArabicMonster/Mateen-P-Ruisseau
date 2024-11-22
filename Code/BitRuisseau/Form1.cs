using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace BitRuisseau
{
    public partial class Form1 : Form
    {
        private FileSystemWatcher fileWatcher;
        public Form1()
        {
            InitializeComponent();
            //SetupPanel();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

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

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Crée et configure le FolderBrowserDialog
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Veuillez sélectionner un dossier.";
                folderDialog.ShowNewFolderButton = true; // Permet la création de nouveaux dossiers
                folderDialog.RootFolder = Environment.SpecialFolder.Desktop; // Dossier initial

                // Affiche le dialogue et vérifie si l'utilisateur a cliqué sur OK
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath; // Chemin du dossier sélectionné
                    MessageBox.Show($"Dossier sélectionné : {selectedPath}", "Dossier choisi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    DisplayMedia(selectedPath);
                    WatchFolder(selectedPath);
                }
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

                foreach (string file in mediaFiles) { listBox1.Items.Add(Path.GetFileName(file)); }
            }
            else { MessageBox.Show("Le dossier spécifié n'existe pas.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); }

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

            // Abonne-toi aux événements
            fileWatcher.Created += (s, e) => OnFolderChanged(folderPath);
            fileWatcher.Deleted += (s, e) => OnFolderChanged(folderPath);
            fileWatcher.Renamed += (s, e) => OnFolderChanged(folderPath);
        }
        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

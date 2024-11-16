using System.Drawing.Drawing2D;

namespace BitRuisseau
{
    public partial class Form1 : Form
    {
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
    }
}

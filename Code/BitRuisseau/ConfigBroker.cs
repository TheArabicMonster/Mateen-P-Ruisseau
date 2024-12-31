using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitRuisseau
{
    public partial class ConfigBroker : Form
    {
        public string Broker { get; set; }
        public int Port { get; set; }
        public string Topic { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public ConfigBroker()
        {
            InitializeComponent();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Assurez-vous de récupérer les valeurs des contrôles de la fenêtre
            Broker = textBoxBroker.Text;
            Port = int.Parse(textBoxPort.Text);
            Topic = textBoxTopic.Text;
            Username = textBoxUsername.Text;
            Password = textBoxPassword.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

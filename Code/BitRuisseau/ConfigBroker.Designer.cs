namespace BitRuisseau
{
    partial class ConfigBroker
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            textBoxBroker = new TextBox();
            textBoxPort = new TextBox();
            textBoxTopic = new TextBox();
            textBoxUsername = new TextBox();
            textBoxPassword = new TextBox();
            labelBroker = new Label();
            labelPort = new Label();
            labelTopic = new Label();
            labelUsername = new Label();
            labelPassword = new Label();
            buttonSave = new Button();
            buttonCancel = new Button();
            SuspendLayout();
            // 
            // textBoxBroker
            // 
            textBoxBroker.Location = new Point(120, 20);
            textBoxBroker.Name = "textBoxBroker";
            textBoxBroker.Size = new Size(200, 23);
            textBoxBroker.TabIndex = 0;
            // 
            // textBoxPort
            // 
            textBoxPort.Location = new Point(120, 60);
            textBoxPort.Name = "textBoxPort";
            textBoxPort.Size = new Size(200, 23);
            textBoxPort.TabIndex = 1;
            // 
            // textBoxTopic
            // 
            textBoxTopic.Location = new Point(120, 100);
            textBoxTopic.Name = "textBoxTopic";
            textBoxTopic.Size = new Size(200, 23);
            textBoxTopic.TabIndex = 2;
            // 
            // textBoxUsername
            // 
            textBoxUsername.Location = new Point(120, 140);
            textBoxUsername.Name = "textBoxUsername";
            textBoxUsername.Size = new Size(200, 23);
            textBoxUsername.TabIndex = 3;
            // 
            // textBoxPassword
            // 
            textBoxPassword.Location = new Point(120, 180);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(200, 23);
            textBoxPassword.TabIndex = 4;
            // 
            // labelBroker
            // 
            labelBroker.AutoSize = true;
            labelBroker.Location = new Point(20, 20);
            labelBroker.Name = "labelBroker";
            labelBroker.Size = new Size(41, 15);
            labelBroker.TabIndex = 5;
            labelBroker.Text = "Broker";
            // 
            // labelPort
            // 
            labelPort.AutoSize = true;
            labelPort.Location = new Point(20, 60);
            labelPort.Name = "labelPort";
            labelPort.Size = new Size(29, 15);
            labelPort.TabIndex = 6;
            labelPort.Text = "Port";
            // 
            // labelTopic
            // 
            labelTopic.AutoSize = true;
            labelTopic.Location = new Point(20, 100);
            labelTopic.Name = "labelTopic";
            labelTopic.Size = new Size(35, 15);
            labelTopic.TabIndex = 7;
            labelTopic.Text = "Topic";
            // 
            // labelUsername
            // 
            labelUsername.AutoSize = true;
            labelUsername.Location = new Point(20, 140);
            labelUsername.Name = "labelUsername";
            labelUsername.Size = new Size(60, 15);
            labelUsername.TabIndex = 8;
            labelUsername.Text = "Username";
            // 
            // labelPassword
            // 
            labelPassword.AutoSize = true;
            labelPassword.Location = new Point(20, 180);
            labelPassword.Name = "labelPassword";
            labelPassword.Size = new Size(57, 15);
            labelPassword.TabIndex = 9;
            labelPassword.Text = "Password";
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(120, 220);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(75, 23);
            buttonSave.TabIndex = 10;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(245, 220);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.TabIndex = 11;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // ConfigBroker
            // 
            ClientSize = new Size(350, 260);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
            Controls.Add(labelPassword);
            Controls.Add(labelUsername);
            Controls.Add(labelTopic);
            Controls.Add(labelPort);
            Controls.Add(labelBroker);
            Controls.Add(textBoxPassword);
            Controls.Add(textBoxUsername);
            Controls.Add(textBoxTopic);
            Controls.Add(textBoxPort);
            Controls.Add(textBoxBroker);
            Name = "ConfigBroker";
            Text = "ConfigBroker";
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.TextBox textBoxBroker;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.TextBox textBoxTopic;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label labelBroker;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.Label labelTopic;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
    }
}

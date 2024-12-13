namespace BitRuisseau
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            label1 = new Label();
            panel3 = new Panel();
            label2 = new Label();
            label4 = new Label();
            textBox1 = new TextBox();
            panel2 = new Panel();
            listBox1 = new ListBox();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            panel5 = new Panel();
            panel4 = new Panel();
            pictureBox1 = new PictureBox();
            button1 = new Button();
            label8 = new Label();
            label3 = new Label();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            pictureBox2 = new PictureBox();
            folderBrowserDialog1 = new FolderBrowserDialog();
            colorDialog1 = new ColorDialog();
            openFileDialog1 = new OpenFileDialog();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel5.SuspendLayout();
            panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(label1);
            panel1.Controls.Add(panel3);
            panel1.Location = new Point(0, -2);
            panel1.Name = "panel1";
            panel1.Size = new Size(158, 44);
            panel1.TabIndex = 1;
            panel1.Paint += panel1_Paint_1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(20, 11);
            label1.Name = "label1";
            label1.Size = new Size(118, 20);
            label1.TabIndex = 2;
            label1.Text = "Liste de fichiers";
            label1.Click += label1_Click;
            // 
            // panel3
            // 
            panel3.Location = new Point(-21, -17);
            panel3.Name = "panel3";
            panel3.Size = new Size(891, 504);
            panel3.TabIndex = 7;
            panel3.Paint += panel3_Paint;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold);
            label2.ForeColor = Color.Bisque;
            label2.Location = new Point(5, 47);
            label2.Name = "label2";
            label2.Size = new Size(123, 16);
            label2.TabIndex = 2;
            label2.Text = "Partage commun";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold);
            label4.ForeColor = Color.Bisque;
            label4.Location = new Point(46, 74);
            label4.Name = "label4";
            label4.Size = new Size(77, 16);
            label4.TabIndex = 4;
            label4.Text = "Personnel";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 12);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(202, 23);
            textBox1.TabIndex = 5;
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.ActiveCaption;
            panel2.Controls.Add(listBox1);
            panel2.Controls.Add(textBox1);
            panel2.Location = new Point(0, 127);
            panel2.Name = "panel2";
            panel2.Size = new Size(232, 398);
            panel2.TabIndex = 6;
            panel2.Paint += panel2_Paint;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(1, 43);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(226, 364);
            listBox1.TabIndex = 6;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // panel5
            // 
            panel5.Controls.Add(panel4);
            panel5.Controls.Add(label8);
            panel5.Controls.Add(label3);
            panel5.Location = new Point(494, 175);
            panel5.Name = "panel5";
            panel5.Size = new Size(310, 207);
            panel5.TabIndex = 7;
            // 
            // panel4
            // 
            panel4.Controls.Add(pictureBox1);
            panel4.Controls.Add(button1);
            panel4.Location = new Point(41, 121);
            panel4.Name = "panel4";
            panel4.Size = new Size(229, 80);
            panel4.TabIndex = 3;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.Folder;
            pictureBox1.Location = new Point(12, 13);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(60, 50);
            pictureBox1.TabIndex = 8;
            pictureBox1.TabStop = false;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 11F);
            button1.Location = new Point(78, 7);
            button1.Name = "button1";
            button1.Size = new Size(148, 66);
            button1.TabIndex = 2;
            button1.Text = "Localiser";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label8.Location = new Point(58, 48);
            label8.Name = "label8";
            label8.Size = new Size(199, 54);
            label8.TabIndex = 1;
            label8.Text = "Localiser votre dossier qui \r\ncontient vos fichier que vous \r\nvoulez mettre a disposition";
            label8.Click += label5_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(70, 11);
            label3.Name = "label3";
            label3.Size = new Size(168, 25);
            label3.TabIndex = 0;
            label3.Text = "Dossier de Médias";
            label3.Click += label3_Click;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.BackColor = Color.Transparent;
            radioButton1.Location = new Point(134, 46);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(14, 13);
            radioButton1.TabIndex = 8;
            radioButton1.TabStop = true;
            radioButton1.UseVisualStyleBackColor = false;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.BackColor = Color.Transparent;
            radioButton2.Location = new Point(134, 76);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(14, 13);
            radioButton2.TabIndex = 9;
            radioButton2.TabStop = true;
            radioButton2.UseVisualStyleBackColor = false;
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = Color.Transparent;
            pictureBox2.Image = Properties.Resources.broker_config;
            pictureBox2.Location = new Point(159, 12);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(68, 83);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 9;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            // 
            // folderBrowserDialog1
            // 
            folderBrowserDialog1.HelpRequest += folderBrowserDialog1_HelpRequest;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1081, 524);
            Controls.Add(pictureBox2);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            Controls.Add(panel5);
            Controls.Add(panel2);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(panel1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            Paint += Form1_Paint;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel panel1;
        private Label label1;
        private Label label2;
        private Label label4;
        private TextBox textBox1;
        private Panel panel2;
        private Panel panel3;
        private ListBox listBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Panel panel5;
        private Label label3;
        private Label label8;
        private Button button1;
        private Panel panel4;
        private PictureBox pictureBox1;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private PictureBox pictureBox2;
        private FolderBrowserDialog folderBrowserDialog1;
        private ColorDialog colorDialog1;
        private OpenFileDialog openFileDialog1;
    }
}

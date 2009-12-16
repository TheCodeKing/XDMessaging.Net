namespace TheCodeKing.Demo
{
    partial class Messenger
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.statusCheckBox = new System.Windows.Forms.CheckBox();
            this.msgCheckBox = new System.Windows.Forms.CheckBox();
            this.Mode = new System.Windows.Forms.GroupBox();
            this.ioStreamRadio = new System.Windows.Forms.RadioButton();
            this.wmRadio = new System.Windows.Forms.RadioButton();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.sendBtn = new System.Windows.Forms.Button();
            this.displayTextBox = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.Mode.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.Mode);
            this.panel1.Controls.Add(this.inputTextBox);
            this.panel1.Controls.Add(this.sendBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 222);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(289, 115);
            this.panel1.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.statusCheckBox);
            this.groupBox1.Controls.Add(this.msgCheckBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(130, 68);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Channels";
            // 
            // statusCheckBox
            // 
            this.statusCheckBox.Checked = true;
            this.statusCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.statusCheckBox.Location = new System.Drawing.Point(15, 40);
            this.statusCheckBox.Name = "statusCheckBox";
            this.statusCheckBox.Size = new System.Drawing.Size(61, 18);
            this.statusCheckBox.TabIndex = 12;
            this.statusCheckBox.Text = "Status";
            this.statusCheckBox.UseVisualStyleBackColor = true;
            this.statusCheckBox.Click += new System.EventHandler(this.statusCheckBox_CheckedChanged);
            // 
            // msgCheckBox
            // 
            this.msgCheckBox.Checked = true;
            this.msgCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.msgCheckBox.Location = new System.Drawing.Point(15, 19);
            this.msgCheckBox.Name = "msgCheckBox";
            this.msgCheckBox.Size = new System.Drawing.Size(94, 17);
            this.msgCheckBox.TabIndex = 11;
            this.msgCheckBox.Text = "UserMessage";
            this.msgCheckBox.UseVisualStyleBackColor = true;
            this.msgCheckBox.Click += new System.EventHandler(this.msgCheckBox_CheckedChanged);
            // 
            // Mode
            // 
            this.Mode.Controls.Add(this.ioStreamRadio);
            this.Mode.Controls.Add(this.wmRadio);
            this.Mode.Location = new System.Drawing.Point(148, 35);
            this.Mode.Name = "Mode";
            this.Mode.Size = new System.Drawing.Size(130, 68);
            this.Mode.TabIndex = 7;
            this.Mode.TabStop = false;
            this.Mode.Text = "Mode";
            // 
            // ioStreamRadio
            // 
            this.ioStreamRadio.AutoSize = true;
            this.ioStreamRadio.Location = new System.Drawing.Point(15, 41);
            this.ioStreamRadio.Name = "ioStreamRadio";
            this.ioStreamRadio.Size = new System.Drawing.Size(72, 17);
            this.ioStreamRadio.TabIndex = 8;
            this.ioStreamRadio.TabStop = true;
            this.ioStreamRadio.Text = "IO Stream";
            this.ioStreamRadio.UseVisualStyleBackColor = true;
            this.ioStreamRadio.CheckedChanged += new System.EventHandler(this.mode_CheckedChanged);
            // 
            // wmRadio
            // 
            this.wmRadio.AutoSize = true;
            this.wmRadio.Checked = true;
            this.wmRadio.Location = new System.Drawing.Point(15, 18);
            this.wmRadio.Name = "wmRadio";
            this.wmRadio.Size = new System.Drawing.Size(67, 17);
            this.wmRadio.TabIndex = 7;
            this.wmRadio.TabStop = true;
            this.wmRadio.Text = "Win Msg";
            this.wmRadio.UseVisualStyleBackColor = true;
            this.wmRadio.CheckedChanged += new System.EventHandler(this.mode_CheckedChanged);
            // 
            // inputTextBox
            // 
            this.inputTextBox.Location = new System.Drawing.Point(13, 6);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(178, 20);
            this.inputTextBox.TabIndex = 1;
            // 
            // sendBtn
            // 
            this.sendBtn.Location = new System.Drawing.Point(197, 6);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(80, 23);
            this.sendBtn.TabIndex = 2;
            this.sendBtn.Text = "Send";
            this.sendBtn.UseVisualStyleBackColor = true;
            this.sendBtn.Click += new System.EventHandler(this.sendBtn_Click);
            // 
            // displayTextBox
            // 
            this.displayTextBox.BackColor = System.Drawing.Color.White;
            this.displayTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayTextBox.Location = new System.Drawing.Point(0, 0);
            this.displayTextBox.Name = "displayTextBox";
            this.displayTextBox.ReadOnly = true;
            this.displayTextBox.Size = new System.Drawing.Size(289, 222);
            this.displayTextBox.TabIndex = 4;
            this.displayTextBox.TabStop = false;
            this.displayTextBox.Text = "";
            // 
            // Messenger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 337);
            this.Controls.Add(this.displayTextBox);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Messenger";
            this.Text = "XDMessaging Demo";
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.Mode.ResumeLayout(false);
            this.Mode.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Button sendBtn;
        private System.Windows.Forms.RichTextBox displayTextBox;
        private System.Windows.Forms.GroupBox Mode;
        private System.Windows.Forms.RadioButton ioStreamRadio;
        private System.Windows.Forms.RadioButton wmRadio;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox statusCheckBox;
        private System.Windows.Forms.CheckBox msgCheckBox;

    }
}


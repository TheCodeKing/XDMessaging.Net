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
            this.statusCheckBox = new System.Windows.Forms.CheckBox();
            this.msgCheckBox = new System.Windows.Forms.CheckBox();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.sendBtn = new System.Windows.Forms.Button();
            this.displayTextBox = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.statusCheckBox);
            this.panel1.Controls.Add(this.msgCheckBox);
            this.panel1.Controls.Add(this.inputTextBox);
            this.panel1.Controls.Add(this.sendBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 267);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(289, 56);
            this.panel1.TabIndex = 2;
            // 
            // statusCheckBox
            // 
            this.statusCheckBox.Checked = true;
            this.statusCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.statusCheckBox.Location = new System.Drawing.Point(113, 31);
            this.statusCheckBox.Name = "statusCheckBox";
            this.statusCheckBox.Size = new System.Drawing.Size(61, 18);
            this.statusCheckBox.TabIndex = 4;
            this.statusCheckBox.Text = "Status";
            this.statusCheckBox.UseVisualStyleBackColor = true;
            this.statusCheckBox.CheckedChanged += new System.EventHandler(this.statusCheckBox_CheckedChanged);
            // 
            // msgCheckBox
            // 
            this.msgCheckBox.Checked = true;
            this.msgCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.msgCheckBox.Location = new System.Drawing.Point(13, 32);
            this.msgCheckBox.Name = "msgCheckBox";
            this.msgCheckBox.Size = new System.Drawing.Size(94, 17);
            this.msgCheckBox.TabIndex = 3;
            this.msgCheckBox.Text = "UserMessage";
            this.msgCheckBox.UseVisualStyleBackColor = true;
            this.msgCheckBox.CheckedChanged += new System.EventHandler(this.msgCheckBox_CheckedChanged);
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
            this.sendBtn.Size = new System.Drawing.Size(75, 23);
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
            this.displayTextBox.Size = new System.Drawing.Size(289, 267);
            this.displayTextBox.TabIndex = 4;
            this.displayTextBox.TabStop = false;
            this.displayTextBox.Text = "";
            // 
            // Messenger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 323);
            this.Controls.Add(this.displayTextBox);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Messenger";
            this.Text = "XDMessaging Demo";
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Button sendBtn;
        private System.Windows.Forms.CheckBox msgCheckBox;
        private System.Windows.Forms.RichTextBox displayTextBox;
        private System.Windows.Forms.CheckBox statusCheckBox;

    }
}


namespace ChatApplication
{
    partial class ServerForm
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
            btnSendFile = new Button();
            txtFilePath = new TextBox();
            txtMessage = new TextBox();
            btnSendMessage = new Button();
            rTextBoxOldMessages = new RichTextBox();
            btnListen = new Button();
            txtIpAndPort = new TextBox();
            SuspendLayout();
            // 
            // btnSendFile
            // 
            btnSendFile.Location = new Point(464, 422);
            btnSendFile.Name = "btnSendFile";
            btnSendFile.Size = new Size(75, 22);
            btnSendFile.TabIndex = 0;
            btnSendFile.Text = "Send File";
            btnSendFile.UseVisualStyleBackColor = true;
            btnSendFile.Click += btnSendFile_Click;
            // 
            // txtFilePath
            // 
            txtFilePath.Location = new Point(464, 393);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.PlaceholderText = "File Path";
            txtFilePath.ReadOnly = true;
            txtFilePath.Size = new Size(277, 23);
            txtFilePath.TabIndex = 1;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(3, 422);
            txtMessage.Name = "txtMessage";
            txtMessage.PlaceholderText = "Enter Your Message";
            txtMessage.Size = new Size(277, 23);
            txtMessage.TabIndex = 3;
            // 
            // btnSendMessage
            // 
            btnSendMessage.Location = new Point(286, 422);
            btnSendMessage.Name = "btnSendMessage";
            btnSendMessage.Size = new Size(131, 23);
            btnSendMessage.TabIndex = 5;
            btnSendMessage.Text = "Send Message";
            btnSendMessage.UseVisualStyleBackColor = true;
            btnSendMessage.Click += btnSendMessage_Click;
            // 
            // rTextBoxOldMessages
            // 
            rTextBoxOldMessages.Location = new Point(3, 138);
            rTextBoxOldMessages.Name = "rTextBoxOldMessages";
            rTextBoxOldMessages.ReadOnly = true;
            rTextBoxOldMessages.Size = new Size(455, 278);
            rTextBoxOldMessages.TabIndex = 11;
            rTextBoxOldMessages.Text = "";
            // 
            // btnListen
            // 
            btnListen.Location = new Point(173, 12);
            btnListen.Name = "btnListen";
            btnListen.Size = new Size(75, 23);
            btnListen.TabIndex = 14;
            btnListen.Text = "Listen";
            btnListen.UseVisualStyleBackColor = true;
            btnListen.Click += btnListen_Click;
            // 
            // txtIpAndPort
            // 
            txtIpAndPort.Location = new Point(12, 12);
            txtIpAndPort.Name = "txtIpAndPort";
            txtIpAndPort.PlaceholderText = "IP,Port";
            txtIpAndPort.Size = new Size(155, 23);
            txtIpAndPort.TabIndex = 13;
            // 
            // ServerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnListen);
            Controls.Add(txtIpAndPort);
            Controls.Add(btnSendFile);
            Controls.Add(rTextBoxOldMessages);
            Controls.Add(btnSendMessage);
            Controls.Add(txtMessage);
            Controls.Add(txtFilePath);
            Name = "ServerForm";
            Text = "ServerForm";
            FormClosing += ServerForm_FormClosing;
            Load += ServerForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSendFile;
        private TextBox txtFilePath;
        private TextBox txtMessage;
        private Button btnSendMessage;
        private RichTextBox rTextBoxOldMessages;
        private Button btnListen;
        private TextBox txtIpAndPort;
    }
}

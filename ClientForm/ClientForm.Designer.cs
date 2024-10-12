namespace ClientForm
{
    partial class ClientForm
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
            btnSendMessage = new Button();
            txtMessage = new TextBox();
            txtFilePath = new TextBox();
            btnSelectTargetLoc = new Button();
            lblClientName = new Label();
            rTxtBoxOldMessages = new RichTextBox();
            txtIpAndPort = new TextBox();
            btnConnect = new Button();
            SuspendLayout();
            // 
            // btnSendMessage
            // 
            btnSendMessage.Location = new Point(250, 417);
            btnSendMessage.Name = "btnSendMessage";
            btnSendMessage.Size = new Size(97, 23);
            btnSendMessage.TabIndex = 1;
            btnSendMessage.Text = "Send Message";
            btnSendMessage.UseVisualStyleBackColor = true;
            btnSendMessage.Click += btnSendMessage_Click;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(3, 417);
            txtMessage.Name = "txtMessage";
            txtMessage.PlaceholderText = "Enter Your Message";
            txtMessage.Size = new Size(241, 23);
            txtMessage.TabIndex = 2;
            // 
            // txtFilePath
            // 
            txtFilePath.Location = new Point(464, 132);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.PlaceholderText = "Target File Path";
            txtFilePath.ReadOnly = true;
            txtFilePath.Size = new Size(241, 23);
            txtFilePath.TabIndex = 5;
            // 
            // btnSelectTargetLoc
            // 
            btnSelectTargetLoc.Location = new Point(464, 157);
            btnSelectTargetLoc.Name = "btnSelectTargetLoc";
            btnSelectTargetLoc.Size = new Size(141, 39);
            btnSelectTargetLoc.TabIndex = 4;
            btnSelectTargetLoc.Text = "Select Target File Path";
            btnSelectTargetLoc.UseVisualStyleBackColor = true;
            btnSelectTargetLoc.Click += btnSelectTargetLoc_Click;
            // 
            // lblClientName
            // 
            lblClientName.AutoSize = true;
            lblClientName.Location = new Point(654, 157);
            lblClientName.Name = "lblClientName";
            lblClientName.Size = new Size(0, 15);
            lblClientName.TabIndex = 9;
            // 
            // rTxtBoxOldMessages
            // 
            rTxtBoxOldMessages.Location = new Point(3, 132);
            rTxtBoxOldMessages.Name = "rTxtBoxOldMessages";
            rTxtBoxOldMessages.ReadOnly = true;
            rTxtBoxOldMessages.Size = new Size(455, 278);
            rTxtBoxOldMessages.TabIndex = 10;
            rTxtBoxOldMessages.Text = "";
            // 
            // txtIpAndPort
            // 
            txtIpAndPort.Location = new Point(12, 12);
            txtIpAndPort.Name = "txtIpAndPort";
            txtIpAndPort.PlaceholderText = "IP,Port";
            txtIpAndPort.Size = new Size(155, 23);
            txtIpAndPort.TabIndex = 11;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(173, 12);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(75, 23);
            btnConnect.TabIndex = 12;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // ClientForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnConnect);
            Controls.Add(txtIpAndPort);
            Controls.Add(rTxtBoxOldMessages);
            Controls.Add(lblClientName);
            Controls.Add(txtFilePath);
            Controls.Add(btnSelectTargetLoc);
            Controls.Add(txtMessage);
            Controls.Add(btnSendMessage);
            Name = "ClientForm";
            Text = "ClientForm";
            FormClosing += ClientForm_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnSendMessage;
        private TextBox txtMessage;
        private TextBox txtFilePath;
        private Button btnSelectTargetLoc;
        private Label lblClientName;
        private RichTextBox rTxtBoxOldMessages;
        private TextBox txtIpAndPort;
        private Button btnConnect;
    }
}

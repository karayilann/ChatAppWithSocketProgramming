using System;
using System.Net.Sockets;
using System.Text;
using BusinessLogic;
using ChatApplication.Domain;
using Message = ChatApplication.Domain.Message;

namespace ClientForm
{
    public partial class ClientForm : Form
    {
        private ClientService _clientService;
        private MessageService _messageService;
        private string filePath;

        public ClientForm()
        {
            InitializeComponent();
            btnSendMessage.Enabled = false;
            _clientService = new ClientService();
        }

        private void ReceiveMessages()
        {
            _messageService = new MessageService(_clientService.NetworkStream);

            try
            {
                while (true)
                {
                    Message receivedMessage = _messageService.ReceiveMessage();
                    Invoke((MethodInvoker)delegate
                    {
                        AddToTextBox(receivedMessage.ToString());
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error (Receive Message): " + ex.Message);
            }
        }

        private void ReceiveFile(string fileName, long fileSize)
        {
            try
            {
                byte[] buffer = new byte[1024];
                string fullFilePath = Path.Combine(filePath, fileName);

                using (FileStream fs = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
                {
                    long totalBytesReceived = 0;
                    while (totalBytesReceived < fileSize)
                    {
                        int bytesRead = _clientService.NetworkStream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            fs.Write(buffer, 0, bytesRead);
                            totalBytesReceived += bytesRead;
                        }
                    }

                    Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("Dosya ba�ar�yla al�nd� ve kaydedildi: " + fullFilePath);
                    });
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show("Hata (Dosya al�rken): " + ex.Message);
                });
            }
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtMessage.Text)) return;
            Message message = new Message(txtMessage.Text);
            _messageService.SendMessage(message);
            AddToTextBox("�stemci : " + message);
            txtMessage.Clear();
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _clientService.StopClient();
        }

        private void btnSelectTargetLoc_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browserDialog = new FolderBrowserDialog();
            if (browserDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = browserDialog.SelectedPath;
                filePath = browserDialog.SelectedPath;
            }

            if (!String.IsNullOrEmpty(txtFilePath.Text))
            {
                byte[] data = Encoding.ASCII.GetBytes("File location selected");
                _clientService.NetworkStream.Write(data, 0, data.Length);
            }

        }

        private void AddToTextBox(string message)
        {
            rTxtBoxOldMessages.Text += message + Environment.NewLine;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtIpAndPort.Text))
            {
                var strings = txtIpAndPort.Text.Split(',');
                string ip = strings[0];
                int port = int.Parse(strings[1]);

                Task.Run(() =>
                {
                    try
                    {
                        _clientService.StartClient(ip, port);

                        Invoke((MethodInvoker)delegate
                        {
                            if (_clientService.Client.Connected)
                            {
                                btnSendMessage.Enabled = true;
                                Task.Run(() => ReceiveMessages());
                                MessageBox.Show("Sunucuya ba�lan�ld�.");
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show("Ba�lant� hatas�: " + ex.Message);
                        });
                    }
                });
            }
        }
    }
}

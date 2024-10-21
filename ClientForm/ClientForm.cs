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
        private FileTransferService _fileTransfer;
        private string _fileSavePath;

        public ClientForm()
        {
            InitializeComponent();
            btnSendMessage.Enabled = false;
            _clientService = new ClientService();
            _fileTransfer = new FileTransferService();
        }

        private async Task ReceiveMessages()
        {
            _messageService = new MessageService(_clientService.NetworkStream);

            try
            {
                while (true)
                {
                    try
                    {
                        Message receivedMessage = _messageService.ReceiveMessage();

                        if (receivedMessage == null)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                MessageBox.Show("Connection terminated");
                                _clientService.StopClient();
                            });
                            break;
                        }

                        if (receivedMessage.Content.StartsWith("FILE:"))
                        {
                            string[] fileInfo = receivedMessage.Content.Split('|');
                            string fileName = fileInfo[0].Split(':')[1];
                            long fileSize = long.Parse(fileInfo[1].Split(':')[1]);

                            await ReceiveFileAsync(fileName, fileSize);
                            continue;
                        }

                        Invoke((MethodInvoker)delegate
                        {
                            AddToTextBox("Sunucu : " + receivedMessage);
                        });
                    }
                    catch (IOException)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show("Connection terminated");
                            _clientService.StopClient();
                        });
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error (Receive Message): " + ex.Message);
            }
        }

        // Alýndýðýna dair bilgi yok
        private async Task ReceiveFileAsync(string fileName, long fileSize)
        {
            string fullFilePath = Path.Combine(_fileSavePath, fileName);
            await _fileTransfer.ReceiveFileAsync(_clientService.NetworkStream, fileName, fullFilePath, fileSize);
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtMessage.Text)) return;
            Message message = new Message(txtMessage.Text);
            _messageService.SendMessage(message);
            AddToTextBox("Client : " + message);
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
                _fileSavePath = browserDialog.SelectedPath;
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
                                Task.Run(ReceiveMessages);
                                MessageBox.Show("Connected to server.");
                                txtIpAndPort.Clear();
                                btnConnect.Enabled = false;
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show("Connection Error: " + ex.Message);
                        });
                    }
                });
            }
        }
    }
}
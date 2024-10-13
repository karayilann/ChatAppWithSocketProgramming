using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BusinessLogic;
using ChatApplication.Domain;
using Message = ChatApplication.Domain.Message;

namespace ChatApplication
{
    public partial class ServerForm : Form
    {
        private ServerService _serverService;
        private List<MessageService> _messageServices;
        private List<NetworkStream> _clientStreams;
        private FileTransfer _fileTransfer;
        private readonly object _lockObj = new object();

        public ServerForm()
        {
            // Add dependency injection
            InitializeComponent();
            _serverService = new ServerService();
            _messageServices = new List<MessageService>();
            _clientStreams = new List<NetworkStream>();
            _fileTransfer = new FileTransfer(_clientStreams);
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMessage.Text)) return;
                Message message = new Message(txtMessage.Text);

                lock (_lockObj)
                {
                    foreach (var messageService in _messageServices)
                    {
                        messageService.SendMessage(message);
                    }
                }

                AddToOldMessages("Server: " + message);
                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error (Send Message): " + ex.Message);
            }
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _serverService.StopServer();
        }

        private void btnSendFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                _fileTransfer.SendFileToAll(fileName);
            }
        }

        private void AddToOldMessages(string message)
        {
            rTextBoxOldMessages.Text += message + Environment.NewLine;
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            string[] strings = txtIpAndPort.Text.Split(',');
            string ip = strings[0];
            int port = int.Parse(strings[1]);

            Thread serverThread = new Thread(() =>
            {
                _serverService.StartServer(ip, port);

                while (true)
                {
                    var client = _serverService.AcceptClient();
                    var networkStream = client.GetStream();

                    _clientStreams.Add(networkStream);

                    var messageService = new MessageService(networkStream);
                    _messageServices.Add(messageService);

                    Thread receiveThread = new Thread(() =>
                    {
                        while (true)
                        {
                            try
                            {
                                Message receivedMessage = messageService.ReceiveMessage();

                                if (receivedMessage == null)
                                {
                                    Invoke((MethodInvoker)delegate
                                    {
                                        MessageBox.Show("A client connection has been terminated.");
                                        _clientStreams.Remove(networkStream);
                                        _messageServices.Remove(messageService);
                                    });
                                    break;
                                }

                                Invoke((MethodInvoker)delegate
                                {
                                    AddToOldMessages("Client: " + receivedMessage);
                                });
                            }
                            catch (Exception ex)
                            {
                                Invoke((MethodInvoker)delegate
                                {
                                    MessageBox.Show("Error (Receive Message): " + ex.Message);
                                });
                                break;
                            }
                        }
                    });
                    receiveThread.IsBackground = true;
                    receiveThread.Start();
                }
            });

            serverThread.IsBackground = true;
            serverThread.Start();
            MessageBox.Show("Server started.");
        }

    }
}

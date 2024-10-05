using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;


namespace ChatApplication
{
    public partial class ServerForm : Form
    {
        private NetworkStream ns;
        private TcpClient client;
        public ServerForm()
        {
            InitializeComponent();
            btnSendFile.Enabled = false;
            btnSendMessage.Enabled = false;
        }

        private void ServerForm_Load(object sender, EventArgs e)
        {
            
        }

        private void StartServer(string ip,int port)
        {
            try
            {
                TcpListener server = new TcpListener(IPAddress.Parse(ip), port);
                server.Start();

                client = server.AcceptTcpClient();
                ns = client.GetStream();
                btnSendMessage.Enabled = true;

                Thread receiveThread = new Thread(ReceiveMessages);
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error (Start server): " + ex.Message);
            }
        }

        private void ReceiveMessages()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;
                StringBuilder completeMessage = new StringBuilder();

                while ((bytesRead = ns.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string receivedMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    completeMessage.Append(receivedMessage);
                    if (receivedMessage.StartsWith("File"))
                    {
                        btnSendFile.Enabled = true;
                    }
                    else
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            AddToOldMessages("Client : " + completeMessage);

                        });
                        completeMessage.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error (Receive Message): " + ex.Message);
            }
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtMessage.Text)) return;
                string message = txtMessage.Text;
                byte[] data = Encoding.ASCII.GetBytes(message);
                ns.Write(data, 0, data.Length);
                AddToOldMessages("Server : " + message);
                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error (Send Message): " + ex.Message);
            }
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ns?.Close();
            client?.Close();
        }

        private void btnSendFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string fileName = ofd.FileName;
                    FileInfo fileInfo = new FileInfo(fileName);
                    long fileSize = fileInfo.Length;

                    byte[] fileNameBytes = Encoding.ASCII.GetBytes("FILE:" + Path.GetFileName(fileName) + "|SIZE:" + fileSize.ToString() + "|");
                    ns.Write(fileNameBytes, 0, fileNameBytes.Length);
                    ns.Flush();

                    byte[] fileData = File.ReadAllBytes(fileName);
                    ns.Write(fileData, 0, fileData.Length);
                    ns.Flush();

                    MessageBox.Show("File send.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error (Send file): " + ex.Message);
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
            Thread serverThread = new Thread(() => StartServer(ip, port));
            serverThread.IsBackground = true;
            serverThread.Start();
            MessageBox.Show("Server started.");

        }
    }
}
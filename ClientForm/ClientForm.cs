using System.Net.Sockets;
using System.Text;

namespace ClientForm
{
    public partial class ClientForm : Form
    {
        private NetworkStream stream;
        private TcpClient client;
        private string filePath;

        public ClientForm()
        {
            InitializeComponent();
            btnSendMessage.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void ReceiveMessageThread()
        {
            Thread receiveThread = new Thread(ReceiveMessages);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        private void StartClient(string ip,int port)
        {
            client = new TcpClient(ip, port);
            stream = client.GetStream();
            MessageBox.Show("Sunucuya baðlanýldý");
            btnSendMessage.Enabled = true;
            ReceiveMessageThread();
        }

        private void ReceiveMessages()
        {
            try
            {
                byte[] data = new byte[1024];
                while (true)
                {
                    int recv = stream.Read(data, 0, data.Length);
                    if (recv == 0) break;

                    string message = Encoding.ASCII.GetString(data, 0, recv);

                    if (message.StartsWith("FILE:"))
                    {
                        string[] fileInfo = message.Split('|');
                        string fileName = fileInfo[0].Substring(5);
                        long fileSize = long.Parse(fileInfo[1].Substring(5));

                        ReceiveFile(fileName, fileSize);
                    }
                    else
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            AddToTextBox("Sunucu : " + message);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata (Mesaj alýrken): " + ex.Message);
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
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            fs.Write(buffer, 0, bytesRead);
                            totalBytesReceived += bytesRead;
                        }
                    }

                    Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("Dosya baþarýyla alýndý ve kaydedildi: " + fullFilePath);
                    });
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show("Hata (Dosya alýrken): " + ex.Message);
                });
            }
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtMessage.Text)) return;
            string message = txtMessage.Text;
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            AddToTextBox("Ýstemci : " + message);
            txtMessage.Clear();
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            stream?.Close();
            client?.Close();
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
                stream.Write(data, 0, data.Length);
            }

        }

        private void AddToTextBox(string message)
        {
            rTxtBoxOldMessages.Text += message + Environment.NewLine;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(txtIpAndPort.Text))
            {
                var strings = txtIpAndPort.Text.Split(',');
                string ip = strings[0];
                int port = int.Parse(strings[1]);
                StartClient(ip, port);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class ClientService
    {
        private NetworkStream stream;
        private TcpClient client;

        public NetworkStream NetworkStream
        {
            get => stream;
            set => stream = value;
        }

        public TcpClient Client
        {
            get => client;
            set => client = value;
        }


        public void StartClient(string ip, int port)
        {
            try
            {
               client = new TcpClient(ip, port);
               stream = client.GetStream();
            }
            catch (Exception e)
            {
                throw new Exception("Error in starting client: " + e.Message);
            }
        }

        public void StopClient()
        {
            stream?.Close();
            client?.Close();
        }

    }
}

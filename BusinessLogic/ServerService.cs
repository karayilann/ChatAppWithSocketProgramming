using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class ServerService
    {
        private TcpListener _server;
        private List<TcpClient> _clients;
        private List<NetworkStream> _clientStreams;

        public ServerService()
        {
            _clients = new List<TcpClient>();
            _clientStreams = new List<NetworkStream>();
        }

        public List<NetworkStream> ClientStreams => _clientStreams;

        public void StartServer(string ip, int port)
        {
            try
            {
                _server = new TcpListener(IPAddress.Parse(ip), port);
                _server.Start();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in starting server: " + ex.Message);
            }
        }

        public TcpClient AcceptClient()
        {
            try
            {
                var client = _server.AcceptTcpClient();
                _clients.Add(client);
                var ns = client.GetStream();
                _clientStreams.Add(ns);
                return client;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in accepting client: " + ex.Message);
            }
        }

        /// <summary>
        /// Anouncement to all clients (Currently not used)
        /// </summary>
        /// <param name="message"></param>
        public void BroadcastMessage(byte[] message)
        {
            foreach (var stream in _clientStreams)
            {
                if (stream.CanWrite)
                {
                    stream.Write(message, 0, message.Length);
                    stream.Flush();
                }
            }
        }

        public void StopServer()
        {
            foreach (var client in _clients)
            {
                client?.Close();
            }

            foreach (var stream in _clientStreams)
            {
                stream?.Close();
            }

            _server?.Stop();
        }
    }
}

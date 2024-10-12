using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApplication.Domain
{
    public class MessageService
    {
        private NetworkStream _networkStream;

        public MessageService(NetworkStream networkStream)
        {
            _networkStream = networkStream;
        }

        public void SendMessage(Message message)
        {
            string messageContent = message.ToString();
            byte[] data = Encoding.ASCII.GetBytes(messageContent);
            _networkStream.Write(data, 0, data.Length);

        }

        public Message ReceiveMessage()
        {
            byte [] data = new byte[1024];
            int receivedBytes = _networkStream.Read(data, 0, data.Length);
            string messageContent = Encoding.ASCII.GetString(data, 0, receivedBytes);
            return new Message(messageContent);
        }

    }
}

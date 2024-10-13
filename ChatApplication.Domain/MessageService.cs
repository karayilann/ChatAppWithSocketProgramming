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

        //Fix the issue where messages sent from the client only appear on the server
        public void SendMessage(Message message)
        {
            string messageContent = message.ToString();
            byte[] data = Encoding.ASCII.GetBytes(messageContent);
            _networkStream.Write(data, 0, data.Length);
        }

        public Message ReceiveMessage()
        {
            byte[] data = new byte[1024];
            try
            {
                int receivedBytes = _networkStream.Read(data, 0, data.Length);
                if (receivedBytes == 0)
                {
                    return null;
                }

                string messageContent = Encoding.ASCII.GetString(data, 0, receivedBytes);


                // Add to check file
                //if (messageContent.StartsWith("FILE:"))
                //{
                //    return new Message(fileName, fileSize);
                //}
                return new Message(messageContent);
            }
            catch (Exception ex)
            {
                throw new Exception("Hata (ReceiveMessage): " + ex.Message);
            }
        }


    }
}

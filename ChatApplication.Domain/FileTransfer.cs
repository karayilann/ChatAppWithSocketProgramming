using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ChatApplication.Domain
{
    public class FileTransfer
    {
        private readonly List<NetworkStream> _clientStreams;

        public FileTransfer(List<NetworkStream> clientStreams)
        {
            _clientStreams = clientStreams;
        }

        public void SendFileToAll(string fileName)
        {
            try
            {
                var fileInfo = new FileInfo(fileName);
                var fileSize = fileInfo.Length;
                var fileNameBytes = Encoding.ASCII.GetBytes($"FILE:{Path.GetFileName(fileName)}|SIZE:{fileSize}|");

                foreach (var stream in _clientStreams)
                {
                    if (!stream.CanWrite)
                        continue;

                    stream.Write(fileNameBytes, 0, fileNameBytes.Length);
                    stream.Flush();

                    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read)) //Chuncked transfer 
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead;

                        while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            stream.Write(buffer, 0, bytesRead);
                            stream.Flush();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error (Send File): " + ex.Message);
            }
        }
    }

    // Add receive file method here

}
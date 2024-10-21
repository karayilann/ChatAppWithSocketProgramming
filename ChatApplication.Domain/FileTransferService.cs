using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApplication.Domain
{
    public class FileTransferService
    {
        public async Task SendFileToAllAsync(List<NetworkStream> clientStreams, string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            var fileSize = fileInfo.Length;
            var fileNameBytes = Encoding.ASCII.GetBytes($"FILE:{Path.GetFileName(fileName)}|SIZE:{fileSize}|");

            foreach (var stream in clientStreams)
            {
                if (!stream.CanWrite)
                    continue;

                try
                {
                    await stream.WriteAsync(fileNameBytes, 0, fileNameBytes.Length);
                    await stream.FlushAsync();

                    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead;
                        while ((bytesRead = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await stream.WriteAsync(buffer, 0, bytesRead);
                            await stream.FlushAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error (Send File to stream): {ex.Message}");
                }
            }
        }

        public async Task SendFileToClientAsync(NetworkStream clientStream, string fileName)
        {
            if (!clientStream.CanWrite)
                throw new InvalidOperationException("Client stream is not writable.");

            var fileInfo = new FileInfo(fileName);
            var fileSize = fileInfo.Length;
            var fileNameBytes = Encoding.ASCII.GetBytes($"FILE:{Path.GetFileName(fileName)}|SIZE:{fileSize}|");

            try
            {
                await clientStream.WriteAsync(fileNameBytes, 0, fileNameBytes.Length);
                await clientStream.FlushAsync();

                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await clientStream.WriteAsync(buffer, 0, bytesRead);
                        await clientStream.FlushAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error (Send File to client): {ex.Message}");
            }
        }

        public async Task ReceiveFileAsync(NetworkStream networkStream, string fileName, string fullFilePath, long fileSize)
        {
            if (!networkStream.CanRead) throw new InvalidOperationException("The network stream is not readable.");

            byte[] buffer = new byte[1024];

            try
            {
                using (FileStream fs = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
                {
                    long totalBytesReceived = 0;

                    while (totalBytesReceived < fileSize)
                    {
                        int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            await fs.WriteAsync(buffer, 0, bytesRead);
                            totalBytesReceived += bytesRead;
                        }
                    }
                }

                Console.WriteLine("File successfully received and saved at: " + fullFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error (Receiving File): {ex.Message}");
            }
        }

        public bool ValidateFileSize(string fileName, long expectedSize)
        {
            var fileInfo = new FileInfo(fileName);
            return fileInfo.Length == expectedSize;
        }
    }
}
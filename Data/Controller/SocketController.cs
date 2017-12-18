using Common.Tools;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Data.Controller
{
    public delegate void SocketFinishedEventHandler(string response, bool success);

    public class SocketController
    {
        private const string TAG = "SocketController";

        public SocketController()
        {
            // Empty constructor, nothing needed here
        }

        public event SocketFinishedEventHandler OnSocketFinished;
        private void publishOnSocketFinished(string response, bool success)
        {
            OnSocketFinished?.Invoke(response, success);
        }

        public async void SendCommandToWebsite(string url, int port, string command)
        {
            try
            {
                await sendCommandToWebsiteAsync(url, port, command);
            }
            catch (Exception exception)
            {
                Logger.Instance.Error(TAG, exception.Message);
                publishOnSocketFinished(exception.Message, false);
            }
        }

        private async Task sendCommandToWebsiteAsync(string url, int port, string command)
        {
            if (url == null)
            {
                Logger.Instance.Error(TAG, "URL may not be null!");
                publishOnSocketFinished("ERROR: URL may not be null!", false);
                return;
            }

            if (port == -1)
            {
                Logger.Instance.Error(TAG, "Invalid port!");
                publishOnSocketFinished("ERROR: Invalid port!", false);
                return;
            }

            if (command == null)
            {
                Logger.Instance.Error(TAG, "Command may not be null!");
                publishOnSocketFinished("ERROR: Command may not be null!", false);
                return;
            }

            string data = null;

            try
            {
                TcpClient clientSocket = new TcpClient();
                clientSocket.Connect(url, port);

                NetworkStream serverStream = clientSocket.GetStream();
                byte[] outStream = Encoding.ASCII.GetBytes(command + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                byte[] inStream = new byte[8192];
                serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
                data = Encoding.ASCII.GetString(inStream);
            }
            catch (Exception exception)
            {
                Logger.Instance.Error(TAG, exception.Message);
            }

            publishOnSocketFinished(data, (data != null));
        }

        public void Dispose()
        {
            Logger.Instance.Debug(TAG, "Dispose");
        }
    }
}

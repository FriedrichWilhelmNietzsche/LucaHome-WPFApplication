using Common.Tools;
using System.Net.Sockets;
using System.Text;

/* Reference Help
 * http://csharp.net-informations.com/communications/csharp-client-socket.htm
 */

namespace Data.Services
{
    public class SocketService
    {
        private const string TAG = "SocketService";
        private Logger _logger;

        private const int RECEIVE_BUFFER_SIZE = 16384;

        private TcpClient _tcpClient = new TcpClient();

        public SocketService()
        {
            _logger = new Logger(TAG);
        }

        public void ConnectToServer(string ip, int port)
        {
            _logger.Debug(string.Format("Connecting to server with ip {0} at port {1}", ip, port));
            _tcpClient.ConnectAsync(ip, port);
        }

        public string SendCommand(string command)
        {
            _logger.Debug(string.Format("Sending command {0} to server", command));

            if (!_tcpClient.Connected)
            {
                _logger.Error("TcpClient is not connected!");
                return "ERROR: Not connected!";
            }

            if (command.Length < 10)
            {
                _logger.Error("Invalid command length!");
                return "ERROR: Invalid command length!";
            }

            NetworkStream serverStream = _tcpClient.GetStream();
            byte[] outStream = Encoding.ASCII.GetBytes(command + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            byte[] inputStream = new byte[RECEIVE_BUFFER_SIZE];
            serverStream.Read(inputStream, 0, (int)_tcpClient.ReceiveBufferSize);
            string returnData = Encoding.ASCII.GetString(inputStream);

            _logger.Debug(string.Format("Received data {0} from server!", returnData));

            return returnData;
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");
            _tcpClient.Dispose();
        }
    }
}

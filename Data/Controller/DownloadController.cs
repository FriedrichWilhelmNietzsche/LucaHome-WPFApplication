using Common.Tools;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Data.Controller
{
    public enum DownloadType
    {
        Birthday, BirthdayAdd,
        Movie, MovieAdd,
        ShoppingList, ShoppingListAdd,
        Temperature,
        User,
        WirelessSocket, WirelessSocketAdd
    };

    public delegate void DownloadFinishedEventHandler(string response, bool success, DownloadType downloadType);

    public class DownloadController
    {
        private const string TAG = "DownloadController";
        private readonly Logger _logger;

        private const int RECEIVE_BUFFER_SIZE = 16777216;

        private readonly TcpClient _tcpClient = new TcpClient();

        public DownloadController()
        {
            _logger = new Logger(TAG);
        }

        public event DownloadFinishedEventHandler OnDownloadFinished;

        public string SendCommandToConnectedServer(string ip, int port, string command)
        {
            _logger.Debug("SendCommandToConnectedServer");

            if (ip == null)
            {
                _logger.Error("IP may not be null!");
                return "ERROR: IP may not be null!";
            }

            if (ip.Length < 13)
            {
                _logger.Error("Invalid ip length!");
                return "ERROR: Invalid ip length!";
            }

            if (port == -1)
            {
                _logger.Error("Invalid value for port!");
                return "ERROR: Invalid value for port!";
            }

            _logger.Debug(string.Format("Connecting to server with ip {0} at port {1}", ip, port));
            _tcpClient.Connect(ip, port);

            if (!_tcpClient.Connected)
            {
                _logger.Error("TcpClient is not connected!");
                return "ERROR: Not connected!";
            }

            if (command == null)
            {
                _logger.Error("Command may not be null!");
                return "ERROR: Command may not be null!";
            }

            if (command.Length < 10)
            {
                _logger.Error("Invalid command length!");
                return "ERROR: Invalid command length!";
            }

            _logger.Debug(string.Format("Sending command {0} to server", command));

            NetworkStream serverStream = _tcpClient.GetStream();
            byte[] outStream = Encoding.ASCII.GetBytes(command + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            byte[] inputStream = new byte[RECEIVE_BUFFER_SIZE];
            serverStream.Read(inputStream, 0, (int)_tcpClient.ReceiveBufferSize);
            _logger.Debug(string.Format("Received inputStream {0} from server!", inputStream));

            string returnData = Encoding.ASCII.GetString(inputStream);
            _logger.Debug(string.Format("Received data {0} from server!", returnData));

            serverStream.Close();

            if (_tcpClient.Connected)
            {
                _tcpClient.Close();
            }

            return returnData;
        }

        public async Task SendCommandToWebsiteAsync(string requestUrl, DownloadType downloadType)
        {
            _logger.Debug("SendCommandToConnectedServer");

            if (requestUrl == null)
            {
                _logger.Error("RequestUrl may not be null!");
                OnDownloadFinished("ERROR: RequestUrl may not be null!", false, downloadType);
                return;
            }

            if (requestUrl.Length < 15)
            {
                _logger.Error("Invalid requestUrl length!");
                OnDownloadFinished("ERROR: Invalid requestUrl length!", false, downloadType);
                return;
            }

            HttpClient httpClient = new HttpClient();
            string data = await httpClient.GetStringAsync(requestUrl);

            OnDownloadFinished(data, (data != null), downloadType);
        }

        public void Dispose()
        {
            _logger.Debug("Dispose");
            if (_tcpClient.Connected)
            {
                _tcpClient.Close();
            }
        }
    }
}

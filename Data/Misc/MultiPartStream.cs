using Common.Tools;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Data.Misc
{
    public class MultiPartStream
    {
        private const string TAG = "MultiPartStream";
        private readonly Logger _logger;

        private byte[] _seperatorBytes = Encoding.UTF8.GetBytes("\r\n\r\n");
        private byte[] _headerbytes = new byte[100];

        private Regex _contentRegex = new Regex("Content-Length:       (?<length>[0-9]+)\r\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private BinaryReader _binaryReader;

        public MultiPartStream(Stream stream)
        {
            _logger = new Logger(TAG);
            _binaryReader = new BinaryReader(new BufferedStream(stream));
        }

        public Task<byte[]> NextPartAsync()
        {
            return Task.Run(() =>
            {
                // every part has it's own headers
                string headerSection = readContentHeaderSection(_binaryReader);

                // let's parse the header section for the content-length
                int length = getPartLength(headerSection);

                // now let's get the image
                return _binaryReader.ReadBytes(length);
            });
        }

        public void Close()
        {
            _binaryReader.Dispose();
        }

        private string readContentHeaderSection(BinaryReader binaryReader)
        {
            try
            {
                // headers and content in multi part are seperated by two \r\n
                bool found = false;

                int count = 4;
                binaryReader.Read(_headerbytes, 0, 4);

                for (int index = 0; index < _headerbytes.Length; index++)
                {
                    found = seperatorBytesExistsInArray(index, _headerbytes);
                    if (!found)
                    {
                        _headerbytes[count] = binaryReader.ReadByte();
                        count++;
                    }
                    else
                    {
                        break;
                    }

                }

                return Encoding.UTF8.GetString(_headerbytes, 0, count);
            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message);
                return string.Empty;
            }
        }

        private int getPartLength(string headerSection)
        {
            try
            {
                Match match = _contentRegex.Match(headerSection);
                return int.Parse(match.Groups["length"].Value);
            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message);
                return 0;
            }
        }

        private bool seperatorBytesExistsInArray(int position, byte[] array)
        {
            bool result = false;
            for (int index = position, compareIndex = 0; compareIndex < _seperatorBytes.Length; index++, compareIndex++)
            {
                result = array[index] == _seperatorBytes[compareIndex];
                if (!result)
                {
                    break;
                }
            }
            return result;
        }
    }
}

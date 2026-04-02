using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace project_sharp
{
    public class TelemetryData
    {
        public string Position;
        public string Speed;
        public string Accel;
        public string Override;
        public string SCurve;
        public string SlopeMinus;
        public string SlopePlus;
    }

    public class AxisController
    {
        private const int Port = 10001;
        private const int BufferSize = 1024;

        private TcpClient _client;
        private CancellationTokenSource _cts;

        public string Name { get; }
        public bool IsConnected { get { return _client != null && _client.Connected; } }
        public CancellationToken TelemetryToken { get { return _cts != null ? _cts.Token : CancellationToken.None; } }

        public AxisController(string name)
        {
            Name = name;
        }

        public void Connect(string ip)
        {
            _cts = new CancellationTokenSource();
            _client = new TcpClient();
            _client.Connect(IPAddress.Parse(ip), Port);
        }

        // IP가 일치할 때만 연결 해제. 성공 여부 반환.
        public bool Disconnect(string selectedIp)
        {
            if (_client == null) return false;
            string connectedIp = ((IPEndPoint)_client.Client.RemoteEndPoint).Address.ToString();
            if (connectedIp != selectedIp) return false;

            _cts?.Cancel();
            _client.Close();
            _client = null;
            return true;
        }

        // 명령 전송 후 응답 반환. 수신 실패 시 null 반환.
        public async Task<string> SendCommandAsync(string command)
        {
            NetworkStream stream = _client.GetStream();
            byte[] data = Encoding.ASCII.GetBytes(command + "\n");
            await stream.WriteAsync(data, 0, data.Length);

            byte[] buffer = new byte[BufferSize];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            return bytesRead > 0 ? Encoding.ASCII.GetString(buffer, 0, bytesRead) : null;
        }

        // 텔레메트리 폴링: tp, sp?, ac?, ovrd?, scrv?, slpn?, slpp? 순서로 조회
        public async Task<TelemetryData> PollTelemetryAsync()
        {
            NetworkStream stream = _client.GetStream();

            byte[] tpData = Encoding.ASCII.GetBytes("tp\n");
            await stream.WriteAsync(tpData, 0, tpData.Length);

            byte[] tpBuffer = new byte[BufferSize];
            int tpRead = await stream.ReadAsync(tpBuffer, 0, tpBuffer.Length);
            if (tpRead <= 0) return null;

            string tpResponse = Encoding.ASCII.GetString(tpBuffer, 0, tpRead);

            return new TelemetryData
            {
                Position   = tpResponse.Length > 4 ? tpResponse.Substring(4) : tpResponse,
                Speed      = await QueryAsync(stream, "sp?\n"),
                Accel      = await QueryAsync(stream, "ac?\n"),
                Override   = await QueryAsync(stream, "ovrd?\n"),
                SCurve     = await QueryAsync(stream, "scrv?\n"),
                SlopeMinus = await QueryAsync(stream, "slpn?\n"),
                SlopePlus  = await QueryAsync(stream, "slpp?\n"),
            };
        }

        // 쿼리 명령 전송 후 에코된 명령 부분을 제거하고 값만 반환
        private async Task<string> QueryAsync(NetworkStream stream, string command)
        {
            byte[] data = Encoding.ASCII.GetBytes(command);
            await stream.WriteAsync(data, 0, data.Length);

            byte[] buffer = new byte[BufferSize];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead <= 0) return string.Empty;

            string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            int offset = Math.Min(command.Length + 1, response.Length);
            return response.Substring(offset);
        }
    }
}

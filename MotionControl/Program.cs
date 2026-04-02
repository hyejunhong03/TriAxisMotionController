using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MotionControl
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 기존 코드에서 스레드 풀을 사용하여 소켓 연결 시도
            // 여러 대상 IP 주소에 대해 소켓 연결을 시도하는 로직을 작성합니다.
            string[] targetIPs = { "192.168.2.100", "192.168.2.101", "192.168.2.102" };
            int port = 10001;

            foreach (string ip in targetIPs)
            {
                // 각 IP 주소에 대해 별도의 스레드로 ConnectToServer 메서드를 실행합니다.
                ThreadPool.QueueUserWorkItem(ConnectToServer, new Tuple<string, int>(ip, port));
            }

            // 폼 시작
            Application.Run(new Form1());
        }

        // 각 IP 주소에 대한 소켓 연결 시도를 담당하는 메서드입니다.
        static void ConnectToServer(object state)
        {
            // 기존 코드: 소켓 연결만 시도
            // 각 IP 주소와 포트 정보 추출
            var ipAndPort = (Tuple<string, int>)state;
            string ipAddress = ipAndPort.Item1;
            int port = ipAndPort.Item2;

            try
            {
                // TcpClient를 사용하여 해당 IP 주소와 포트로 소켓 연결 시도
                using (var client = new TcpClient())
                {
                    client.Connect(IPAddress.Parse(ipAddress), port);
                    Console.WriteLine($"Connected to {ipAddress}:{port}");
                }
            }
            catch (Exception ex)
            {
                // 소켓 연결 실패 시 오류 메시지 출력
                Console.WriteLine($"Failed to connect to {ipAddress}:{port}. Error: {ex.Message}");
            }
        }
    }
}
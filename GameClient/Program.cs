using System;
using System.Net.Sockets;
using System.Text;

class SimpleClient
{
    // 서버 IP와 포트 설정
    private const string ServerIP = "127.0.0.1";
    private const int ServerPort = 5000;

    public static void Main(string[] args)
    {
        Console.WriteLine("🎮 솔리테어 클라이언트 시뮬레이터 시작");

        // 시나리오: 유저가 12345 시드 맵을 플레이해서 80번 만에 45.5초로 클리어함
        string myId = "SolitaireMaster";
        string currentSeed = "12345";
        int myMoves = 80;
        double myTime = 45.52;

        Console.WriteLine($"\n[게임 종료] 시드:{currentSeed}, 이동:{myMoves}, 시간:{myTime}s");
        Console.Write("서버로 기록을 전송하시겠습니까? (y/n): ");
        
        if (Console.ReadLine()?.ToLower() == "y")
        {
            SendGameResult(myId, currentSeed, myMoves, myTime);
        }

        Console.WriteLine("\n아무 키나 누르면 종료합니다.");
        //Console.ReadKey();
        Console.ReadLine();
    }

    public static void SendGameResult(string userId, string seed, int moves, double time)
    {
        try
        {
            // 1. 서버 접속
            using TcpClient client = new TcpClient(ServerIP, ServerPort);
            using NetworkStream stream = client.GetStream();

            // 2. 데이터 패킷 구성 (형식: ID|Seed|Moves|Time)
            string data = $"{userId}|{seed}|{moves}|{time:F2}";
            byte[] buffer = Encoding.UTF8.GetBytes(data);

            // 3. 데이터 전송
            stream.Write(buffer, 0, buffer.Length);
            Console.WriteLine($"📤 전송 중... ({data})");

            // 4. 서버 응답 확인 (서버가 DB 저장 후 "OK"를 보내는지 확인)
            byte[] respBuffer = new byte[1024];
            int bytesRead = stream.Read(respBuffer, 0, respBuffer.Length);
            string response = Encoding.UTF8.GetString(respBuffer, 0, bytesRead);
            
            Console.WriteLine($"📨 서버 응답: {response}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"❌ 서버 연결 실패: {e.Message}");
        }
    }
}
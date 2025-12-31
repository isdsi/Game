using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Data.Sqlite; // SQLite 라이브러리 추가
using Microsoft.Extensions.Logging;

public class GameServer
{
    // 시스템 로그
    private ILogger _logger;

    // 서버 IP와 포트 설정
    private int _serverPort;

    private IGameRepository _gameRepository;

    // 테스트를 위해 리포지토리와 로거를 외부에서 주입받도록 수정
    public GameServer(IGameRepository gameRepository, ILogger logger, int serverPort = 5000)
    {
        _gameRepository = gameRepository;
        _logger = logger;
        _serverPort = serverPort;
        
        // 초기화는 외부나 내부 어디서든 의도에 따라 호출
        _gameRepository.Initialize();
    }
    
    public void Process()
    {
        // IPAddress.Any 는 0.0.0.0 으로 네트워크로 들어오는 모든 ip를 받는다라는 의미이다.
        TcpListener server = new TcpListener(IPAddress.Any, _serverPort);
        server.Start();
        _logger.LogInformation("🚀 DB 연동 서버 가동 중 (Port: 5000)...");

        while (true)
        {
            using TcpClient client = server.AcceptTcpClient();
            using NetworkStream stream = client.GetStream();
            
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string rawData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            HandleClientData(rawData);
            
            byte[] response = Encoding.UTF8.GetBytes("OK");
            stream.Write(response, 0, response.Length);
        }
    }

    public void HandleClientData(string rawData)
    {
        try 
            {
                // 데이터 파싱: "User01|12345|80|45.5" (ID|Seed|Moves|Time)
                string[] parts = rawData.Split('|');
                if (parts.Length == 4)
                {
                    string userId = parts[0];
                    string seed = parts[1];
                    int moves = int.Parse(parts[2]);
                    double playTime = double.Parse(parts[3]);

                    // 2. DB에 데이터 저장
                    _gameRepository.SaveGameResult(userId, seed, moves, playTime);

                    _logger.LogInformation($"[DB 저장 완료] 유저:{userId}, 시드:{seed}, 이동:{moves}, 시간:{playTime}s");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ 처리 오류: {ex.Message}");
            }

    }
}
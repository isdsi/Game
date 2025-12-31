using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Data.Sqlite; // SQLite 라이브러리 추가
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Npgsql;

class SimpleServer
{
    private static string ConnectionString = "Host=localhost;Username=admin;Password=password123;Database=game_server_db";

    // 시스템 로그 생성기
    private static ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

    // 시스템 로그
    private static ILogger logger = loggerFactory.CreateLogger("GameServerDocker");

    // 서버 IP와 포트 설정
    private const int ServerPort = 5000;


    static void Main()
    {
        // 1. 서버 시작 전 DB 및 테이블 초기화
        InitializeDatabase();

        // IPAddress.Any 는 0.0.0.0 으로 네트워크로 들어오는 모든 ip를 받는다라는 의미이다.
        TcpListener server = new TcpListener(IPAddress.Any, ServerPort);
        server.Start();
        logger.LogInformation("🚀 DB 연동 서버 가동 중 (Port: 5000)...");

        while (true)
        {
            using TcpClient client = server.AcceptTcpClient();
            using NetworkStream stream = client.GetStream();
            
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string rawData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

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
                    SaveResultToDb(userId, seed, moves, playTime);

                    logger.LogInformation($"[DB 저장 완료] 유저:{userId}, 시드:{seed}, 이동:{moves}, 시간:{playTime}s");
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation($"❌ 처리 오류: {ex.Message}");
            }

            byte[] response = Encoding.UTF8.GetBytes("OK");
            stream.Write(response, 0, response.Length);
        }
    }

    // DB 테이블이 없으면 생성하는 함수
    static void InitializeDatabase()
    {
        // 2. 데이터베이스 연결 및 테이블 생성 예시
        using (var conn = new NpgsqlConnection(ConnectionString))
        {
            try 
            {
                conn.Open();
                
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS GameResults (
                        Id SERIAL PRIMARY KEY,             
                        UserID TEXT,
                        Seed TEXT,
                        Moves INTEGER,
                        PlayTime DOUBLE PRECISION,         
                        Timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    );";

                using (var cmd = new NpgsqlCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("✅ PostgreSQL 준비 완료!");
                }
            }
            catch (NpgsqlException ex) // PostgreSQL 관련 오류만 콕 집어서 잡기
            {
                Console.WriteLine("❌ SQL 실행 중 오류 발생!");
                Console.WriteLine($"메시지: {ex.Message}");        // 무엇이 틀렸는가 (예: 문법 오류)
                Console.WriteLine($"코드: {ex.SqlState}");         // 표준 SQL 오류 코드
                Console.WriteLine($"위치: {ex.StackTrace}");       // 코드의 어느 부분에서 터졌는가
            }
            catch (Exception ex) // 그 외 일반적인 네트워크/시스템 오류 잡기
            {
                Console.WriteLine($"기타 오류: {ex.Message}");
            }
        }
    }

    // 데이터를 실제로 INSERT 하는 함수
    static void SaveResultToDb(string userId, string seed, int moves, double playTime)
    {
        using (var conn = new NpgsqlConnection(ConnectionString))
        {
            try
            {
                conn.Open();
                var command = conn.CreateCommand();

                // 1. 매개변수 기호를 $에서 @로 변경합니다.
                command.CommandText = "INSERT INTO GameResults (UserID, Seed, Moves, PlayTime) VALUES (@user, @seed, @moves, @time)";

                // 2. AddWithValue에서도 @를 사용합니다.
                // Npgsql은 데이터 타입을 자동으로 추론하므로 편리합니다.
                command.Parameters.AddWithValue("@user", userId);
                command.Parameters.AddWithValue("@seed", seed);
                command.Parameters.AddWithValue("@moves", moves);
                command.Parameters.AddWithValue("@time", playTime);

                command.ExecuteNonQuery();
            }
            catch (NpgsqlException ex) // PostgreSQL 관련 오류만 콕 집어서 잡기
            {
                Console.WriteLine("❌ SQL 실행 중 오류 발생!");
                Console.WriteLine($"메시지: {ex.Message}");        // 무엇이 틀렸는가 (예: 문법 오류)
                Console.WriteLine($"코드: {ex.SqlState}");         // 표준 SQL 오류 코드
                Console.WriteLine($"위치: {ex.StackTrace}");       // 코드의 어느 부분에서 터졌는가
            }
            catch (Exception ex) // 그 외 일반적인 네트워크/시스템 오류 잡기
            {
                Console.WriteLine($"기타 오류: {ex.Message}");
            }
        }
    }
}
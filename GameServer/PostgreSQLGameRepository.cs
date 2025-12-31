using Npgsql;
using Microsoft.Extensions.Logging;

public class PostgreSqlGameRepository : IGameRepository
{
    private readonly string _connectionString;

    // 시스템 로그
    private ILogger _logger;

    // 생성자를 통해 연결 문자열을 주입받습니다.
    public PostgreSqlGameRepository(string connectionString, ILogger logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public void Initialize()
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
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

            using var cmd = new NpgsqlCommand(createTableQuery, conn);
            cmd.ExecuteNonQuery();
            _logger.LogInformation("✅ PostgreSQL 준비 완료!");
        }
        catch (NpgsqlException ex)
        {
            LogSqlError(ex);
        }
        catch (Exception ex)
        {
            _logger.LogError($"기타 오류: {ex.Message}");
        }
    }

    public void SaveGameResult(string userId, string seed, int moves, double playTime)
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var command = conn.CreateCommand();
            command.CommandText = "INSERT INTO GameResults (UserID, Seed, Moves, PlayTime) VALUES (@user, @seed, @moves, @time)";

            command.Parameters.AddWithValue("@user", userId);
            command.Parameters.AddWithValue("@seed", seed);
            command.Parameters.AddWithValue("@moves", moves);
            command.Parameters.AddWithValue("@time", playTime);

            command.ExecuteNonQuery();
            _logger.LogInformation($"[DB 저장 완료] User: {userId}");
        }
        catch (NpgsqlException ex)
        {
            LogSqlError(ex);
        }
        catch (Exception ex)
        {
            _logger.LogError($"기타 오류: {ex.Message}");
        }
    }

    // 중복되는 에러 로그 출력 로직을 공통 메서드로 분리
    private void LogSqlError(NpgsqlException ex)
    {
        _logger.LogError("❌ SQL 실행 중 오류 발생!");
        _logger.LogError($"메시지: {ex.Message}");
        _logger.LogError($"코드: {ex.SqlState}");
        _logger.LogError($"위치: {ex.StackTrace}");
    }
}
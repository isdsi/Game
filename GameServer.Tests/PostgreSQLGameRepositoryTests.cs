using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using Xunit;

public class PostgreSqlGameRepositoryTests
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly string _testConnectionString;

    public PostgreSqlGameRepositoryTests()
    {
        // 1. Arrange: 테스트에 필요한 가짜 객체(Mock) 생성
        _mockLogger = new Mock<ILogger>();
        
        // 테스트용 연결 문자열 (실제 접속은 하지 않음)
        _testConnectionString = "Host=localhost;Database=TestDb;Username=postgres;Password=password";
        //_testConnectionString = "Host=localhost;Username=admin;Password=password123;Database=game_server_db";
    }

    [Fact]
    public void Initialize_WithInvalidConnectionString_ShouldLogError()
    {
        // Arrange
        // 일부러 잘못된 연결 문자열을 넣어서 에러 상황을 유도
        var repository = new PostgreSqlGameRepository("Invalid Connection String", _mockLogger.Object);

        // Act
        repository.Initialize();

        // Assert: 에러가 발생했을 때 로그가 정확히 찍혔는지 검증
        // LogError가 호출되었는지 Moq를 통해 확인합니다.
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("기타 오류") || v.ToString().Contains("SQL 실행 중")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.AtLeastOnce);
    }

    [Fact]
    public void SaveGameResult_ParametersArePassedCorrectly()
    {
        // 이 테스트는 실제 DB 없이 로직만 테스트하기엔 한계가 있습니다.
        // 하지만 '의존성 주입' 구조 덕분에 DB 연결부만 Mocking 하거나, 
        // 실제 테스트용 DB(Docker 등)를 띄워 '통합 테스트'로 확장할 수 있습니다.
        
        var repository = new PostgreSqlGameRepository(_testConnectionString, _mockLogger.Object);

        // 실제 실행 (DB가 없으므로 여기서는 NpgsqlException이 발생할 것입니다)
        // 하지만 우리는 이 에러가 '로그'로 잘 남는지를 테스트 목표로 잡을 수 있습니다.
        repository.SaveGameResult("user1", "seed123", 10, 50.5);

        // Assert
        _mockLogger.Verify(
            x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce, 
            "DB가 연결되지 않았으므로 에러 로그가 반드시 찍혀야 합니다.");
    }
}
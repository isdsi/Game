using Moq;
using Xunit;
using Microsoft.Extensions.Logging;

public class GameServerTests
{
    private readonly Mock<IGameRepository> _mockRepo;
    private readonly Mock<ILogger> _mockLogger;
    private readonly GameServer _sut; // System Under Test (테스트 대상)

    public GameServerTests()
    {
        _mockRepo = new Mock<IGameRepository>();
        _mockLogger = new Mock<ILogger>();
        
        // 테스트 대상을 생성하고 가짜(Mock) 객체들을 주입합니다.
        _sut = new GameServer(_mockRepo.Object, _mockLogger.Object);
    }

    [Fact]
    public void HandleClientData_ValidFormat_ShouldSaveToRepository()
    {
        // Arrange (준비)
        string validData = "User01|Seed123|50|120.5";

        // Act (실행)
        _sut.HandleClientData(validData);

        // Assert (검증): 리포지토리의 SaveGameResult가 예상한 값들로 '정확히 1번' 호출되었는지 확인
        _mockRepo.Verify(r => r.SaveGameResult(
            "User01", 
            "Seed123", 
            50, 
            120.5
        ), Times.Once);
        
        // 성공 로그가 찍혔는지도 확인 가능
        _mockLogger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("[DB 저장 완료]")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public void HandleClientData_InvalidFormat_ShouldNotSaveAndLogError()
    {
        // Arrange: 데이터 개수가 부족한 잘못된 형식
        string invalidData = "BadData|OnlyTwoParts";

        // Act
        _sut.HandleClientData(invalidData);

        // Assert: 데이터가 잘못되었으므로 저장 메서드는 '절대로' 호출되지 않아야 함
        _mockRepo.Verify(r => r.SaveGameResult(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<int>(), 
            It.IsAny<double>()
        ), Times.Never);
    }

    [Fact]
    public void HandleClientData_MalformedNumbers_ShouldHandleException()
    {
        // Arrange: 숫자가 들어갈 자리에 문자가 들어간 경우
        string malformedData = "User01|Seed123|NotANumber|45.5";

        // Act
        _sut.HandleClientData(malformedData);

        // Assert: int.Parse에서 예외가 발생했을 것이고, 에러 로그가 찍혔는지 확인
        _mockLogger.Verify(l => l.Log(
            LogLevel.Error, // 또는 LogInformation (작성하신 코드 기준)
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("처리 오류")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
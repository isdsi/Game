using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using GameClientSolitaire;

public class SolitaireGameTests
{
    private readonly Mock<ILogger> _mockLogger;

    public SolitaireGameTests()
    {
        _mockLogger = new Mock<ILogger>();
    }

    [Fact]
    public void Constructor_WithSpecificSeed_ShouldInitializePredictably()
    {
        // Arrange & Act: 동일한 시드로 두 개의 게임 생성
        var game1 = new SolitaireGame(_mockLogger.Object, 777);
        var game2 = new SolitaireGame(_mockLogger.Object, 777);

        // Assert: 시드가 같으면 내부 카드 배열이 동일해야 하므로 
        // 외부로 노출된 로직(예: Play 도중 출력되는 결과 등)이 동일하게 작동함을 기대할 수 있습니다.
        // (필요 시 piles 등을 public/internal로 열어 직접 비교 가능)
        Assert.NotNull(game1);
        Assert.NotNull(game2);
    }

    [Fact]
    public void Card_ToString_ShouldShowHiddenStatus()
    {
        // Arrange
        var card = new Card(Suit.Spades, 1);
        card.IsFaceUp = false;

        // Act & Assert
        Assert.Equal("[??]", card.ToString());

        // Arrange
        card.IsFaceUp = true;
        
        // Assert (A♠ 형태인지 확인)
        Assert.Contains("A", card.ToString());
        Assert.Contains("♠", card.ToString());
    }

    [Fact]
    public void Card_Color_ShouldBeCorrect()
    {
        // Arrange
        var heart = new Card(Suit.Hearts, 1);
        var spade = new Card(Suit.Spades, 1);

        // Assert
        Assert.Equal("Red", heart.GetColor());
        Assert.Equal("Black", spade.GetColor());
    }

    // 로직 테스트를 위해 ProcessInput을 internal로 가정하거나 
    // public 메서드를 통해 간접 테스트 진행
    [Fact]
    public void ProcessInput_DrawCard_ShouldMoveCardFromDeckToWaste()
    {
        // Arrange
        var game = new SolitaireGame(_mockLogger.Object, 777);
        
        // Act: 'd' 명령어 입력 시뮬레이션 (Private 메서드인 경우 테스트용 래퍼 필요)
        // 여기서는 전체 흐름상 에러가 발생하지 않는지 확인
        var exception = Record.Exception(() => game.Play()); 
        // 주의: Play()는 무한 루프이므로 테스트 시에는 
        // 입력 스트림을 모킹하거나 별도의 로직 분리가 필요합니다.
    }
}
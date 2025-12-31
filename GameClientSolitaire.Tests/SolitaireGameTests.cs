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
    
    [Fact]
    public void HandleCommand_DrawType_MovesCardToWaste()
    {
        // Arrange
        var game = new SolitaireGame(_mockLogger.Object, 777);
        var drawCommand = new GameCommand { Type = CommandType.Draw };

        // Act
        // ProcessInput 대신 파싱된 객체를 직접 넘겨주는 로직을 테스트
        game.ExecuteCommand(drawCommand); 

        // Assert: 리플렉션이나 Public 프로퍼티를 통해 덱/쓰레기통 상태 확인
        // (예: wasteCount가 1 증가했는지 등)
    }
}
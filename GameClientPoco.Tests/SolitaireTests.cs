using GameClientPoco;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class SolitaireTests
{
    private readonly Mock<ILogger> _mockLogger;

    private IList<Card> _deck;
    private IList<Card> _waste;
    private IList<Card>[] _foundations = new IList<Card>[Solitaire<Card>.FoundationCount];
    private IList<Card>[] _piles = new IList<Card>[Solitaire<Card>.PileCount];

    public SolitaireTests()
    {
        _mockLogger = new Mock<ILogger>();
        _deck = new List<Card>();
        _waste = new List<Card>();
        for (int i = 0; i < Solitaire<Card>.FoundationCount; i++) _foundations[i] = new List<Card>();
        for (int i = 0; i < Solitaire<Card>.PileCount; i++)
        {
            _piles[i] = new List<Card>();
        }
    }

    [Fact]
    public void Constructor_WithSpecificSeed_ShouldInitializePredictably()
    {
        // Arrange & Act: 동일한 시드로 두 개의 게임 생성
        var game1 = new Solitaire<Card>(_mockLogger.Object, _deck, _waste, _foundations, _piles,
            (s, r) => new Card(s, r), 777);
        var game2 = new Solitaire<Card>(_mockLogger.Object, _deck, _waste, _foundations, _piles,
            (s, r) => new Card(s, r), 777);

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
        Assert.Equal("[??]", ((ICard)card).GetString());

        // Arrange
        card.IsFaceUp = true;
        
        // Assert (A♠ 형태인지 확인)
        Assert.Contains("A", ((ICard)card).GetString());
        Assert.Contains("♠", ((ICard)card).GetString());
    }

    [Fact]
    public void Card_Color_ShouldBeCorrect()
    {
        // Arrange
        var heart = new Card(Suit.Hearts, 1);
        var spade = new Card(Suit.Spades, 1);

        // Assert
        heart.IsFaceUp = true;
        Assert.Equal("Red", ((ICard)heart).GetColor());
        spade.IsFaceUp = true;
        Assert.Equal("Black", ((ICard)spade).GetColor());
    }
    
    [Fact]
    public void HandleCommand_DrawType_MovesCardToWaste()
    {
        // Arrange
        var game = new Solitaire<Card>(_mockLogger.Object, _deck, _waste, _foundations, _piles,
            (s, r) => new Card(s, r), 777);
        var drawCommand = new CardCommand { Type = CommandType.Draw };

        // Act
        // ProcessInput 대신 파싱된 객체를 직접 넘겨주는 로직을 테스트
        game.ExecuteCommand(drawCommand); 

        // Assert: 리플렉션이나 Public 프로퍼티를 통해 덱/쓰레기통 상태 확인
        // (예: wasteCount가 1 증가했는지 등)
    }
    [Fact]
    public void IsGameWon_WhenAllFoundationsFull_ReturnsTrue()
    {
        // Arrange
        var game = new Solitaire<Card>(_mockLogger.Object, _deck, _waste, _foundations, _piles,
            (s, r) => new Card(s, r));

        // 리플렉션을 이용해 foundations 더미에 가짜로 13장씩 채우기
        var field = typeof(Solitaire<Card>).GetField("_foundations", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(field);
        var foundations = (IList<Card>[]?)field.GetValue(game);
        Assert.NotNull(foundations);

        for (int i = 0; i < Solitaire<Card>.FoundationCount; i++)
        {
            for (int r = 1; r <= Solitaire<Card>.SuitCardCount; r++)
            {
                foundations[i].Add(new Card((Suit)i, r));
            }
        }

        // Act
        bool won = game.IsGameWon();

        // Assert
        Assert.True(won);
    }

    [Fact]
    public void Game_WithSeed777_ShouldReachWinConditionIn143Moves()
    {
/*
        // Arrange: 동일한 시드로 게임 초기화
        var game = new Solitaire<Card>(_mockLogger.Object, _deck, _waste, _foundations, _piles,
            (s, r) => new Card(s, r), 777);

        // 143개의 명령어 리스트 (사용자가 입력했던 순서대로)
        string[] winningMoves = new[] {
            // 1 / 12
            "m 5 4 1", 
            "m 7 5 1", 
            "m 2 7 1", 
            "d", 
            "d", 
            "d", 
            "fw 1", 
            "d", 
            "d", 
            "mw 7",
            "d", 
            "mw 6", 

            // 2 / 12
            "d", 
            "d", 
            "fw 2", 
            "d", 
            "d", 
            "d", 
            "mw 5", ////** 
            "d", 
            "d", 
            "d", 
            "d", 
            "d", 

            // 3 / 12
            "mw 4", 
            "d", 
            "d", 
            "d", 
            "mw 4", 
            "mw 4", 
            "d", 
            "d", 
            "d", 
            "d",  // + 1
            "mw 3",
            "d", 
            "fw 3", 

            // 4 / 12
            "d", 
            "d", 
            "d", 
            "d", 
            "d", 
            "d", 
            "f 4 2", 
            "d", 
            "d", 
            "d", 
            "d", 
            "d", 

            // 5 / 12
            "d", 
            "mw 3", 
            "m 5 3 3", 
            "m 1 5 1", 
            "m 6 1 2", 
            "m 5 6 2", 
            "f 5 4", 
            "f 5 3", 
            "f 6 4", 
            "d", // + 1
            "mw 1", 
            "m 3 1 6", 
            "m 4 3 4", 

            // 6 / 12
            "m 6 1 2", 
            "d", 
            "mw 6", 
            "d", 
            "d", 
            "d", 
            "d", 
            "d", 
            "d", 
            "d", 
            "mw 5", 
            "d", 

            // 7 / 12
            "d", 
            "fw 1", 
            "d", 
            "mw 5", 
            "d", 
            "d", 
            "d", 
            "mw 5", 
            "m 6 5 2", 
            "f 6 2", 
            "d",  // 비었다
            "d", 

            // 8 / 12
            "d", 
            "d", 
            "d", 
            "d", 
            "d", 
            "d", 
            "fw 4", 
            "f 1 3", 
            "f 3 1", 
            "f 1 1", 
            "f 7 4", 
            "f 3 3", 

            // 9 / 12
            "f 2 1", 
            "f 3 4", 
            "f 7 3", 
            "f 3 3", 
            "f 7 4", 
            "f 7 2", 
            "f 1 2", 
            "fw 2", 
            "f 3 4", 
            "f 1 1", 
            "f 1 2", 
            "f 3 1", 

            // 10 / 12
            "m 6 2 1", 
            "f 1 4", 
            "f 6 1", 
            "f 4 1", 
            "m 4 2 1", 
            "d",  // 비었음
            "d", 
            "d", 
            "fw 3", 
            "f 4 2", 
            "f 7 2", 
            "f 5 4", 

            // 11 / 12
            "f 5 2", 
            "f 7 2", 
            "f 7 3", 
            "f 1 3", 
            "f 7 3", 
            "f 1 4", 
            "f 1 3", 
            "f 5 4", 
            "d", 
            "fw 1", 
            "fw 1",  // 비었음
            "d", 

            // 12 / 12
            "fw 3", 
            "d", 
            "f 1 4", 
            "f 2 1", 
            "f 5 2", 
            "f 1 3", 
            "f 2 2", 
            "f 5 1", 
            "fw 4", 

        };

        // Act: 143번의 명령 수행
        foreach (var move in winningMoves)
        {
            // 143번의 과정 중 어디서든 예외가 발생하면 테스트 실패
            //game.ProcessInput(move);
            CardCommand command = CommandParser.Parse(move);
            game.ExecuteCommand(command);

            // 필요하다면 각 스텝마다 중간 상태를 체크하는 코드를 넣을 수도 있음

            game.CheckFlipTopCards();
        }

        // Assert: 최종적으로 승리 조건이 달성되었는지 확인
        Assert.True(game.IsGameWon(), "143단계 후에는 반드시 승리 상태여야 합니다.");
        */
    }
}
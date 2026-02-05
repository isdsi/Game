using GameClientConsole;
using GameClientPoco;

public class CommandParserTests
{
    [Theory]
    [InlineData("m 1 2 3", CommandType.MovePileToPile, 0, 1, 3)]
    [InlineData("mwp 5", CommandType.MoveWasteToPile, 0, 4, 1)]
    [InlineData("d", CommandType.Draw, 0, 0, 1)]
    public void Parse_ValidInput_ReturnsCorrectCommand(string input, CommandType expectedType, int from, int to, int count)
    {
        // Act
        var result = CommandParser.Parse(input);

        // Assert
        Assert.Equal(expectedType, result.Type);
        Assert.Equal(from, result.From);
        Assert.Equal(to, result.To);
        Assert.Equal(count, result.Count);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("m 1 x")]    // 숫자 대신 문자
    [InlineData("mwp")]       // 인자 부족
    [InlineData("")]         // 빈 문자열
    public void Parse_InvalidInput_ReturnsInvalidCommand(string input)
    {
        // Act
        var result = CommandParser.Parse(input);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(CommandType.Unknown, result.Type);
    }
}
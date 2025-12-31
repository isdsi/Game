namespace GameClientSolitaire
{
    // 명령어의 종류를 정의
    public enum CommandType { Draw, MoveToPile, MoveToFoundation, MoveWasteToPile, MoveWasteToFoundation, Quit, Unknown }

    // 파싱된 결과를 담는 객체
    public class GameCommand
    {
        public CommandType Type { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public int Count { get; set; } = 1;
        public bool IsValid { get; set; } = true;
    }

    public static class CommandParser
    {
        public static GameCommand Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) 
                return new GameCommand { Type = CommandType.Unknown, IsValid = false };

            string[] parts = input.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string action = parts[0];

            try
            {
                return action switch
                {
                    "d" => new GameCommand { Type = CommandType.Draw },
                    "q" => new GameCommand { Type = CommandType.Quit },
                    "mw" => new GameCommand 
                    { 
                        Type = CommandType.MoveWasteToPile, 
                        To = int.Parse(parts[1]) - 1 
                    },
                    "m" => new GameCommand 
                    { 
                        Type = CommandType.MoveToPile, 
                        From = int.Parse(parts[1]) - 1, 
                        To = int.Parse(parts[2]) - 1,
                        Count = parts.Length > 3 ? int.Parse(parts[3]) : 1 
                    },
                    "f" => new GameCommand 
                    { 
                        Type = CommandType.MoveToFoundation, 
                        From = int.Parse(parts[1]) - 1, 
                        To = int.Parse(parts[2]) - 1 
                    },
                    "fw" => new GameCommand { Type = CommandType.MoveWasteToFoundation },
                    _ => new GameCommand { Type = CommandType.Unknown, IsValid = false }
                };
            }
            catch
            {
                return new GameCommand { Type = CommandType.Unknown, IsValid = false };
            }
        }
    }
}
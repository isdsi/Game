namespace GameClientPoco
{
    // 명령어의 종류를 정의
    public enum CommandType { Draw, MoveToPile, MoveToFoundation, MoveWasteToPile, MoveWasteToFoundation, Quit, Unknown }

    // 파싱된 결과를 담는 객체
    public class CardCommand
    {
        public CommandType Type { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public int Count { get; set; } = 1;
        public bool IsValid { get; set; } = true;

        public override string ToString()
        {
            return $"Type {Type.ToString()} From {From} To {To} Count {Count} IsValid {IsValid}";
        }
    }

    public static class CommandParser
    {
        public static CardCommand Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) 
                return new CardCommand { Type = CommandType.Unknown, IsValid = false };

            string[] parts = input.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string action = parts[0];

            try
            {
                return action switch
                {
                    "d" => new CardCommand { Type = CommandType.Draw },
                    "q" => new CardCommand { Type = CommandType.Quit },
                    "mw" => new CardCommand 
                    { 
                        Type = CommandType.MoveWasteToPile, 
                        To = int.Parse(parts[1]) - 1 
                    },
                    "m" => new CardCommand 
                    { 
                        Type = CommandType.MoveToPile, 
                        From = int.Parse(parts[1]) - 1, 
                        To = int.Parse(parts[2]) - 1,
                        Count = parts.Length > 3 ? int.Parse(parts[3]) : 1 
                    },
                    "f" => new CardCommand 
                    { 
                        Type = CommandType.MoveToFoundation, 
                        From = int.Parse(parts[1]) - 1, 
                        To = int.Parse(parts[2]) - 1 
                    },
                    "fw" => new CardCommand { Type = CommandType.MoveWasteToFoundation },
                    _ => new CardCommand { Type = CommandType.Unknown, IsValid = false }
                };
            }
            catch
            {
                return new CardCommand { Type = CommandType.Unknown, IsValid = false };
            }
        }
    }
}
namespace GameClientPoco
{
    public enum Suit { Spades, Hearts, Diamonds, Clubs }

    public class Card
    {
        public Suit Suit { get; }
        public int Rank { get; }
        public bool IsFaceUp { get; set; }

        public Card(Suit suit, int rank)
        {
            Suit = suit;
            Rank = rank;
            IsFaceUp = false;
        }

        public string GetColor() 
        { 
            if ((Suit == Suit.Hearts || Suit == Suit.Diamonds) && IsFaceUp == true)
                return "Red";
            else
                return "Black";
        }

        public override string ToString()
        {
            if (!IsFaceUp) return "[??]";
            string r = Rank switch
            {
                1 => "A", 11 => "J", 12 => "Q", 13 => "K", _ => Rank.ToString()
            };
            char s = Suit switch
            {
                Suit.Spades => '♠', Suit.Hearts => '♥', Suit.Diamonds => '♦', Suit.Clubs => '♣', _ => ' '
            };
            return $"[{r}{s}]";
        }
    }
}
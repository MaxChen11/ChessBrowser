namespace ChessBrowser.Components
{
    public class ChessGame
    {
        public readonly string eventName;
        public readonly string site;
        public readonly string roundNumber;
        public readonly string whitePlayer;
        public readonly string blackPlayer;
        public readonly int whiteElo;
        public readonly int blackElo;
        public readonly char result;
        public readonly DateTime eventDateTime;
        public readonly string moves;
            
        public ChessGame(string eventName, string site, string roundNumber, string whitePlayer, string blackPlayer, int whiteElo, int blackElo, char result, DateTime eventDateTime, string moves)
        {
            this.eventName = eventName;
            this.site = site;
            this.roundNumber = roundNumber;
            this.whitePlayer = whitePlayer;
            this.blackPlayer = blackPlayer;
            this.whiteElo = whiteElo;
            this.blackElo = blackElo;
            this.result = result;
            this.eventDateTime = eventDateTime;
            this.moves = moves;
        }

        public void printGame()
        {
            Console.WriteLine("Event Name: " + eventName);
            Console.WriteLine("Site: " + site);
            Console.WriteLine("Round: " + roundNumber);
            Console.WriteLine("White: " + whitePlayer);
            Console.WriteLine("Black: " + blackPlayer);
            Console.WriteLine("White Elo: " + whiteElo);
            Console.WriteLine("Black Elo: " + blackElo);
            Console.WriteLine("Result: " + result);
            Console.WriteLine("Date Time: " + eventDateTime);
            Console.WriteLine("Moves: " + moves);
        }
    }
}

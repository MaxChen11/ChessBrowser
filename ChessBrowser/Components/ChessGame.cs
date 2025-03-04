namespace ChessBrowser.Components
{
    /// <summary>
    /// Class that serves as a container for data for chess games. Outside of storage, the class has very limited functionality
    /// </summary>
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
            
        /// <summary>
        /// Creates a new ChessGame object using the given values
        /// </summary>
        /// <param name="eventName">The name of the Event that this game was played at</param>
        /// <param name="site">The site that the game was played at</param>
        /// <param name="roundNumber">The round number of the game</param>
        /// <param name="whitePlayer">The name of the white player</param>
        /// <param name="blackPlayer">The name of the black player</param>
        /// <param name="whiteElo">The white player's elo</param>
        /// <param name="blackElo">The black player's elo</param>
        /// <param name="result">The result of the game. "W" indicates the white player won, "B" indicates it was the black player, and "D" represents a draw</param>
        /// <param name="eventDateTime">The date the game took place</param>
        /// <param name="moves">The list of moves during the game</param>
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

        /// <summary>
        /// Prints out a ChessGame object's contents for testing purposes
        /// </summary>
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

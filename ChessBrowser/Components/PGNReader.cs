namespace ChessBrowser.Components
{
    /// <summary>
    /// This class takes in strings in PGN format and converts them into ChessGame objects
    /// </summary>
    public static class PGNReader
    {
        /// <summary>
        /// Takes in PGN data as an array of strings and converts the contents into a list of ChessGames
        /// </summary>
        /// <param name="chessText">The PGN text to be read</param>
        /// <returns>The list of translated ChessGame objects</returns>
        public static List<ChessGame> Read(string[] chessText)
        {
            List<ChessGame> chessGames = new List<ChessGame>();

            int blankLineCounter = 0;

            string eventName = "";
            string site = "";
            string round = "";
            string whitePlayer = "";
            string blackPlayer = "";
            int whiteElo = 0;
            int blackElo = 0;
            char result = ' ';
            DateTime eventDateTime = new DateTime();
            string moves = "";


            for (int i = 0; i < chessText.Length; i++)
            {
                if (chessText[i].StartsWith("[Event ")) {
                    int strLen = chessText[i].Length;
                    eventName = chessText[i].Substring(8, chessText[i].Length - 10);
                }
                else if (chessText[i].StartsWith("[Site "))
                {
                    site = chessText[i].Substring(7, chessText[i].Length - 9);
                }
                else if (chessText[i].StartsWith("[Round "))
                {
                    round = chessText[i].Substring(8, chessText[i].Length - 10);
                }
                else if (chessText[i].StartsWith("[White "))
                {
                    whitePlayer = chessText[i].Substring(8, chessText[i].Length - 10);
                }
                else if (chessText[i].StartsWith("[Black "))
                {
                    blackPlayer = chessText[i].Substring(8, chessText[i].Length - 10);
                }
                else if (chessText[i].StartsWith("[Result "))
                {
                    string tempResult = chessText[i].Substring(9, chessText[i].Length - 11);

                    if (tempResult == "1-0")
                    {
                        result = 'W';
                    } else if (tempResult == "0-1")
                    {
                        result = 'B';
                    } else
                    {
                        result = 'D';
                    }
                }
                else if (chessText[i].StartsWith("[WhiteElo "))
                {
                    whiteElo = Int32.Parse(chessText[i].Substring(11, chessText[i].Length - 13));
                }
                else if (chessText[i].StartsWith("[BlackElo "))
                {
                    blackElo = Int32.Parse(chessText[i].Substring(11, chessText[i].Length - 13));
                }
                else if (chessText[i].StartsWith("[EventDate "))
                {
                    if (chessText[i].Contains('?'))
                    {
                        eventDateTime = new DateTime();
                    }
                    else
                    {
                        eventDateTime = DateTime.Parse(chessText[i].Substring(12, chessText[i].Length - 14));
                    }
                }
                else if (chessText[i] == "")
                {
                    blankLineCounter++;
                    if (blankLineCounter % 2 == 0) {
                        ChessGame chessGame = new ChessGame(
                            eventName,
                            site,
                            round,
                            whitePlayer,
                            blackPlayer,
                            whiteElo,
                            blackElo,
                            result,
                            eventDateTime,
                            moves
                        );
                        blankLineCounter = 0;
                        chessGames.Add(chessGame);
                        moves = "";
                    }

                }
                else if (chessText[i].StartsWith("[ECO ") || chessText[i].StartsWith("[Date "))
                {
                    continue;
                }
                else
                {
                    moves += chessText[i];
                }
            }

            return chessGames;
        }
    }
}

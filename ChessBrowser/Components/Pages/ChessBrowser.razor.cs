﻿using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace ChessBrowser.Components.Pages
{
    public partial class ChessBrowser
    {
        /// <summary>
        /// Bound to the Unsername form input
        /// </summary>
        private string Username = "";

        /// <summary>
        /// Bound to the Password form input
        /// </summary>
        private string Password = "";

        /// <summary>
        /// Bound to the Database form input
        /// </summary>
        private string Database = "";

        /// <summary>
        /// Represents the progress percentage of the current
        /// upload operation. Update this value to update 
        /// the progress bar.
        /// </summary>
        private int Progress = 0;

        /// <summary>
        /// This method runs when a PGN file is selected for upload.
        /// Given a list of lines from the selected file, parses the 
        /// PGN data, and uploads each chess game to the user's database.
        /// </summary>
        /// <param name="PGNFileLines">The lines from the selected file</param>
        private async Task InsertGameData(string[] PGNFileLines)
        {
            // This will build a connection string to your user's database on atr,
            // assuimg you've filled in the credentials in the GUI
            string connection = GetConnectionString();

            List<ChessGame> chessGames = PGNReader.Read(PGNFileLines);

            for (int i = 0; i < chessGames.Count; i++)
            {
                chessGames[i].printGame();
            }


            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                try
                {
                    // Open a connection
                    conn.Open();
                    for(int i = 0; i < chessGames.Count; i++)
                    {
                        MySqlCommand gameCommand = conn.CreateCommand();
                        ChessGame currGame = chessGames[i];
                        //Adding prepared statements
                        gameCommand.CommandText = "insert into Players (Name, Elo) values (@wPlayer, @wElo) on duplicate key update Elo = if (@wElo > Elo, @wElo, Elo);";
                        gameCommand.CommandText += "insert into Players (Name, Elo) values (@bPlayer, @bElo) on duplicate key update Elo = if (@bElo > Elo, @bElo, Elo);";
                        gameCommand.CommandText += "insert ignore into Events (Name, Site, Date) values (@eName, @site, @date);";
                        gameCommand.CommandText += "insert ignore into Games (Round, Result, Moves, WhitePlayer, BlackPlayer, eID) values(@round, @result, @moves, (" +
                            "select pID from Players where Name = @wPlayer), (select pID from Players where Name = @bPlayer), " +
                            "(select eID from Events where Name = @eName and Site = @site and Date = @date))";

                        //Replacing placeholder values with values from chessGames
                        gameCommand.Parameters.AddWithValue("@wPlayer", currGame.whitePlayer);
                        gameCommand.Parameters.AddWithValue("@wElo", currGame.whiteElo);
                        gameCommand.Parameters.AddWithValue("@bPlayer", currGame.blackPlayer);
                        gameCommand.Parameters.AddWithValue("@bElo", currGame.blackElo);
                        gameCommand.Parameters.AddWithValue("@eName", currGame.eventName);
                        gameCommand.Parameters.AddWithValue("@site", currGame.site);
                        gameCommand.Parameters.AddWithValue("@date", currGame.eventDateTime);
                        gameCommand.Parameters.AddWithValue("@round", currGame.roundNumber);
                        gameCommand.Parameters.AddWithValue("@result", currGame.result);
                        gameCommand.Parameters.AddWithValue("@moves", currGame.moves);
                        gameCommand.ExecuteNonQuery();
                        Progress = (int)(((float)(i + 1) / (float)chessGames.Count) * 100);
                        await InvokeAsync(StateHasChanged);
                    }
                    Progress = 0;

                    // This tells the GUI to redraw after you update Progress (this should go inside your loop)
                    await InvokeAsync(StateHasChanged);


                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

        }


        /// <summary>
        /// Queries the database for games that match all the given filters.
        /// The filters are taken from the various controls in the GUI.
        /// </summary>
        /// <param name="white">The white player, or "" if none</param>
        /// <param name="black">The black player, or "" if none</param>
        /// <param name="opening">The first move, e.g. "1.e4", or "" if none</param>
        /// <param name="winner">The winner as "W", "B", "D", or "" if none</param>
        /// <param name="useDate">true if the filter includes a date range, false otherwise</param>
        /// <param name="start">The start of the date range</param>
        /// <param name="end">The end of the date range</param>
        /// <param name="showMoves">true if the returned data should include the PGN moves</param>
        /// <returns>A string separated by newlines containing the filtered games</returns>
        private string PerformQuery(string white, string black, string opening,
          string winner, bool useDate, DateTime start, DateTime end, bool showMoves)
        {
            // This will build a connection string to your user's database on atr,
            // assuimg you've typed a user and password in the GUI
            string connection = GetConnectionString();

            // Build up this string containing the results from your query
            string parsedResult = "";

            // Use this to count the number of rows returned by your query
            // (see below return statement)
            int numRows = 0;

            using (MySqlConnection conn = new MySqlConnection(connection))
            {

                try
                {
                    // Open a connection
                    conn.Open();

                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = 
                        "select Moves, " +
                        "Events.Name as eName, " +
                        "Site, " +
                        "Date, " +
                        "wPlayers.Name as wName, bPlayers.Name as bName, " +
                        "wPlayers.Elo as wElo, bPlayers.Elo as bElo, " +
                        "Result from Games natural join Events join (Players wPlayers) join (Players bPlayers) " +
                        "where wPlayers.pID = WhitePlayer and bPlayers.pID = BlackPlayer ";

                    String whitePlayerCheck = "";
                    String blackPlayerCheck = "";
                    String openingMoveCheck = "";
                    String winnerCheck = "";
                    String dateCheck = "";


                    if (!String.IsNullOrEmpty(white))
                    {
                        whitePlayerCheck += "wPlayers.Name = @wPlayer";
                    }
                    if (!String.IsNullOrEmpty(black))
                    {
                        blackPlayerCheck += "bPlayers.Name = @bPlayer";
                    }
                    if (!String.IsNullOrEmpty(opening))
                    {
                        openingMoveCheck += "Moves like @opening";
                    }
                    if (!String.IsNullOrEmpty(winner) && !winner.Equals("Any"))
                    {
                        winnerCheck += "Result=@winner";
                    }
                    if (useDate)
                    {
                        dateCheck = "Date >= @startDate AND Date <= @endDate";
                    }


                    String[] conditionStrings = { whitePlayerCheck, blackPlayerCheck, openingMoveCheck, winnerCheck, dateCheck };

                    for (int i = 0; i < conditionStrings.Length; i++) {
                        if (conditionStrings[i] == "")
                        {
                            continue;
                        }

                        command.CommandText += " and ";

                        command.CommandText += conditionStrings[i];
                    }


                    command.Parameters.AddWithValue("@wPlayer", white);
                    command.Parameters.AddWithValue("@bPlayer", black);
                    command.Parameters.AddWithValue("@opening", opening + "%");
                    if(winner.Length > 0) command.Parameters.AddWithValue("@winner", winner[0]);
                    command.Parameters.AddWithValue("@startDate", start);
                    command.Parameters.AddWithValue("@endDate", end);

                    command.CommandText += ";";


                    using (MySqlDataReader reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            numRows++;
                            parsedResult += "Event: " + reader["eName"] + "\n";
                            parsedResult += "Site: " + reader["Site"] + "\n";
                            parsedResult += "Date: " + reader["Date"] + "\n";
                            parsedResult += "White: " + reader["wName"] + " (" + reader["wElo"] + ")\n";
                            parsedResult += "Black: " + reader["bName"] + " (" + reader["bElo"] + ")\n";
                            parsedResult += "Result: " + reader["Result"] + "\n";

                            if (showMoves) {
                                parsedResult += reader["Moves"] + "\n";
                            }
                            parsedResult += "\n";

                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            Console.WriteLine("Parsed Result:" + parsedResult);
            return numRows + " results\n" + parsedResult;
        }


        private string GetConnectionString()
        {
            return "server=atr.eng.utah.edu;database=" + Database + ";uid=" + Username + ";password=" + Password;
        }


        /// <summary>
        /// This method will run when the file chooser is used.
        /// It loads the files contents as an array of strings,
        /// then invokes the InsertGameData method.
        /// </summary>
        /// <param name="args">The event arguments, which contains the selected file name</param>
        private async void HandleFileChooser(EventArgs args)
        {
            try
            {
                string fileContent = string.Empty;

                InputFileChangeEventArgs eventArgs = args as InputFileChangeEventArgs ?? throw new Exception("unable to get file name");
                if (eventArgs.FileCount == 1)
                {
                    var file = eventArgs.File;
                    if (file is null)
                    {
                        return;
                    }

                    // load the chosen file and split it into an array of strings, one per line
                    using var stream = file.OpenReadStream(1000000); // max 1MB
                    using var reader = new StreamReader(stream);
                    fileContent = await reader.ReadToEndAsync();
                    string[] fileLines = fileContent.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                    // insert the games, and don't wait for it to finish
                    // _ = throws away the task result, since we aren't waiting for it
                    _ = InsertGameData(fileLines);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("an error occurred while loading the file..." + e);
            }
        }

    }

}

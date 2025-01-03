class Program
{
    static int x = 1, y = 1;
    static int length = 0;
    static int[] foodPos = new int[2] { 5, 5 };
    static readonly Random rng = new();
    static int direction = 1;
    static List<int[]> posList = new() { new int[] { x, y } }; // Intitialze Pos List
    static bool paused = false;
    const char snakeSegment = '#';

    static void AppleChecker()
    {
        if (x == foodPos[0] && y == foodPos[1])
        {
            length++;
            foodPos[0] = rng.Next(1, 34); foodPos[1] = rng.Next(1, 11);
        }
    }

    static void DrawApple()
    {
        Console.SetCursorPosition(foodPos[0], foodPos[1]);
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write("*");
    }

/*
      Input Map

          0
      
    3  -  |  -  1

          2
*/

    static void InputHandler()
    {
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (direction != 2 && !paused || length == 0 && !paused) direction = 0;
                    break;

                case ConsoleKey.DownArrow:
                    if (direction != 0 && !paused || length == 0 && !paused) direction = 2;
                    break;

                case ConsoleKey.LeftArrow:
                    if (direction != 1 && !paused || length == 0 && !paused) direction = 3;
                    break;

                case ConsoleKey.RightArrow:
                    if (direction != 3 && !paused || length == 0 && !paused) direction = 1;
                    break;

                case ConsoleKey.Escape:
                    // Clear pause message if paused.
                    if (paused)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.SetCursorPosition(6, 5);
                        Console.WriteLine("                       ");
                        Console.SetCursorPosition(7, 6);
                        Console.WriteLine("                    ");

                        // Rewrite le snek
                        foreach (int[] pos in posList)
                        {
                            Console.SetCursorPosition(pos[0], pos[1]);
                            Console.Write(snakeSegment);
                        }
                    }
                    // Enable/Disable pausing.
                    paused = !paused;
                    break;

                case ConsoleKey.M:
                    if (paused) Main(null);
                    break;
            }
        }
    }

    public static void Main(string[] args)
    {
        Console.Title = "Crap Snake"; Console.CursorVisible = false; Console.ForegroundColor = ConsoleColor.Gray; Console.WindowHeight = 12; Console.WindowWidth = 35; // Visual Shit

        #region New High Score Check
        if (length > 0) 
        {
            bool firstScore = false;
            Console.Clear(); Console.ForegroundColor = ConsoleColor.Gray;
            if (!File.Exists("scores.dat"))
            {
                File.WriteAllText("scores.dat", "");
                firstScore = true;
            }
            string[] scores = File.ReadAllLines("scores.dat");
            int score;
            // Check to see if the scores file was already there.
            if (firstScore) score = 0;
            else score = int.Parse(scores[0].Split(';')[1]);
            if (length > score)
            {
                Console.WriteLine("New High Score!");
                Console.Write("Please write your name: ");
                string name = Console.ReadLine();

                // Put new high score on top.
                File.WriteAllText("scores.dat", $"{name};{length}");
                foreach (string s in scores)
                    File.AppendAllText("scores.dat", "\n" + s);
            }
        }
        #endregion

        Console.Clear();
        Console.WriteLine("===== Crap Snake =====\n");
        Console.WriteLine("[1] Play Game");
        Console.WriteLine("[2] High Scores");
        Console.WriteLine("[3] Quit Game");
        while (true)
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.D1) Game();
            if (key == ConsoleKey.D2)
            {
                Console.Clear();
                if (!File.Exists("scores.dat")) Console.WriteLine("No High Scores.");
                else
                {
                    string[] scores = File.ReadAllLines("scores.dat");
                    foreach (string scoreLine in scores)
                    {
                        string name = scoreLine.Split(";")[0];
                        int score = int.Parse(scoreLine.Split(";")[1]);
                        Console.WriteLine(name + " : " + score);
                    }
                }
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
                Main(null);
            }
            if (key == ConsoleKey.D3) Environment.Exit(0);
        }
    }

    public static void Game()
    {
        Console.Clear();

        // Reset game variables
        x = 1; y = 1;
        direction = 1;
        length = 0;
        posList.Clear();
        posList.Add(new int[2] { x, y });

        bool gameRunning = true;
        int gameSpeed = 100;
        paused = false;

        #region Draw Border
        Console.ForegroundColor = ConsoleColor.DarkGray;
        for (int i = 0; i < 35; i++)
        {
            Console.SetCursorPosition(i, 0);
            Console.Write("█");
        }
        for (int i = 0; i < 35; i++)
        {
            Console.SetCursorPosition(i, 11);
            Console.Write("█");
        }
        for (int i = 1; i < 11; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write("█");
        }
        for (int i = 1; i < 11; i++)
        {
            Console.SetCursorPosition(34, i);
            Console.Write("█");
        }
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        #endregion

        while (gameRunning)
        {
            Thread.Sleep(gameSpeed);
            InputHandler();

            // Clear the buffer to prevent the movement from "locking up."
            while (Console.KeyAvailable) Console.ReadKey(true);

            if (!paused) // Game Loop
            {
                // Direction Handler
                switch (direction)
                {
                    case 0: // Up
                        y--;
                        break;
                    case 2: // Down
                        y++;
                        break;
                    case 1: // Left
                        x++;
                        break;
                    case 3: // Right
                        x--;
                        break;
                }

                AppleChecker();
                Console.SetCursorPosition(x, y); Console.Write(snakeSegment); // Draw Snake Segment
                DrawApple();

                // Redraw Right Border To Fix Removal Bug (please C# let me do stuff without having to hack you up with a hacksaw every 5 seconds..)
                Console.ForegroundColor = ConsoleColor.DarkGray;
                for (int i = 1; i < 11; i++)
                {
                    Console.SetCursorPosition(34, i);
                    Console.Write("█");
                }
                // Draw in score and set color for snake.
                Console.SetCursorPosition(13, 0);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Score:{length:D3}");

                // Collision Check w/ Walls
                if (x == 34 || x == 0) gameRunning = false;
                if (y == 11 || y == 0) gameRunning = false;

                if (posList.Count > length)
                {
                    Console.SetCursorPosition(posList[0][0], posList[0][1]);
                    Console.Write(" ");
                    posList.RemoveAt(0);
                }

                // Collision Check w/ Self
                foreach (int[] pos in posList)
                    if (pos[0] == x && pos[1] == y) gameRunning = false;

                posList.Add(new int[2] { x, y });
            }
            else // Pause Menu
            {
                Console.SetCursorPosition(6, 5);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Paused. Esc to unpause.");
                Console.SetCursorPosition(7, 6);
                Console.WriteLine("M to goto main menu.");
            }
        }

        #region Game Over Message
        Console.SetCursorPosition(12, 5);
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("Game Over!");
        Thread.Sleep(1000);
        // Just clear the buffer real quick to prevent the "Press any key." message from being skipped early.
        while (Console.KeyAvailable) Console.ReadKey(true);
        Console.SetCursorPosition(10, 6);
        Console.WriteLine("Press any key.");
        #endregion
        Console.ReadKey(true);
        Main(null);
    }
}
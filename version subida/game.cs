using System;
using System.Collections.Generic;
using System.Threading;
using Spectre.Console;
using System.Text;

class Program
{
    static int width = 15;
    static int height = 15;
    static int[,] maze = new int[height, width];
    static List<Trap> traps = new List<Trap>();
    static List<Trap> swapTraps = new List<Trap>();
    static List<Trap> stunTraps = new List<Trap>();
    static int player1StunTurns = 0;
    static int player2StunTurns = 0;
    
    static List<Player> players = new List<Player>
    {
        new Player("P1", "Puede teletransportarse hacia la salida cada dos turnos. Tecla: T", "Guerrero desconocido de un lugar lejano a las afuera del reino"),
        new Player("P2", "Puede cambiar de posición con el otro jugador cada tres turnos. Tecla: B", "Arquero preciso perteneciente a la artillería del reino que busca más aventura como forma de mejorar su vida"),
        new Player("P3", "Puede aturdir al otro jugador por 2 turnos. Tecla: R", "Asesino que deserta de su clan se esconde en uno de los gremios mercenarios y terminó por giros del destino uniéndose a esta peligrosa aventura"),
        new Player("P4", "Puede colocar trampas aleatorias por el tablero. Tecla: P", "Mago de gran prestigio en busca de lo desconocido y recompensas para continuar con su investigación")
    };

    static Player selectedPlayer1 = new Player("Placeholder", "Placeholder", "Placeholder");
    static Player selectedPlayer2 = new Player("Placeholder", "Placeholder", "Placeholder");

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        ShowPlayerSelectionMenu();
        ShowAdventureStory();
        do
        {
            InitializeMaze();
            GenerateMaze(1, 1);
        } while (!IsSolvable());

        PlaceTraps();
        PlaceSwapTraps();
        PlaceStunTraps();
        StartGame();
    }

    static void ShowPlayerSelectionMenu()
    {
        AnsiConsole.Write(new FigletText("Menu de Seleccion").Centered().Color(Color.Aqua));
        
        // Selección del primer jugador
        selectedPlayer1 = AnsiConsole.Prompt(
            new SelectionPrompt<Player>()
                .Title("[bold yellow]Elige el primer jugador[/]")
                .UseConverter(player => $"{player.Name} - {player.PowerDescription}")
                .AddChoices(players));

        AnsiConsole.Write(new Panel($"Primer jugador seleccionado: [bold yellow]{selectedPlayer1.Name}[/]\n{selectedPlayer1.PowerDescription}")
            .Border(BoxBorder.Double).BorderColor(Color.Yellow).Header("[bold yellow]Jugador 1[/]"));
        
        // Selección del segundo jugador
        selectedPlayer2 = AnsiConsole.Prompt(
            new SelectionPrompt<Player>()
                .Title("[bold blue]Elige el segundo jugador[/]")
                .UseConverter(player => $"{player.Name} - {player.PowerDescription}")
                .AddChoices(players));

        while (selectedPlayer1 == selectedPlayer2)
        {
            AnsiConsole.MarkupLine("[red]Los jugadores no pueden ser el mismo. Elija un jugador diferente.[/]");
            selectedPlayer2 = AnsiConsole.Prompt(
                new SelectionPrompt<Player>()
                    .Title("[bold yellow]Elige el segundo jugador[/]")
                    .UseConverter(player => $"{player.Name} - {player.PowerDescription}")
                    .AddChoices(players));
        }

        AnsiConsole.Write(new Panel($"Segundo jugador seleccionado: [bold blue]{selectedPlayer2.Name}[/]\n{selectedPlayer2.PowerDescription}")
            .Border(BoxBorder.Double).BorderColor(Color.Grey).Header("[bold blue]Jugador 2[/]"));
    }

    static void ShowAdventureStory()
    {
        AnsiConsole.Write(new FigletText("La Aventura Comienza").Color(Color.Green).Centered());

        string story = $"{selectedPlayer1.Name} y {selectedPlayer2.Name} se embarcaron en una emocionante aventura. \n\n" +
                       $"{selectedPlayer1.Story}\n\n{selectedPlayer2.Story}\n\n" +
                       "Juntos, enfrentaron los peligros del laberinto y desafiaron las trampas que encontraban a su paso, en busca de la salida y la gloria.";

        string[] storyLines = story.Split('\n');

        foreach (string line in storyLines)
        {
            foreach (char c in line)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    Console.Write(line.Substring(line.IndexOf(c)));
                    break;
                }
                Console.Write(c);
                Thread.Sleep(50);
            }
            Console.WriteLine();
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter) continue;
            Thread.Sleep(500);
        }

        Console.WriteLine("\n\n¡La aventura está a punto de comenzar!");
        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter) return;
        Thread.Sleep(2000);
    }

    static void ShowVictoryStory(Player winner)
    {
        string victoryStory = $"Después de muchos desafíos, {winner.Name} finalmente encontró la salida del laberinto. \n\n" +
                              $"{winner.Story}\n\n" +
                              "Con valor y determinación, sobrevivió a todas las trampas y peligros del laberinto, " +
                              "emergiendo como un verdadero héroe. La gloria y las riquezas de la victoria pertenecen a este ganador. ¡Bien hecho, valiente aventurero!";

        string[] victoryLines = victoryStory.Split('\n');

        foreach (string line in victoryLines)
        {
            foreach (char c in line)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    Console.Write(line.Substring(line.IndexOf(c)));
                    break;
                }
                Console.Write(c);
                Thread.Sleep(50);
            }
            Console.WriteLine();
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter) continue;
            Thread.Sleep(500);
        }

        Console.WriteLine("\n\n¡Fin de la aventura!");
        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter) return;
    }

    static void InitializeMaze()
    {
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                maze[i, j] = 1;
    }

    static void GenerateMaze(int startX, int startY)
    {
        maze[startY, startX] = 0;
        HashSet<(int x, int y)> walls = new HashSet<(int x, int y)>();
        AddWalls(startX, startY, walls);

        Random rand = new Random();

        while (walls.Count > 0)
        {
            var wallList = new List<(int x, int y)>(walls);
            var wall = wallList[rand.Next(walls.Count)];
            walls.Remove(wall);

            if (CountAdjacentPaths(wall.x, wall.y) == 1)
            {
                maze[wall.y, wall.x] = 0;
                AddWalls(wall.x, wall.y, walls);
            }
        }

        maze[height - 2, width - 2] = 0;
        maze[height - 3, width - 2] = 0;
        maze[height - 2, width - 3] = 0;
    }

    static void AddWalls(int x, int y, HashSet<(int x, int y)> walls)
    {
        foreach (var direction in new[] { (2, 0), (-2, 0), (0, 2), (0, -2) })
        {
            int nx = x + direction.Item1;
            int ny = y + direction.Item2;

            if (IsInBounds(nx, ny) && maze[ny, nx] == 1)
            {
                walls.Add((x + direction.Item1 / 2, y + direction.Item2 / 2));
            }
        }
    }

    static int CountAdjacentPaths(int x, int y)
    {
        int count = 0;
        foreach (var direction in new[] { (-1, 0), (1, 0), (0, -1), (0, 1) })
        {
            int nx = x + direction.Item1;
            int ny = y + direction.Item2;
            if (IsInBounds(nx, ny) && maze[ny, nx] == 0)
                count++;
        }
        return count;
    }

    static bool IsInBounds(int x, int y) => x > 0 && x < width - 1 && y > 0 && y < height - 1;

   static void PlaceTraps()
{
    Random rand = new Random();
    int trapCount = rand.Next(3, 5);

    for (int i = 0; i < trapCount; i++)
    {
        int trapX, trapY;
        do
        {
            trapX = rand.Next(1, width - 1);
            trapY = rand.Next(1, height - 1);
        } while (maze[trapY, trapX] != 0 || (trapX == width - 2 && trapY == height - 2));
        
        traps.Add(new Trap(trapX, trapY));
    }
}

static void PlaceSwapTraps()
{
    Random rand = new Random();
    int trapCount = 2;

    for (int i = 0; i < trapCount; i++)
    {
        int trapX, trapY;
        do
        {
            trapX = rand.Next(1, width - 1);
            trapY = rand.Next(1, height - 1);
        } while (maze[trapY, trapX] != 0 || (trapX == width - 2 && trapY == height - 2) || traps.Exists(t => t.Position == (trapX, trapY)));
        
        swapTraps.Add(new Trap(trapX, trapY));
    }
}

static void PlaceStunTraps()
{
    Random rand = new Random();
    int trapCount = 2;

    for (int i = 0; i < trapCount; i++)
    {
        int trapX, trapY;
        do
        {
            trapX = rand.Next(1, width - 1);
            trapY = rand.Next(1, height - 1);
        } while (maze[trapY, trapX] != 0 || (trapX == width - 2 && trapY == height - 2) || traps.Exists(t => t.Position == (trapX, trapY)) || swapTraps.Exists(t => t.Position == (trapX, trapY)));
        
        stunTraps.Add(new Trap(trapX, trapY));
    }
}

static void StartGame()
{
    Random rand = new Random();
    int player1X, player1Y, player2X, player2Y;

    do
    {
        player1X = rand.Next(1, width - 1);
        player1Y = rand.Next(1, height - 1);
    } while (maze[player1Y, player1X] != 0);

    do
    {
        player2X = rand.Next(1, width - 1);
        player2Y = rand.Next(1, height - 1);
    } while (maze[player2Y, player2X] != 0 || (player2X == player1X && player2Y == player1Y));

    int turns = 0;

    while (true)
    {
        Console.Clear();
        PrintMazeWithPlayers(player1X, player1Y, player2X, player2Y);

        if (player1StunTurns > 0) player1StunTurns--;
        if (player2StunTurns > 0) player2StunTurns--;

        if (turns % 2 == 0) // Turno del Jugador 1
        {
            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.T && turns % 2 == 0)
            {
                TeleportTowardsExit(ref player1X, ref player1Y, width - 2, height - 2);
            }
            else if (key == ConsoleKey.B && turns % 3 == 0)
            {
                SwapPositions(ref player1X, ref player1Y, ref player2X, ref player2Y);
            }
            else if (key == ConsoleKey.R)
            {
                StunOtherPlayer(ref player2StunTurns);
            }
            else if (key == ConsoleKey.P)
            {
                PlaceRandomTrap(rand);
            }
            else
            {
                // Movimiento estándar
                if (key == ConsoleKey.W && player1Y > 0 && maze[player1Y - 1, player1X] == 0) player1Y--;
                if (key == ConsoleKey.S && player1Y < height - 1 && maze[player1Y + 1, player1X] == 0) player1Y++;
                if (key == ConsoleKey.A && player1X > 0 && maze[player1Y, player1X - 1] == 0) player1X--;
                if (key == ConsoleKey.D && player1X < width - 1 && maze[player1Y, player1X + 1] == 0) player1X++;
            }
            ActivateTrap(ref player1X, ref player1Y, ref player2X, ref player2Y, rand, ref player1StunTurns);
        }
        else // Turno del Jugador 2 (IA)
        {
            MoveAIPlayerTowardsExit(ref player2X, ref player2Y);
            ActivateTrap(ref player2X, ref player2Y, ref player1X, ref player1Y, rand, ref player2StunTurns);
        }

        turns++;

        if (player1X == width - 2 && player1Y == height - 2)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[yellow bold]******************************[/]");
            AnsiConsole.MarkupLine("[yellow bold]*                            *[/]");
            AnsiConsole.MarkupLine("[yellow bold]*[/] [green bold] ¡Jugador 1 gana! [/][yellow bold]*[/]");
            AnsiConsole.MarkupLine("[yellow bold]*                            *[/]");
            AnsiConsole.MarkupLine("[yellow bold]******************************[/]");
            AnsiConsole.Write(new FigletText("¡Victoria!").Color(Color.Yellow).Centered());
            AnsiConsole.Markup("\n[bold green]¡Jugador 1 ha encontrado la salida del laberinto![/]\n");
            AnsiConsole.Markup("[bold cyan]¡Eres un verdadero maestro explorador![/]");
            ShowVictoryStory(selectedPlayer1);
            break;
        }

        if (player2X == width - 2 && player2Y == height - 2)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[yellow bold]******************************[/]");
            AnsiConsole.MarkupLine("[yellow bold]*                            *[/]");
            AnsiConsole.MarkupLine("[yellow bold]*[/] [green bold] ¡Jugador 2 gana! [/][yellow bold]*[/]");
            AnsiConsole.MarkupLine("[yellow bold]*                            *[/]");
            AnsiConsole.MarkupLine("[yellow bold]******************************[/]");
            AnsiConsole.Write(new FigletText("¡Victoria!").Color(Color.Yellow).Centered());
            AnsiConsole.Markup("\n[bold green]¡Jugador 2 ha encontrado la salida del laberinto![/]\n");
            AnsiConsole.Markup("[bold cyan]¡Eres un verdadero maestro explorador![/]");
            ShowVictoryStory(selectedPlayer2);
            break;
        }
    }
}

static void MoveAIPlayerTowardsExit(ref int playerX, ref int playerY)
{
    var path = AStar(playerX, playerY, width - 2, height - 2);
    if (path != null && path.Count > 1)
    {
        playerX = path[1].Item1;
        playerY = path[1].Item2;
    }
}


static void ActivateTrap(ref int playerX, ref int playerY, ref int otherPlayerX, ref int otherPlayerY, Random rand, ref int playerStunTurns)
{
    var playerPosition = (playerX, playerY);

    // Trampa normal: teletransportar a un lugar aleatorio
    var trap = traps.Find(t => t.Position == playerPosition);
    if (trap != null)
    {
        Teleport(ref playerX, ref playerY, rand);
        traps.Remove(trap);
    }

    // Trampa de cambio de posición
    var swapTrap = swapTraps.Find(t => t.Position == playerPosition);
    if (swapTrap != null)
    {
        int tempX = playerX;
        int tempY = playerY;
        playerX = otherPlayerX;
        playerY = otherPlayerY;
        otherPlayerX = tempX;
        otherPlayerY = tempY;
        swapTraps.Remove(swapTrap);
    }

    // Trampa de estupor: aplicar estupor
    var stunTrap = stunTraps.Find(t => t.Position == playerPosition);
    if (stunTrap != null)
    {
        playerStunTurns = 3;
        stunTraps.Remove(stunTrap);
    }
}

static bool DFS(int x, int y, bool[,] visited)
{
    if (x == width - 2 && y == height - 2) return true;

    visited[y, x] = true;

    foreach (var direction in new[] { (-1, 0), (1, 0), (0, -1), (0, 1) })
    {
        int nx = x + direction.Item1;
        int ny = y + direction.Item2;

        if (IsInBounds(nx, ny) && maze[ny, nx] == 0 && !visited[ny, nx])
        {
            if (DFS(nx, ny, visited)) return true;
        }
    }

    return false;
}

static bool IsSolvable()
{
    bool[,] visited = new bool[height, width];
    return DFS(1, 1, visited);
}

static List<(int, int)> AStar(int startX, int startY, int goalX, int goalY)
{
    var openSet = new SortedSet<(int fScore, int gScore, int x, int y)>(); // Sorted by fScore, then gScore
    var cameFrom = new Dictionary<(int, int), (int, int)>();
    var gScore = new Dictionary<(int, int), int> { [(startX, startY)] = 0 };
    var fScore = new Dictionary<(int, int), int> { [(startX, startY)] = Heuristic(startX, startY, goalX, goalY) };

    openSet.Add((fScore[(startX, startY)], gScore[(startX, startY)], startX, startY));

    while (openSet.Count > 0)
    {
        var current = openSet.Min;
        openSet.Remove(current);

        int currentX = current.x;
        int currentY = current.y;

        if (currentX == goalX && currentY == goalY)
        {
            var path = new List<(int, int)>();
            var node = (currentX, currentY);
            while (cameFrom.ContainsKey(node))
            {
                path.Add(node);
                node = cameFrom[node];
            }
            path.Reverse();
            return path;
        }

        foreach (var direction in new[] { (-1, 0), (1, 0), (0, -1), (0, 1) })
        {
            int neighborX = currentX + direction.Item1;
            int neighborY = currentY + direction.Item2;

            if (!IsInBounds(neighborX, neighborY) || maze[neighborY, neighborX] != 0)
                continue;

            int tentativeGScore = gScore[(currentX, currentY)] + 1;

            if (!gScore.ContainsKey((neighborX, neighborY)) || tentativeGScore < gScore[(neighborX, neighborY)])
            {
                cameFrom[(neighborX, neighborY)] = (currentX, currentY);
                gScore[(neighborX, neighborY)] = tentativeGScore;
                fScore[(neighborX, neighborY)] = gScore[(neighborX, neighborY)] + Heuristic(neighborX, neighborY, goalX, goalY);

                openSet.Add((fScore[(neighborX, neighborY)], gScore[(neighborX, neighborY)], neighborX, neighborY));
            }
        }
    }

    return null!; // No path found
}

static int Heuristic(int x1, int y1, int x2, int y2)
{
    return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
}


static void Teleport(ref int playerX, ref int playerY, Random rand)
{
    do
    {
        playerX = rand.Next(1, width - 1);
        playerY = rand.Next(1, height - 1);
    } while (maze[playerY, playerX] != 0 || (playerX == width - 2 && playerY == height - 2));
}

static void TeleportTowardsExit(ref int playerX, ref int playerY, int goalX, int goalY)
{
    var path = AStar(playerX, playerY, goalX, goalY);
    if (path != null && path.Count > 2) // Avanzar un paso
    {
        playerX = path[1].Item1;
        playerY = path[1].Item2;
    }
}

static void SwapPositions(ref int playerX, ref int playerY, ref int otherPlayerX, ref int otherPlayerY)
{
    int tempX = playerX;
    int tempY = playerY;
    playerX = otherPlayerX;
    playerY = otherPlayerY;
    otherPlayerX = tempX;
    otherPlayerY = tempY;
}

static void StunOtherPlayer(ref int otherPlayerStunTurns)
{
    otherPlayerStunTurns = 2;
}

static void PlaceRandomTrap(Random rand)
{
    int trapX, trapY;
    do
    {
        trapX = rand.Next(1, width - 1);
        trapY = rand.Next(1, height - 1);
    } while (maze[trapY, trapX] != 0 || traps.Exists(t => t.Position == (trapX, trapY)) || swapTraps.Exists(t => t.Position == (trapX, trapY)) || stunTraps.Exists(t => t.Position == (trapX, trapY)));

    traps.Add(new Trap(trapX, trapY));
}

static void PrintMazeWithPlayers(int player1X, int player1Y, int player2X, int player2Y)
{
    AnsiConsole.Write(new FigletText("Laberinto del Juego").Color(Color.Aqua).Centered());

    // Panel del Laberinto
    var mazeTable = new Table();
    mazeTable.Border(TableBorder.None);

    for (int i = 0; i < width; i++)
    {
        mazeTable.AddColumn(new TableColumn(string.Empty));
    }

    for (int i = 0; i < height; i++)
    {
        var row = new List<Markup>();
        for (int j = 0; j < width; j++)
        {
            if (i == player1Y && j == player1X)
                row.Add(new Markup($"[bold yellow]{selectedPlayer1.Name}[/]")); // Jugador 1
            else if (i == player2Y && j == player2X)
                row.Add(new Markup($"[bold blue]{selectedPlayer2.Name}[/]")); // Jugador 2
            else if (i == height - 2 && j == width - 2)
                row.Add(new Markup("[bold green]S[/]")); // Salida
            else if (traps.Exists(t => t.Position == (j, i)))
                row.Add(new Markup("[bold magenta]T[/]")); // Trampa normal
            else if (swapTraps.Exists(t => t.Position == (j, i)))
                row.Add(new Markup("[bold cyan]X[/]")); // Trampa de cambio de posición
            else if (stunTraps.Exists(t => t.Position == (j, i)))
                row.Add(new Markup("[bold red]Z[/]")); // Trampa de estupor
            else if (maze[i, j] == 1)
                row.Add(new Markup("[red]█[/]")); // Pared
            else
                row.Add(new Markup("[white].[/]")); // Camino
        }
        mazeTable.AddRow(row.ToArray());
    }

    // Panel de habilidades
    var abilitiesTable = new Table();
    abilitiesTable.Border(TableBorder.None)
                  .AddColumn(new TableColumn("[bold]Jugador[/]"))
                  .AddColumn(new TableColumn("[bold]Habilidad[/]"))
                  .AddColumn(new TableColumn("[bold]Tecla[/]"));

    abilitiesTable.AddRow(selectedPlayer1.Name, selectedPlayer1.PowerDescription, GetPowerKey(selectedPlayer1.Name));
    abilitiesTable.AddRow(selectedPlayer2.Name, selectedPlayer2.PowerDescription, GetPowerKey(selectedPlayer2.Name));

    // Panel de historia de los personajes
    var storiesTable = new Table();
    storiesTable.Border(TableBorder.None)
                .AddColumn(new TableColumn("[bold]Jugador[/]"))
                .AddColumn(new TableColumn("[bold]Historia[/]"));

    storiesTable.AddRow(selectedPlayer1.Name, selectedPlayer1.Story);
    storiesTable.AddRow(selectedPlayer2.Name, selectedPlayer2.Story);

    AnsiConsole.Write(
        new Rows(
            new Panel(mazeTable).Header("Laberinto").BorderColor(Color.Silver),
            new Panel(abilitiesTable).Header("Habilidades").BorderColor(Color.Silver),
            new Panel(storiesTable).Header("Historias").BorderColor(Color.Silver)
        ));
}

static string GetPowerKey(string playerName)
{
    switch (playerName)
    {
        case "P1": return "T"; // Teletransportarse hacia la salida
        case "P2": return "B"; // Cambiar de posición con el otro jugador
        case "P3": return "R"; // Aturdir al otro jugador
        case "P4": return "P"; // Colocar trampas aleatorias
        default: return string.Empty;
    }
}
}
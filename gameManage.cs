 using System;
using System.Collections.Generic;
using System.Threading;
using Spectre.Console;
using System.Text;
using System.Media;

class GameManager
{
    public static int width = 15;
    public static int height = 15;
    public static int[,] maze = new int[height, width];
    public static List<Trap> traps = new List<Trap>();
    public static List<Trap> swapTraps = new List<Trap>();
   
public static List<Trap> knockbackTraps = new List<Trap>();

    public static int player1StunTurns = 0;
    public static int player2StunTurns = 0;
    public static bool isAIPlayer2 = false;

   public static List<Player> players = new List<Player>
{
    new Player("P1", "Puede teletransportarse a una posición aleatoria del laberinto. Tecla: T", "Guerrero desconocido de un lugar lejano a las afuera del reino"),
    new Player("P2", "Puede moverse un paso más cerca de la salida usando la teletransportación. Tecla: T", "Arquero preciso perteneciente a la artillería del reino que busca más aventura como forma de mejorar su vida"),
    new Player("P3", "Puede intercambiar su posición con la del otro jugador. Tecla: B", "Asesino que deserta de su clan se esconde en uno de los gremios mercenarios y terminó por giros del destino uniéndose a esta peligrosa aventura"),
    new Player("P4", "Puede aturdir al otro jugador durante 2 turnos. Tecla: R", "Mago de gran prestigio en busca de lo desconocido y recompensas para continuar con su investigación"),
    new Player("P5", "Puede colocar trampas aleatorias por el tablero. Tecla: P", "Guerrero desconocido de un lugar lejano a las afuera del reino")
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
            Logica.InitializeMaze();
           
            Logica.GenerateMaze(1, 1);
        } while (!Logica.IsSolvable());

        Logica.PlaceTraps();
        Logica.PlaceSwapTraps();
        Logica.PlaceKnockbackTraps();
        StartGame();
    }
static void ShowPlayerSelectionMenu()
{
  //  PlaySound(@"E:\maze en consola\sounds\sound_main_menu.wav");
    AnsiConsole.Write(new FigletText("Menu de Seleccion").Centered().Color(Color.Aqua));

    selectedPlayer1 = AnsiConsole.Prompt(
        new SelectionPrompt<Player>()
            .Title("[bold yellow]Elige el primer jugador[/]")
            .UseConverter(player => $"{player.Name} - {player.PowerDescription}")
            .AddChoices(players));

    AnsiConsole.Write(new Panel($"Primer jugador seleccionado: [bold yellow]{selectedPlayer1.Name}[/]\n{selectedPlayer1.PowerDescription}")
        .Border(BoxBorder.Double).BorderColor(Color.Yellow).Header("[bold yellow]Jugador 1[/]"));

    // Pregunta si el segundo jugador es IA
    isAIPlayer2 = AnsiConsole.Confirm("¿Deseas jugar contra la IA?");

    if (!isAIPlayer2)
    {
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
    else
    {
        selectedPlayer2 = new Player("IA", "Controlado por un spiritu maligno", "Jugador controlado por un spiritu , el jugador se mueve dos casillas.");
        AnsiConsole.Write(new Panel($"Segundo jugador: [bold blue]{selectedPlayer2.Name}[/]\n{selectedPlayer2.PowerDescription}")
            .Border(BoxBorder.Double).BorderColor(Color.Grey).Header("[bold blue]Jugador 2[/]"));
    }
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

     static void StartGame()
{
   /* StopSound(); // Detener la música del menú 
    PlaySound(@"E:\maze en consola\sounds\sound_game.wav");*/
    Random rand = new Random();
    int player1X, player1Y, player2X, player2Y;

    // Definir la distancia mínima a la salida
    int minDistanceToExit = 3;

    do
    {
        player1X = rand.Next(1, width - 1);
        player1Y = rand.Next(1, height - 1);
    } while (maze[player1Y, player1X] != 0 || IsNearExit(player1X, player1Y, minDistanceToExit));

    do
    {
        player2X = rand.Next(1, width - 1);
        player2Y = rand.Next(1, height - 1);
    } while (maze[player2Y, player2X] != 0 || (player2X == player1X && player2Y == player1Y) || IsNearExit(player2X, player2Y, minDistanceToExit));

    int turns = 0;
    while (true)
        {
            Console.Clear();
            PrintMazeWithPlayers(player1X, player1Y, player2X, player2Y);

            if (player1StunTurns > 0) player1StunTurns--;
            if (player2StunTurns > 0) player2StunTurns--;

            if (turns % 2 == 0) // Turno del Jugador 1
            {
                if (player1StunTurns == 0) // Verifica si el jugador 1 no está aturdido
                {
                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.T)
                    {
                        selectedPlayer1.TeleportTowardsExit(ref player1X, ref player1Y, width - 2, height - 2);
                    }
                    else if (key == ConsoleKey.B)
                    {
                        selectedPlayer1.SwapPositions(ref player1X, ref player1Y, ref player2X, ref player2Y);
                    }
                    else if (key == ConsoleKey.R)
                    {
                        selectedPlayer1.StunOtherPlayer(ref player2StunTurns);
                    }
                    else if (key == ConsoleKey.P)
                    {
                        selectedPlayer1.PlaceRandomTrap(rand);
                    }
                    else
                    {
                        if (key == ConsoleKey.W && player1Y > 0 && maze[player1Y - 1, player1X] == 0) player1Y--;
                        if (key == ConsoleKey.S && player1Y < height - 1 && maze[player1Y + 1, player1X] == 0) player1Y++;
                        if (key == ConsoleKey.A && player1X > 0 && maze[player1Y, player1X - 1] == 0) player1X--;
                        if (key == ConsoleKey.D && player1X < width - 1 && maze[player1Y, player1X + 1] == 0) player1X++;
                    }
                  Trap.ActivateTrap(ref player1X, ref player1Y, ref player2X, ref player2Y, rand, selectedPlayer1); // Actualizar esta línea


                }
            }
            else // Turno del Jugador 2
            {
                if (!isAIPlayer2) // Jugador 2 humano
                {
                    if (player2StunTurns == 0) // Verifica si el jugador 2 no está aturdido
                    {
                        var key = Console.ReadKey(true).Key;

                        if (key == ConsoleKey.T)
                        {
                            selectedPlayer2.TeleportTowardsExit(ref player2X, ref player2Y, width - 2, height - 2);
                        }
                        else if (key == ConsoleKey.B)
                        {
                            selectedPlayer2.SwapPositions(ref player2X, ref player2Y, ref player1X, ref player1Y);
                        }
                        else if (key == ConsoleKey.R)
                        {
                            selectedPlayer2.StunOtherPlayer(ref player1StunTurns);
                        }
                        else if (key == ConsoleKey.P)
                        {
                            selectedPlayer2.PlaceRandomTrap(rand);
                        }
                        else
                        {
                            if (key == ConsoleKey.W && player2Y > 0 && maze[player2Y - 1, player2X] == 0) player2Y--;
                            if (key == ConsoleKey.S && player2Y < height - 1 && maze[player2Y + 1, player2X] == 0) player2Y++;
                            if (key == ConsoleKey.A && player2X > 0 && maze[player2Y, player2X - 1] == 0) player2X--;
                            if (key == ConsoleKey.D && player2X < width - 1 && maze[player2Y, player2X + 1] == 0) player2X++;
                        }
                       Trap.ActivateTrap(ref player2X, ref player2Y, ref player1X, ref player1Y, rand, selectedPlayer2); // Actualizar esta línea
                    }
                }
                else if ( turns % 2 != 0)// Jugador 2 controlado por la IA
                {
                    if (player2StunTurns == 0)
                    {
                        Logica.MoveAIPlayerTowardsExit(ref player2X, ref player2Y, width -1, height -1);
                        Trap.ActivateTrap(ref player2X, ref player2Y, ref player1X, ref player1Y, rand, selectedPlayer2);
                    }
                }
            }

            if (player1X == width - 2 && player1Y == height - 2)
            {
              /*  StopSound(); // Detener la música del juego 
                PlaySound(@"E:\maze en consola\sounds\sound_victory.wav");*/
                Console.Clear();
                AnsiConsole.MarkupLine("[yellow bold]******[/]");
                AnsiConsole.MarkupLine("[yellow bold]*                            *[/]");
                AnsiConsole.MarkupLine("[yellow bold]*[/] [green bold] ¡Jugador 1 gana! [/][yellow bold]*[/]");
                AnsiConsole.MarkupLine("[yellow bold]*                            *[/]");
                AnsiConsole.MarkupLine("[yellow bold]******[/]");
                AnsiConsole.Write(new FigletText("¡Victoria!").Color(Color.Yellow).Centered());
                AnsiConsole.Markup("\n[bold green]¡Jugador 1 ha encontrado la salida del laberinto![/]\n");
                AnsiConsole.Markup("[bold cyan]¡Eres un verdadero maestro explorador![/]");
                ShowVictoryStory(selectedPlayer1);
                break;
            }

            if (player2X == width - 2 && player2Y == height - 2)
            {
             /*   StopSound(); // Detener la música del juego 
                PlaySound(@"E:\maze en consola\sounds\sound_victory.wav");*/
                Console.Clear();
                AnsiConsole.MarkupLine("[yellow bold]******[/]");
                AnsiConsole.MarkupLine("[yellow bold]*                            *[/]");
                AnsiConsole.MarkupLine("[yellow bold]*[/] [green bold] ¡Jugador 2 gana! [/][yellow bold]*[/]");
                AnsiConsole.MarkupLine("[yellow bold]*                            *[/]");
                AnsiConsole.MarkupLine("[yellow bold]******[/]");
                AnsiConsole.Write(new FigletText("¡Victoria!").Color(Color.Yellow).Centered());
                AnsiConsole.Markup("\n[bold green]¡Jugador 2 ha encontrado la salida del laberinto![/]\n");
                AnsiConsole.Markup("[bold cyan]¡Eres un verdadero maestro explorador![/]");
                ShowVictoryStory(selectedPlayer2);
                break;
            }

            turns++;
        }
    }
static bool IsNearExit(int x, int y, int minDistance) 
   { 
    int exitX = width - 2;
    int exitY = height - 2; 
        return Math.Abs(x - exitX) < minDistance && Math.Abs(y - exitY) < minDistance;
     }
    static void PrintMazeWithPlayers(int player1X, int player1Y, int player2X, int player2Y)
    {
        AnsiConsole.Write(new FigletText("Laberinto del Juego").Color(Color.Aqua).Centered());

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
                else if (knockbackTraps.Exists(t => t.Position == (j, i))) // Añade esta línea
                    row.Add(new Markup("[bold red]K[/]")); // Añade esta línea

                else if (maze[i, j] == 1)
                    row.Add(new Markup("[red]█[/]")); // Pared
                else
                    row.Add(new Markup("[white].[/]")); // Camino
            }
            mazeTable.AddRow(row.ToArray());
        }

        var abilitiesTable = new Table();
        abilitiesTable.Border(TableBorder.None)
                      .AddColumn(new TableColumn("[bold]Jugador[/]"))
                      .AddColumn(new TableColumn("[bold]Habilidad[/]"))
                      .AddColumn(new TableColumn("[bold]Tecla[/]"));

        abilitiesTable.AddRow(selectedPlayer1.Name, selectedPlayer1.PowerDescription, GetPowerKey(selectedPlayer1.Name));
        abilitiesTable.AddRow(selectedPlayer2.Name, selectedPlayer2.PowerDescription, GetPowerKey(selectedPlayer2.Name));

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
    return playerName switch
    {
        "P1" => "T", // Teletransportarse a una posición aleatoria
        "P2" => "T", // Moverse un paso más cerca de la salida usando la teletransportación
        "P3" => "B", // Intercambiar posición con el otro jugador
        "P4" => "R", // Aturdir al otro jugador
        "P5" => "P", // Colocar trampas aleatorias
        _ => string.Empty,
    };
}
/*static void PlaySound(string soundPath)
{
    SoundPlayer player = new SoundPlayer(soundPath);
    player.PlayLooping(); // Reproduce la música en bucle
}
static void StopSound()
{
    SoundPlayer player = new SoundPlayer();
    player.Stop();
}*/

}

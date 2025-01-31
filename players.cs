using System;

public class Player
{
    // Propiedades del jugador
    public string Name { get; set; } // Nombre del jugador
    public string PowerDescription { get; set; } // Descripción del poder especial del jugador
    public string Story { get; set; } // Historia o trasfondo del jugador
    
    // Cantidad de usos disponibles para cada poder
    private int teleportUses = 3;
    private int teleportTowardsExitUses = 3;
    private int swapPositionsUses = 3;
    private int stunOtherPlayerUses = 3;
    private int placeRandomTrapUses = 3;

    // Constructor para inicializar el jugador con su nombre, poder y trasfondo
    public Player(string name, string powerDescription, string story)
    {
        Name = name;
        PowerDescription = powerDescription;
        Story = story;
    }

    // Método para verificar si un poder puede ser usado (tiene intentos disponibles)
    public static bool CanUsePower(ref int uses)
    {
        if (uses > 0)
        {
            uses--; // Disminuye la cantidad de usos disponibles
            return true;
        }
        else
        {
            Console.WriteLine("[red]El poder no se puede utilizar más de tres veces.[/]");
            return false;
        }
    }

    // Método para teletransportar al jugador a una posición aleatoria dentro del laberinto
    public void Teleport(ref int playerX, ref int playerY, Random rand)
    {
        if (CanUsePower(ref teleportUses))
        {
            do
            {
                // Se generan coordenadas aleatorias dentro de los límites del laberinto
                playerX = rand.Next(1, GameManager.width - 1);
                playerY = rand.Next(1, GameManager.height - 1);
            } while (GameManager.maze[playerY, playerX] != 0 || (playerX == GameManager.width - 2 && playerY == GameManager.height - 2));
            // Repite el proceso hasta encontrar una celda vacía (0) que no sea la salida
        }
    }

    // Método para teletransportar al jugador hacia la salida utilizando el algoritmo A*
    public void TeleportTowardsExit(ref int playerX, ref int playerY, int goalX, int goalY)
    {
        if (CanUsePower(ref teleportTowardsExitUses))
        {
            var path = Logica.FindPath(playerX, playerY, goalX, goalY);
            if (path != null && path.Count > 1)
            {
                // Mueve al jugador un paso hacia la salida
                playerX = path[1].Item1;
                playerY = path[1].Item2;
            }
        }
    }

    // Método para intercambiar posiciones con otro jugador
    public void SwapPositions(ref int playerX, ref int playerY, ref int otherPlayerX, ref int otherPlayerY)
    {
        if (CanUsePower(ref swapPositionsUses))
        {
            // Se intercambian las coordenadas de ambos jugadores
            int tempX = playerX;
            int tempY = playerY;
            playerX = otherPlayerX;
            playerY = otherPlayerY;
            otherPlayerX = tempX;
            otherPlayerY = tempY;
        }
    }

    // Método para aturdir a otro jugador durante dos turnos
    public void StunOtherPlayer(ref int otherPlayerStunTurns)
    {
        if (CanUsePower(ref stunOtherPlayerUses))
        {
            otherPlayerStunTurns = 2; // Se asigna una penalización de 2 turnos al jugador rival
        }
    }

    // Método para colocar una trampa en una posición aleatoria del laberinto
    public void PlaceRandomTrap(Random rand)
    {
        if (CanUsePower(ref placeRandomTrapUses))
        {
            int trapX, trapY;
            do
            {
                // Genera coordenadas aleatorias dentro del laberinto
                trapX = rand.Next(1, GameManager.width - 1);
                trapY = rand.Next(1, GameManager.height - 1);
            } while (GameManager.maze[trapY, trapX] != 0 || GameManager.traps.Exists(t => t.Position == (trapX, trapY)) || 
                     GameManager.swapTraps.Exists(t => t.Position == (trapX, trapY)) || GameManager.knockbackTraps.Exists(t => t.Position == (trapX, trapY)));
            // Asegura que la trampa no se coloque en una pared ni en una celda ocupada por otra trampa

            // Se añade la nueva trampa a la lista de trampas del juego
            GameManager.traps.Add(new Trap(trapX, trapY));
        }
    }
}

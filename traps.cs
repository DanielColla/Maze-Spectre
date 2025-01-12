using System;
using System.Collections.Generic;

class Trap
{
    public (int x, int y) Position { get; set; }

    public Trap(int x, int y)
    {
        Position = (x, y);
    }

    public static void ActivateTrap(ref int playerX, ref int playerY, ref int otherPlayerX, ref int otherPlayerY, Random rand, Player player)
    {
        var playerPosition = (playerX, playerY);

        // Trampa normal: teletransportar a un lugar aleatorio
        var trap = GameManager.traps.Find(t => t.Position == playerPosition);
        if (trap != null)
        {
            player.Teleport(ref playerX, ref playerY, rand);
            GameManager.traps.Remove(trap);
            return;
        }

        // Trampa de cambio de posición
        var swapTrap = GameManager.swapTraps.Find(t => t.Position == playerPosition);
        if (swapTrap != null)
        {
            player.SwapPositions(ref playerX, ref playerY, ref otherPlayerX, ref otherPlayerY);
            GameManager.swapTraps.Remove(swapTrap);
            return;
        }

        // Trampa de retroceso: mover al jugador 3 pasos hacia atrás
        var knockbackTrap = GameManager.knockbackTraps.Find(t => t.Position == playerPosition);
        if (knockbackTrap != null)
        {
            MoveBackwards(ref playerX, ref playerY, 3);
            GameManager.knockbackTraps.Remove(knockbackTrap);
            return;
        }
    }

    static void MoveBackwards(ref int playerX, ref int playerY, int steps)
    {
        // Lógica para mover al jugador hacia atrás de manera óptima.
        // La dirección del movimiento se ajustará según la posición actual del jugador.
        // Supongamos que moverse hacia atrás significa moverse en la dirección opuesta de la entrada del laberinto.

        // Determinar la nueva posición basada en los pasos dados hacia atrás.
        for (int i = 0; i < steps; i++)
        {
            if (IsValidMove(playerX, playerY - 1)) // Intentar moverse hacia arriba
            {
                playerY -= 1;
            }
            else if (IsValidMove(playerX - 1, playerY)) // Si no puede, intentar moverse hacia la izquierda
            {
                playerX -= 1;
            }
            else if (IsValidMove(playerX + 1, playerY)) // Si no puede, intentar moverse hacia la derecha
            {
                playerX += 1;
            }
            else if (IsValidMove(playerX, playerY + 1)) // Si no puede, intentar moverse hacia abajo
            {
                playerY += 1;
            }
        }
    }

    static bool IsValidMove(int x, int y)
    {
        // Verifica si la nueva posición está dentro de los límites del laberinto y no es una pared.
        return x >= 0 && x < GameManager.width && y >= 0 && y < GameManager.height && GameManager.maze[y, x] == 0;
    }
}

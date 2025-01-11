using System;
using System.Collections.Generic;

class Trap
{
    public (int x, int y) Position { get; set; }

    public Trap(int x, int y)
    {
        Position = (x, y);
    }

    // efectos de las trampas
    public static void ActivateTrap(ref int playerX, ref int playerY, ref int otherPlayerX, ref int otherPlayerY, Random rand, ref int playerStunTurns, Player player)
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

        // Trampa de estupor: aplicar estupor
        var stunTrap = GameManager.stunTraps.Find(t => t.Position == playerPosition);
        if (stunTrap != null)
        {
            playerStunTurns = 2; // Asegúrate de establecer los turnos de estupor directamente aquí
            GameManager.stunTraps.Remove(stunTrap);
        }
    }
}

using System;

public class Player
{
    public string Name { get; set; }
    public string PowerDescription { get; set; }
    public string Story { get; set; }
    
    private int teleportUses = 3;
    private int teleportTowardsExitUses = 3;
    private int swapPositionsUses = 3;
    private int stunOtherPlayerUses = 3;
    private int placeRandomTrapUses = 3;

    public Player(string name, string powerDescription, string story)
    {
        Name = name;
        PowerDescription = powerDescription;
        Story = story;
    }

    public static bool CanUsePower(ref int uses)
    {
        if (uses > 0)
        {
            uses--;
            return true;
        }
        else
        {
            Console.WriteLine($"[red]El poder no se puede utilizar más de tres veces.[/]");
            return false;
        }
    }

    public void Teleport(ref int playerX, ref int playerY, Random rand)
    {
        if (CanUsePower(ref teleportUses))
        {
            do
            {
                playerX = rand.Next(1, GameManager.width - 1);
                playerY = rand.Next(1, GameManager.height - 1);
            } while (GameManager.maze[playerY, playerX] != 0 || (playerX == GameManager.width - 2 && playerY == GameManager.height - 2));
        }
    }

    public void TeleportTowardsExit(ref int playerX, ref int playerY, int goalX, int goalY)
    {
        if (CanUsePower(ref teleportTowardsExitUses))
        {
            var path = Logica.FindPath(playerX, playerY, goalX, goalY);
            if (path != null && path.Count > 1)
            {
                playerX = path[1].Item1;
                playerY = path[1].Item2;
            }
        }
    }

    public void SwapPositions(ref int playerX, ref int playerY, ref int otherPlayerX, ref int otherPlayerY)
    {
        if (CanUsePower(ref swapPositionsUses))
        {
            int tempX = playerX;
            int tempY = playerY;
            playerX = otherPlayerX;
            playerY = otherPlayerY;
            otherPlayerX = tempX;
            otherPlayerY = tempY;
        }
    }

    public void StunOtherPlayer(ref int otherPlayerStunTurns)
    {
        if (CanUsePower(ref stunOtherPlayerUses))
        {
            otherPlayerStunTurns = 2;
        }
    }

    public void PlaceRandomTrap(Random rand)
    {
        if (CanUsePower(ref placeRandomTrapUses))
        {
            int trapX, trapY;
            do
            {
                trapX = rand.Next(1, GameManager.width - 1);
                trapY = rand.Next(1, GameManager.height - 1);
            } while (GameManager.maze[trapY, trapX] != 0 || GameManager.traps.Exists(t => t.Position == (trapX, trapY)) || GameManager.swapTraps.Exists(t => t.Position == (trapX, trapY)) || GameManager.stunTraps.Exists(t => t.Position == (trapX, trapY)));

            GameManager.traps.Add(new Trap(trapX, trapY));
        }
    }
}

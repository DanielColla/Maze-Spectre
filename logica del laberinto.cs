using System;
using System.Collections.Generic;

class Logica
{
     // Variables globales que toman valores desde GameManager
    public static int width = GameManager.width;
    public static int height = GameManager.height;
    public static int[,] maze = GameManager.maze;
    public static List<Trap> traps = GameManager.traps;
    public static List<Trap> swapTraps = GameManager.swapTraps;
    public static List<Trap> knockbackTraps = GameManager.knockbackTraps;

    // Inicializa el laberinto llenándolo completamente de paredes (valor 1)
    public static void InitializeMaze()
    {
        for (int i = 0; i < height; i++) // Recorre cada fila
            for (int j = 0; j < width; j++) // Recorre cada columna en la fila actual
                maze[i, j] = 1; // Asigna 1 (pared) a cada celda
    }
    // Genera un laberinto aleatorio utilizando el algoritmo de Prim
    public static void GenerateMaze(int startX, int startY)
    {
         // Se marca el punto inicial como camino
        maze[startY, startX] = 0; // Punto de inicio del laberinto
        HashSet<(int x, int y)> walls = new HashSet<(int x, int y)>();
        AddWalls(startX, startY, walls);

        Random rand = new Random();

        while (walls.Count > 0) // Mientras haya paredes candidatas, seguimos generando camino
        {
            var wallList = new List<(int x, int y)>(walls);
            var wall = wallList[rand.Next(walls.Count)]; // Selecciona una pared al azar
            walls.Remove(wall);
            // Si la pared tiene un solo camino adyacente, se convierte en camino
            if (CountAdjacentPaths(wall.x, wall.y) == 1) // Asegura solo una conexión
            {
                maze[wall.y, wall.x] = 0; // Convierte la pared en un camino
                AddWalls(wall.x, wall.y, walls); // Agrega nuevas paredes al conjunto
            }
        }

        // Asegura que la salida sea accesible
        maze[height - 2, width - 2] = 0;
        maze[height - 3, width - 2] = 0;
        maze[height - 2, width - 3] = 0;
    }

    // Agrega paredes adyacentes a una celda para ser consideradas en la generación del laberinto
    public static void AddWalls(int x, int y, HashSet<(int x, int y)> walls)
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

    // Cuenta cuántos caminos existen alrededor de una celda
    public static int CountAdjacentPaths(int x, int y)
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
      // Verifica si una celda está dentro de los límites del laberinto
    public static bool IsInBounds(int x, int y) => x > 0 && x < width - 1 && y > 0 && y < height - 1;
     // Coloca trampas aleatorias en el laberinto
    public static void PlaceTraps()
    {
        Random rand = new Random();
        int trapCount = rand.Next(3, 5); // Número aleatorio de trampas entre 3 y 5

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

    public static void PlaceSwapTraps()
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

public static void PlaceKnockbackTraps() // coloca las trampas de retroceso
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
        } while (maze[trapY, trapX] != 0 || (trapX == width - 2 && trapY == width - 2) || traps.Exists(t => t.Position == (trapX, trapY)) || swapTraps.Exists(t => t.Position == (trapX, trapY)));

        knockbackTraps.Add(new Trap(trapX, trapY));
    }
}

// Algoritmo de búsqueda A* para encontrar la mejor ruta
    public static void MoveAIPlayerTowardsExit(ref int playerX, ref int playerY)
    {
        var path = FindPath(playerX, playerY, width - 2, height - 2);
        if (path != null && path.Count > 0)
        {
            playerX = path[0].Item1;
            playerY = path[0].Item2;
        }
  
    }

    public static List<(int, int)> FindPath(int startX, int startY, int goalX, int goalY)
    {
        var openSet = new List<(int x, int y)>();// Lista de nodos por explorar
        var cameFrom = new Dictionary<(int, int), (int, int)>();//ruta optima
        var gScore = new Dictionary<(int, int), int> { [(startX, startY)] = 0 };
        var fScore = new Dictionary<(int, int), int> { [(startX, startY)] = Heuristic(startX, startY, goalX, goalY) };

        openSet.Add((startX, startY));
        // Seleccionamos el nodo con menor costo fScore
        while (openSet.Count > 0)
        {
            var current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (fScore[openSet[i]] < fScore[current] ||
                    (fScore[openSet[i]] == fScore[current] && gScore[openSet[i]] < gScore[current]))
                {
                    current = openSet[i];
                }
            }
           // Si llegamos al objetivo, reconstruimos el camino
            if (current.x == goalX && current.y == goalY)
            {
                var path = new List<(int, int)>();
                var temp = current;
                while (cameFrom.ContainsKey(temp))
                {
                    path.Add(temp);
                    temp = cameFrom[temp];
                }
                path.Reverse();
                return path;
            }

            openSet.Remove(current);
// Explorar vecinos (arriba, abajo, izquierda, derecha)
            foreach (var direction in new[] { (-1, 0), (1, 0), (0, -1), (0, 1) })
            {
                var neighbor = (x: current.x + direction.Item1, y: current.y + direction.Item2);
// Si el vecino está fuera del laberinto o es una pared, lo ignoramos
                if (!IsInBounds(neighbor.x, neighbor.y) || maze[neighbor.y, neighbor.x] != 0)
                    continue;

                int tentativeGScore = gScore[current] + 1;
                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor.x, neighbor.y, goalX, goalY);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
  

        }

        return null!; // No path found
    }
  // Heurística de Manhattan para A*
    public static int Heuristic(int x1, int y1, int x2, int y2)
    {
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }

    public static bool DFS(int x, int y, bool[,] visited)
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

    public static bool IsSolvable()
    {
        bool[,] visited = new bool[height, width];
        return DFS(1, 1, visited);
    }
}


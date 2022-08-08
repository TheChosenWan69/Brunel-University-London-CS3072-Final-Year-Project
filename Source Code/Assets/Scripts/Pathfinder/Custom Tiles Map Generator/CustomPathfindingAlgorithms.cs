using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomPathfindingAlgorithms : MonoBehaviour
{
    private CustomTilesMapGenerator _customTilesMapGenerator;
    private void Awake()
    {
        _customTilesMapGenerator = FindObjectOfType<CustomTilesMapGenerator>();
    }

    // Algorithm selection for the drop down menu.
    public Algorithms _currentAlgorithm;
    private Dropdown dropDown;

    // Assigns each corresponding number to an algorithm (for the dropdown menu selection).
    public enum Algorithms
    {
        BreadthFirstSearch = 0,
        Dijkstra = 1,
        AStar = 2
    }

    // Matches the enum to the Algorithims you want to select.
    private void Start()
    {
        dropDown = FindObjectOfType<Dropdown>();
        dropDown.onValueChanged.AddListener(OnAlgorithmChanged);
        dropDown.value = PlayerPrefs.GetInt("currentAlgorithm");
    }

    // When the algorithm is selected, it will change the algorithm accordingly.
    public void OnAlgorithmChanged(int algorithmID)
    {
        // Current algorithim = current enum
        _currentAlgorithm = (Algorithms)algorithmID;
        // Looks for the TilesPath script and calls the TileAssigner function (to calculate the path) again.
        FindObjectOfType<CustomTilesPath>().TileAssigner();
        // Saves the selected algorithim.
        PlayerPrefs.SetInt("currentAlgorithm", (int)algorithmID);
        PlayerPrefs.Save();
    }

    // Finds a path from the starting tile to end tile, corresponding to the algorithm that was selected. It will return a queue, which contains the tiles that the unit must move on.
    public Queue<TilesType> FindUnitPath(TilesType start, TilesType end)
    {
        switch (_currentAlgorithm)
        {
            case Algorithms.BreadthFirstSearch:
                return BreadthFirstSearch(start, end);
            case Algorithms.Dijkstra:
                return Dijkstra(start, end);
            case Algorithms.AStar:
                return AStar(start, end);
        }
        return null;
    }

    // Priority Queue system for Djikstra and A* algorithms
    public class PriorityQueue<T>
    {
        private List<Tuple<T, int>> elements = new List<Tuple<T, int>>();

        public int Count
        {
            get { return elements.Count; }
        }

        public void Enqueue(T item, int priority)
        {
            elements.Add(Tuple.Create(item, priority));
        }

        public T Dequeue()
        {
            int bestIndex = 0;
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Item2 < elements[bestIndex].Item2)
                {
                    bestIndex = i;
                }
            }
            T bestItem = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }

    // Manhattan Distance Heuristic for A*  - Calculates the distance between current tile and the destination of the end tile (how many tiles the unit must move to reach it).
    // Mathf.Abs turns the result of this calculation from a negative to a positive value.
    // (X coordinates from tile 1 - X coordinates from tile 2) + (Y coordinates from tile 1 - Y coordinates from tile 2).
    int Distance(TilesType t1, TilesType t2)
    {
        return Mathf.Abs(t1._X - t2._X) + Mathf.Abs(t1._Y - t2._Y);
    }

    // Algorithms
    // Has start and goal tile as parameters - The algorithm returns the path as a queue of tiles. When you need to dequeue, the algorithm returns the first tile you need to move towards.

    Queue<TilesType> BreadthFirstSearch(TilesType start, TilesType goal)
    {
        // List of collections needed for the algorithm.
        Dictionary<TilesType, TilesType> nextTileToGoal = new Dictionary<TilesType, TilesType>(); // Stores the tile order you need to go on the map.
        Queue<TilesType> frontier = new Queue<TilesType>(); // Saves the tiles you are going to look at next with the algorithm.
        List<TilesType> visited = new List<TilesType>(); // Saves the tiles that we have been to already to a list with the algorithm.

        // Puts the goal tile in the frontier.
        frontier.Enqueue(goal);

        // Runs a loop whilst we still have tiles to inspect in the frontier.
        while (frontier.Count > 0)
        {
            // Current tile is the first thing in the frontier.
            TilesType currentTile = frontier.Dequeue();

            // TilsMapGenerator stores the neighbors of each tile in a dictionary. Can access the neighbors of each tile stored in array, by calling Neighbors(currentTile). Runs in a foreach loop where it runs for all the neighbors there are currently.
            foreach (TilesType neighbor in _customTilesMapGenerator.Neighbors(currentTile))
            {
                // If the neighbor not in the visted list, and the frontier does not contain the neighbor, we will put the neighbor in the frontier.
                if (visited.Contains(neighbor) == false && frontier.Contains(neighbor) == false)
                {
                    // Ignores the TileType walls in the grid.
                    if (neighbor._TileType != TilesType.TileType.Wall)
                    {
                        // Queues in the neighbor
                        frontier.Enqueue(neighbor);
                        // If the unit comes to the tile of the neighbor, unit will set it as the current tile and move there.
                        nextTileToGoal[neighbor] = currentTile;
                    }
                }
            }
            // Adds current tiles to the visited tile.
            visited.Add(currentTile);
        }

        // If statement ensures that null (a path cannot be found) is returned if you can't reach the starting position (due to walls).
        if (visited.Contains(start) == false)
            return null;

        // Generates a path with the queue
        Queue<TilesType> path = new Queue<TilesType>();
        // Tile that the unit is currently standing on.
        TilesType currentPathTile = start;
        // Loop that fills the queue with tiles that the unit needs to move towards.
        while (currentPathTile != goal)
        {
            currentPathTile = nextTileToGoal[currentPathTile];
            path.Enqueue(currentPathTile);
        }

        // Returns path.
        return path;
    }

    Queue<TilesType> Dijkstra(TilesType start, TilesType goal)
    {
        // List of collections needed for the algorithm.
        Dictionary<TilesType, TilesType> NextTileToGoal = new Dictionary<TilesType, TilesType>(); // Stores the tile order you need to go on the map.
        Dictionary<TilesType, int> costToReachTile = new Dictionary<TilesType, int>(); // Retrieves the movement cost of each tile.
        PriorityQueue<TilesType> frontier = new PriorityQueue<TilesType>(); // Uses a priority queue

        // Puts the goal tile in the frontier, and specifies a number priority in the queue (can be 0 or 1) - makes it so one tile has extremely high priority in the queue.
        frontier.Enqueue(goal, 0);
        // Sets it to 0, as there is no cost to reach the goal if you're there already.
        costToReachTile[goal] = 0;

        // Runs a loop whilst we still have tiles to inspect in the frontier.
        while (frontier.Count > 0)
        {
            // Current tile is the first thing in the frontier.
            TilesType currentTile = frontier.Dequeue();
            // If the starting position is found, it will exit early.
            if (currentTile == start)
                break;

            // TilsMapGenerator stores the neighbors of each tile in a dictionary. Can access the neighbors of each tile stored in array, by calling Neighbors(currentTile). Runs in a foreach loop where it runs for all the neighbors there are currently.
            foreach (TilesType neighbor in _customTilesMapGenerator.Neighbors(currentTile))
            {
                // Saves the cost to move to the neighbour.
                int newCost = costToReachTile[currentTile] + neighbor._movementCost;
                // If the new cost is less than the current cost to reach the current tile, and it has not been visited yet.
                if (costToReachTile.ContainsKey(neighbor) == false || newCost < costToReachTile[neighbor])
                {
                    // Ignores the TileType walls in the grid.
                    if (neighbor._TileType != TilesType.TileType.Wall)
                    {
                        // Cost to reach neighboring tile = new cost. Overwrite the previous higher cost that might have been in it.
                        costToReachTile[neighbor] = newCost;
                        // Sets the priority = new cost (unnecessary line, but it will be used adjusted for A*).
                        int priority = newCost;
                        // Enqueue the neighbor against the priority.
                        frontier.Enqueue(neighbor, priority);
                        // If the unit comes to the tile of the neighbor, unit will set it as the current tile and move there.
                        NextTileToGoal[neighbor] = currentTile;
                        // Uses the tile's text field to display the movement cost to move on it.
                        neighbor._Text = costToReachTile[neighbor].ToString();
                    }
                }
            }
        }

        // If statement ensures that null (a path cannot be found) is returned if you can't reach the starting position (due to walls).
        if (NextTileToGoal.ContainsKey(start) == false)
        {
            return null;
        }

        // Generates a path with the queue
        Queue<TilesType> path = new Queue<TilesType>();
        // Tile that the unit is currently standing on.
        TilesType currentPathTile = start;
        // Loop that fills the queue with tiles that the unit needs to move towards.
        while (goal != currentPathTile)
        {
            currentPathTile = NextTileToGoal[currentPathTile];
            path.Enqueue(currentPathTile);
        }

        // Returns path.
        return path;
    }

    Queue<TilesType> AStar(TilesType start, TilesType goal)
    {
        // List of collections needed for the algorithm.
        Dictionary<TilesType, TilesType> NextTileToGoal = new Dictionary<TilesType, TilesType>(); // Stores the tile order you need to go on the map.
        Dictionary<TilesType, int> costToReachTile = new Dictionary<TilesType, int>(); // Retrieves the movement cost of each tile.
        PriorityQueue<TilesType> frontier = new PriorityQueue<TilesType>(); // Uses a priority queue

        // Puts the goal tile in the frontier, and specifies a number priority in the queue (can be 0 or 1) - makes it so one tile has extremely high priority in the queue.
        frontier.Enqueue(goal, 0);
        // Sets it to 0, as there is no cost to reach the goal if you're there already.
        costToReachTile[goal] = 0;

        // Runs a loop whilst we still have tiles to inspect in the frontier.
        while (frontier.Count > 0)
        {
            // Current tile is the first thing in the frontier.
            TilesType currentTile = frontier.Dequeue();
            // If the starting position is found, it will exit early.
            if (currentTile == start)
                break;

            // TilsMapGenerator stores the neighbors of each tile in a dictionary. Can access the neighbors of each tile stored in array, by calling Neighbors(currentTile). Runs in a foreach loop where it runs for all the neighbors there are currently.
            foreach (TilesType neighbor in _customTilesMapGenerator.Neighbors(currentTile))
            {
                // Saves the cost to move to the neighbour.
                int newCost = costToReachTile[currentTile] + neighbor._movementCost;
                // If the new cost is less than the current cost to reach the current tile, and it has not been visited yet.
                if (costToReachTile.ContainsKey(neighbor) == false || newCost < costToReachTile[neighbor])
                {
                    // Ignores the TileType walls in the grid.
                    if (neighbor._TileType != TilesType.TileType.Wall)
                    {
                        // Cost to reach neighboring tile = new cost. Overwrite the previous higher cost that might have been in it.
                        costToReachTile[neighbor] = newCost;
                        // Takes into account the heuristic - Priority = cost + distance (from the neighbor tile and the start tile).
                        int priority = newCost + Distance(neighbor, start);
                        // Enqueue the neighbor against the priority.
                        frontier.Enqueue(neighbor, priority);
                        // If the unit comes to the tile of the neighbor, unit will set it as the current tile and move there.
                        NextTileToGoal[neighbor] = currentTile;
                        // Uses the tile's text field to display the movement cost to move on it.
                        neighbor._Text = costToReachTile[neighbor].ToString();
                    }
                }
            }
        }

        // If statement ensures that null (a path cannot be found) is returned if you can't reach the starting position (due to walls).
        if (NextTileToGoal.ContainsKey(start) == false)
        {
            return null;
        }

        // Generates a path with the queue
        Queue<TilesType> path = new Queue<TilesType>();
        // Tile that the unit is currently standing on.
        TilesType currentPathTile = start;
        // Loop that fills the queue with tiles that the unit needs to move towards.
        while (goal != currentPathTile)
        {
            currentPathTile = NextTileToGoal[currentPathTile];
            path.Enqueue(currentPathTile);
        }

        // Returns path.
        return path;
    }
}
    
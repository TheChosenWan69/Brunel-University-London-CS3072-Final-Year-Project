using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTilesMapGenerator : MonoBehaviour
{
    // Uses the tile prefab that is assigned in the inspector to set up the grid map.
    public GameObject _tile;

    // Set the parameters for how long you want the tiles on the x/y axis to be in the inspector.
    public int lengthX;
    public int lengthY;

    public TilesType[,] grid;

    private Dictionary<TilesType, TilesType[]> neighborDictionary;
    public TilesType[] Neighbors(TilesType tiles)
    {
        return neighborDictionary[tiles];
    }

    private void Awake()
    {
        // Custom grid setup -  converts the static variable of x and y input field text into an integer and make lengthX and lengthY equal to the input field.
        lengthX = int.Parse(MenuGridSize.xField.text);
        lengthY = int.Parse(MenuGridSize.yField.text);

        grid = new TilesType[lengthX, lengthY];
        neighborDictionary = new Dictionary<TilesType, TilesType[]>();
        MapGen(lengthX, lengthY);
    }

    // Generating the map itself based on the integers that has been set - loop spawns in the tiles, with the corresponding rotation type with the parameters set.
    private void MapGen(int lengthX, int lengthY)
    {
        for (int y = 0; y < lengthY; y++)
        {
            for (int x = 0; x < lengthX; x++)
            {
                grid[x, y] = Instantiate(_tile, new Vector3(x, 0, y), Quaternion.identity).GetComponent<TilesType>();
                grid[x, y].Init(x, y);
            }
        }

        // Builds a graph from the map itself that has been generated based on the parameters - loop creates the grid layout, and adds them to an array until the value of lengthX and lengthY is reached.
        for (int y = 0; y < lengthY; y++)
        {
            for (int x = 0; x < lengthX; x++)
            {
                List<TilesType> neighbors = new List<TilesType>();
                if (y < lengthY - 1)
                    neighbors.Add(grid[x, y + 1]);
                if (x < lengthX - 1)
                    neighbors.Add(grid[x + 1, y]);
                if (y > 0)
                    neighbors.Add(grid[x, y - 1]);
                if (x > 0)
                    neighbors.Add(grid[x - 1, y]);

                neighborDictionary.Add(grid[x, y], neighbors.ToArray());
            }
        }
    }
}

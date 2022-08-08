using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TilesType : MonoBehaviour
{
    // Sets up the tile types.
    public enum TileType
    {
        Grass,
        Bush,
        Tree,
        Wall
    }

    public GameObject _Bush;
    public GameObject _Tree;
    public GameObject _Wall;

    private Text _text;
    private TileType _tileType;

    private Renderer _renderer;
    private int _x;
    private int _y;

    private void Awake()
    {
        _text = GetComponentInChildren<Text>();
        _renderer = GetComponent<Renderer>();
    }

    // Sets up tile numbering system when scene is loaded in.
    public void Init(int x, int y)
    {
        _x = (x + 1);
        _y = (y + 1);
        name = "Tile: " + (x + 1) + "," + (y + 1);
    }
    
    // Takes x,y numbers from tile numbering system, and uses them for calculating the Manhatten distance.
    public int _X => _x;
    public int _Y => _y;

    // Adds colour to the block.
    public Color _Color { get => _renderer.material.color; set => _renderer.material.color = value; }
    // Adds text to the block via the canvas added in the Tile prefab.
    public string _Text { get => _text.text; set => _text.text = value; }
    
    // Sets the tile type, corresponding to the key that was pressed. Break stops the loop once it has been set.
    public TileType _TileType
    {
        get => _tileType;
        set
        {
            _tileType = value;
            switch (_tileType)
            {
                case TileType.Grass:
                    _Bush.SetActive(false);
                    _Tree.SetActive(false);
                    _Wall.SetActive(false);
                    break;
                case TileType.Bush:
                    _Bush.SetActive(true);
                    _Tree.SetActive(false);
                    _Wall.SetActive(false);
                    break;
                case TileType.Tree:
                    _Bush.SetActive(false);
                    _Tree.SetActive(true);
                    _Wall.SetActive(false);
                    break;
                case TileType.Wall:
                    _Bush.SetActive(false);
                    _Tree.SetActive(false);
                    _Wall.SetActive(true);
                    break;
            }
        }
    }

    // Sets the movement cost for each tile. Higher value = higher movement cost.
    public int _movementCost
    {
        get
        {
            switch (_tileType)
            {
                case TileType.Grass:
                    return 1;
                case TileType.Bush:
                    return 2;
                case TileType.Tree:
                    return 4;
                default:
                    return 0;
            }
        }
    }
}

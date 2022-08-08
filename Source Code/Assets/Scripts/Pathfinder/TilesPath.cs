using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TilesPath : MonoBehaviour
{
    public PathfindingAlgorithms _pathfindingAlgorithms;
    public TilesMapGenerator _tilesMapGenerator;
    public GameObject _Unit;

    private TilesType _tileStartPoint;
    private TilesType _tileEndPoint;
    private TilesUnit _unit;
    private bool spaceToggle;

    // Setting up timer variables.
    private float currentTime;
    // Displays the timer (in seconds) in the UI.
    public Text currentTimeText; 
    // Shows if the timer is active or not in the inspector.
    [SerializeField] private bool timerIsActive = false;

    // Make PathCalculation catch error and keep running.
    private void TilePathCalculation(TilesType Start, TilesType End)
    {
        Queue<TilesType> path = _pathfindingAlgorithms.FindUnitPath(_tileStartPoint, _tileEndPoint);
        if (path == null)
        {
            Debug.Log("Goal set is not currently not reachable.");
            return;
        }
        else
        {
            foreach (TilesType t in path)
            {
                t._Color = Color.gray;
            }
            _tileStartPoint._Text = "START";
            _tileStartPoint._Color = Color.green;

            _tileEndPoint._Text = "END";
            _tileEndPoint._Color = Color.red;
        }
    }

    // Resetting tile colour if start/end point is changed.
    public void TileReset()
    {
        foreach (TilesType t in _tilesMapGenerator.grid)
        {
            t._Text = "";
            t._Color = Color.white;
        }
    }
    public void TileAssigner()
    {
        TileReset();
        //Colours the tile you click, else it wont appear until you set both the Start and End point.
        if (_tileStartPoint != null)
        {
            _tileStartPoint._Text = "START";
            _tileStartPoint._Color = Color.green;
        }

        if (_tileEndPoint != null)
        {
            _tileEndPoint._Text = "END";
            _tileEndPoint._Color = Color.red;

        }

        if (_tileStartPoint !=null && _tileEndPoint !=null)
        {
            TilePathCalculation(_tileStartPoint, _tileEndPoint);
        }
    }

    // Tile targeting system - targets and selects the tile that the mouse is current hovering over. It will only target tiles that have the "Tile" layer assigned to it, so it will ignore everything else.
    private TilesType TargetTileUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        bool wasHit = Physics.Raycast(ray, out rayHit, int.MaxValue, LayerMask.GetMask("Tile"));
        if (wasHit)
            return rayHit.transform.GetComponent<TilesType>();
        else
            return null;
    }


    // Timer functions.
    public void StartTimer()
    {
        if (timerIsActive == true)
        {
            currentTime = currentTime + Time.deltaTime;
        }
    }

    public void StopTimer()
    {
        if (_unit != null)
        {
            if (_unit.gameObject.transform.position == _tileEndPoint.transform.position)
            {
                timerIsActive = false;
            }
        }
    }

    // Function to allow the start button to spawn the unit in.
    public void StartUnit()
    {
        // Spawns unit if its not there on the map.
        if (_unit == null)
        {
            // Sets timer variable to true, which the StartTimer function will check for through a condition.
            timerIsActive = true;

            _unit = Instantiate(_Unit, _tileStartPoint.transform.position, Quaternion.identity).GetComponent<TilesUnit>();
        }
        else
        {
            // Checks if the timer is above 0 - If it is, it will reset it back to 0 again, and start up the timer again whenever the input key is pressed again to spawn the unit at the starting tile.
            if (currentTime > 0)
            {
                currentTime = 0;
                timerIsActive = true;
            }

            _unit.transform.position = _tileStartPoint.transform.position;
        }
        Queue<TilesType> path = _pathfindingAlgorithms.FindUnitPath(_tileStartPoint, _tileEndPoint);
        _unit.SetUnitPath(path);
    }

    // Function to allow the toggling of block coordinates.
    public void ShowCoordinates()
    {
        spaceToggle = !spaceToggle;

        if (spaceToggle == true)
        {
            foreach (TilesType t in _tilesMapGenerator.grid)
            {
                t._Text = t._X + "," + t._Y;
            }
            Debug.Log("Block coordinates on");
        }
        else
        {
            TileAssigner();
            Debug.Log("Block coordinates off");
        }
    }

    private void Update()
    {
        // Timer checks every frame update if the conditons have changed (to know if it should start or stop).
        StartTimer();
        StopTimer();
        // Convert time that is currently a float, into a string variable. Makes currentTimeText = the float variable.
        currentTimeText.text = currentTime.ToString();

        // Handles the input for the game.

        // Left mouse button - Start
        if (Input.GetMouseButtonDown(0))
        {
            // If the mouseTile is not empty, and it's a wall:
            TilesType mouseTile = TargetTileUnderMouse();
            if (mouseTile != null && mouseTile._TileType == TilesType.TileType.Wall)
            {
                Debug.Log("Can't start on a Wall.");
                return;
            }

            // If mouseTile is empty:
            if (mouseTile !=null)
            {
                _tileStartPoint = mouseTile;
            }
            // Calls TileAssigner function and changes tile.
            TileAssigner();
        }

        // Right mouse button - End
        if (Input.GetMouseButtonDown(1))
        {
            TilesType mouseTile = TargetTileUnderMouse();
            if (mouseTile != null && mouseTile._TileType == TilesType.TileType.Wall)
            {
                Debug.Log("Can't end on a Wall.");
                return;
            }

            // If mouseTile is empty:
            if (mouseTile != null)
            {
                _tileEndPoint = mouseTile;
            }
            // Calls TileAssigner function and changes tile.
            TileAssigner();
        }

        // 1 or Q - Grass
        if (Input.GetKey(KeyCode.Alpha1) | Input.GetKey(KeyCode.Keypad1) | Input.GetKey(KeyCode.Q))
        {
            TilesType mouseTile = TargetTileUnderMouse();
            // If mouseTile is empty:
            if (mouseTile != null)
            {
                mouseTile._TileType = TilesType.TileType.Grass;
                TileAssigner();
            }
        }

        // 2 or W - Bush
        if (Input.GetKey(KeyCode.Alpha2) | Input.GetKey(KeyCode.Keypad2) | Input.GetKey(KeyCode.W))
        {
            TilesType mouseTile = TargetTileUnderMouse();
            // If mouseTile is empty:
            if (mouseTile != null)
            {
                mouseTile._TileType = TilesType.TileType.Bush;
                TileAssigner();
            }
        }

        // 3 or E - Tree
        if (Input.GetKey(KeyCode.Alpha3) | Input.GetKey(KeyCode.Keypad3) | Input.GetKey(KeyCode.E))
        {
            TilesType mouseTile = TargetTileUnderMouse();
            // If mouseTile is empty:
            if (mouseTile != null)
            {
                mouseTile._TileType = TilesType.TileType.Tree;
                TileAssigner();
            }
        }

        // 4 or R - Wall
        if (Input.GetKey(KeyCode.Alpha4) | Input.GetKey(KeyCode.Keypad4) | Input.GetKey(KeyCode.R))
        {
            TilesType mouseTile = TargetTileUnderMouse();
            // If mouseTile is empty:
            if (mouseTile != null)
            {
                mouseTile._TileType = TilesType.TileType.Wall;
                TileAssigner();
            }
        }

        // Enter - Spawns the unit for simulation when triggered.
        if (Input.GetKey(KeyCode.Return) | Input.GetKey(KeyCode.KeypadEnter))
        {
            // Spawns unit if its not there on the map.
            if (_unit == null)
            {
                // Sets timer variable to true, which the StartTimer function will check for through a condition.
                timerIsActive = true;

                _unit = Instantiate(_Unit, _tileStartPoint.transform.position, Quaternion.identity).GetComponent<TilesUnit>();
            }
            else
            {
                // Checks if the timer is above 0 - If it is, it will reset it back to 0 again, and start up the timer again whenever the input key is pressed again to spawn the unit at the starting tile.
                if (currentTime > 0)
                {
                    currentTime = 0;
                    timerIsActive = true;
                }

                _unit.transform.position = _tileStartPoint.transform.position;
            }
            Queue<TilesType> path = _pathfindingAlgorithms.FindUnitPath(_tileStartPoint, _tileEndPoint);
            _unit.SetUnitPath(path);
        }

        // Space - A toggle showing the coordinates of each tile.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spaceToggle = !spaceToggle;

            if (spaceToggle == true)
            {
                foreach (TilesType t in _tilesMapGenerator.grid)
                {
                    t._Text = t._X + "," + t._Y;
                }
                Debug.Log("Block coordinates on");
            }
            else
            {
                TileAssigner();
                Debug.Log("Block coordinates off");
            }
        }
    }
}

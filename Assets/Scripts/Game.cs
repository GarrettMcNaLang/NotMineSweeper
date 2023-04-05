

using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Game : MonoBehaviour
{
    //The size of the board;
    public int width;
    public int height;

    //amount of bombs on the board
    public int bombCount;

    //reference to the board data
    private Board board;
    //reference to the cell
    private Cell[,] state;

    //variable to indicate game over
    private bool GameOver;


    //Sets a limit for the number of bombs
    //OnValidate is a function that will call automatically in the editor any time you update any value
    private void OnValidate()
    {
        bombCount = Mathf.Clamp(bombCount, 0, width * height);
    }
    //Initializes the script
    private void Awake()
    {
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        state = new Cell[width, height];

        GameOver = false;

        GenerateCells();
        GenerateBombs();
        GenerateNumbers();

        

        //offsets the camera to fit board to the screen
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        board.Draw(state);
    }

    private void GenerateCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0);
                cell.type = Cell.Type.Empty;
                state[x, y] = cell;
            }
        }
    }

    //randomly generates the mines
    private void GenerateBombs()
    {
        for (int i = 0; i < bombCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            //checks if the next cell already has a mine
            while (state[x, y].type == Cell.Type.Mine)
            {
                x++;

                if (x >= width)
                {
                    x = 0;
                    y++;

                    if (y >= height)
                    {
                        y = 0;
                    }
                }
            }

            //defines cell as the mine type
            state[x, y].type = Cell.Type.Mine;
            //temporary(reveals tiles to see if this function works
            //state[x,y].revealed = true;
        }
    }
    //generates the numbers that are adjacent to a mine
    private void GenerateNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    continue;
                }

                cell.number = CountMines(x, y);

                if (cell.number > 0)
                {
                    cell.type = Cell.Type.Number;
                }

                //checks if numbers are revealed
                // cell.revealed = true;
                state[x, y] = cell;
            }
        }
    }
    //function that handles the logic of counting how many mines are adjacent to a tile
    private int CountMines(int cellx, int celly)
    {
        //checks if the cell is adjacent to any mines, and if so, increase a count by one

        int count = 0;

        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                //if if it is the current cell continue
                if (adjacentX == 0 && adjacentY == 0)
                {
                    continue;
                }

                int x = cellx + adjacentX;
                int y = celly + adjacentY;

                //checks if the cell is out of bounds


                if (GetCell(x, y).type == Cell.Type.Mine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    //function for the input from the player
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }
        else if(!GameOver)
     {
            if (Input.GetMouseButtonDown(1))
             {
                    Flag();
    
             }
        else if (Input.GetMouseButtonDown(0))
        {


            Reveal();

        }
      }
        
    }
    //places the flag tile on the tile that is selected
    private void Flag()
    {
        //determine which aisle that was clicked on
        //requires mouse position from screen space converted to world space and world space converted to cell space

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        //stops the process if the cell type is invalid
        if(cell.type == Cell.Type.Invalid || cell.revealed)
        {
            return;
        }


        cell.flagged = !cell.flagged;
        state[cellPosition.x, cellPosition.y] = cell;
        board.Draw(state);
    }
    //Reveals what the tile is
    private void Reveal()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.type == Cell.Type.Invalid || cell.revealed || cell.flagged)
        {
            return;
        }
        switch (cell.type)
        {
            case Cell.Type.Mine:
                Explode(cell);
                break;

            case Cell.Type.Empty:
                Flood(cell);
                CheckWinCondition();
                break;
            default:
                cell.revealed = true;
                state[cellPosition.x, cellPosition.y] = cell;
                CheckWinCondition();
                break;
        }
      
        
        board.Draw(state);

    }
    //Will cause an series of empty cells next to one selected cells to be revealed
    //Recursion = a function that calls itself, requires an exit in order to avoid an infinite loop
    //This function will execute this logic
    private void Flood(Cell cell)
    {
        if (cell.revealed) return;
        if (cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid) return;

        cell.revealed = true;
        state[cell.position.x, cell.position.y] = cell;

        if (cell.type == Cell.Type.Empty)
        {
            Flood(GetCell(cell.position.x - 1, cell.position.y));
            Flood(GetCell(cell.position.x + 1, cell.position.y));
            Flood(GetCell(cell.position.x, cell.position.y - 1));
            Flood(GetCell(cell.position.x, cell.position.y + 1));
        }
    }
    private void Explode(Cell cell)
    {
        Debug.Log("Game Over!");
        GameOver = true;

        cell.revealed = true;
        cell.exploded = true;

        state[cell.position.x, cell.position.y] = cell;

        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                cell = state[x, y];

                if(cell.type == Cell.Type.Mine)
                {
                    cell.revealed = true;
                    state[x, y] = cell;
                }
            }
        }
    }

    //Will check if the winner has cleared all tiles that aren't bombs
    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                if (cell.type != Cell.Type.Mine && !cell.revealed)
                {
                    return;
                }
            }
        }
        Debug.Log("You win");
        GameOver = true;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    cell.flagged = true;
                    state[x, y] = cell;
                }

            }
        }
    }
        //retrieves the cell coordinates for the selected cell
        private Cell GetCell(int x, int y) 
        { 
            //checks if coordinates are valid
            if(IsValid(x,y))
            {
                return state[x, y];
            }
            else
            {
            //in case the cell is created if the cell is of the invalid type
                return new Cell();
            }
        }
    //checks if the cell is out of bounds for us
    private bool IsValid(int x, int y)
    {
        return x >= 0 && x < width && y > 0 && y < height;
    }

   
}

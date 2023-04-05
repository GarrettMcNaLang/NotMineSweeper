
using UnityEngine;
using UnityEngine.Tilemaps;
//This script will draw the board while referencing the tilemap itself
public class Board : MonoBehaviour
{
    //Reference to the tilemap
    //public get private set is a way future scripts can access the tilemap without making any changes
    public Tilemap tilemap { get; private set; }

    //Tile references
    public Tile Unknown;
    public Tile Empty;
    public Tile Mine;
    public Tile Exploded;
    public Tile Flagged;
    public Tile N1;
    public Tile N2;
    public Tile N3;
    public Tile N4;
    public Tile N5;
    public Tile N6;
    public Tile N7;
    public Tile N8;
    //initializes on the object when the game runs
    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    //updates the board when the state of the game has changed
    //Cell array state = game state comprising of all of the cells
    public void Draw(Cell[,] state)
    {
        //loop over it and update the tilemap
        int width = state.GetLength(0);
        int height = state.GetLength(1);    

        //for loops that move through the x and y coordinates
        for(int x = 0; x < width; x++)
        {
           for(int y =  0; y < height; y++)
            {
                //establishes the cell at coordinates
                Cell cell = state[x, y];

                //.SetTile(position, tile to render)
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    }
    //handles the logic of selecting a tile
    
    private Tile GetTile(Cell cell)
    {
        if(cell.revealed)
        {
            return GetRevealedTile(cell);
        }
        else if(cell.flagged)
        {
            return Flagged;
        }
        else
        {
            return Unknown;
        }
    }

    //Handles the logic of choosing a tile from the tiles that aren't unknown or flagged
    private Tile GetRevealedTile(Cell cell)
    {
        //chooses between the three cell types
        switch (cell.type)
        {
            case Cell.Type.Empty: return Empty;

            case Cell.Type.Mine: return cell.exploded ? Exploded: Mine;

            case Cell.Type.Number: return GetNumbertile(cell);

            default: return null;

        }
    }
    //Handles the logic of choosing tile 1-8
    private Tile GetNumbertile(Cell cell)
    {
        switch (cell.number)
        {
            case 1: return N1;
            case 2: return N2;
            case 3: return N3;
            case 4: return N4;
            case 5: return N5;
            case 6: return N6;
            case 7: return N7;
            case 8: return N8;
            default: return null;
        }
    }
    
}

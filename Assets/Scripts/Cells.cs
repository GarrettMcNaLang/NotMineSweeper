
using UnityEngine;
//This script holds the data present in an individual cell
public struct Cell
{
    //The possible states of a cell
    public enum Type
    {
        //invalid is a placeholder for the possibility that neither of the other three choices are chosen
        Invalid,
        Empty,
        Mine,
        Number,
    }

    //position of cell within the board
    //has an x,y, and z coordinates represented by integers rather than floats
    //THis is useful when working with tilemaps
    public Vector3Int position;

    public Type type;

    //If it is a number, what number is it
    public int number;

    //cell state based on user action(clicked or unclicked)
    public bool revealed;
    public bool flagged;
    public bool exploded;
}

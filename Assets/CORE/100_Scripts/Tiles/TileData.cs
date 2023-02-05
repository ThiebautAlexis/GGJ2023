using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GGJ2023
{
    [Flags]
    public enum TileShape
    {
        TopLeft = 1 << 1, 
        Top = 1 << 2,
        TopRight = 1 << 3,
        Left = 1 << 4,
        Center = 1 << 5,
        Right = 1 << 6,
        BottomLeft = 1 << 7,
        Bottom = 1 << 8,
        BottomRight = 1 << 9
    }

    [Serializable]
    public class CellData
    {
        public CellState TopLeftState = CellState.Empty; 
        public CellState TopState = CellState.Empty; 
        public CellState TopRightState = CellState.Empty; 
        public CellState LeftState = CellState.Empty; 
        public CellState CenterState = CellState.Empty; 
        public CellState RightState = CellState.Empty; 
        public CellState BottomLeftState = CellState.Empty; 
        public CellState BottomState = CellState.Empty; 
        public CellState BottomRightState = CellState.Empty; 
    }


    [CreateAssetMenu(fileName = "Tile Data", menuName = "GGJ2023/Data/Tiles", order = 101)]
    public class TileData : ScriptableObject
    {
        public TileShape Shape = TileShape.Center;
        public CellData Data = new CellData();
        public Tile Tile = null;
        public TileData RotatedTile = null; 
    }

}

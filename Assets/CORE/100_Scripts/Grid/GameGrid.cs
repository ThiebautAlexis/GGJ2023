using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GGJ2023
{
    #region Enums
    public enum CellState
    {
        Empty = 0,
        Root = 1,
        Burrow = 2
    }
    #endregion

    public static class GameGrid
    {
        #region Fields and properties
        private static CellState[,] cells;
        //private static int[,] multipliers; 
        public static CellState[,] Cells => cells; 
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        public static void InitGrid(CellState[,] _baseCells)
        {
            cells = _baseCells;
            //multipliers= new int[cells.GetLength(0), cells.GetLength(1)];
        }

        public static bool TryFillPosition(int _xCenterIndex, int _yCenterIndex, TileData _tileData, out bool _displayTile)
        {
            if (_xCenterIndex < 0 || _xCenterIndex >= cells.GetLength(0) || _yCenterIndex < 0 || _yCenterIndex >= cells.GetLength(1))
            {
                _displayTile = false; 
                return false; 
            }
            _displayTile = true; 

            // Center 
            if (_tileData.Shape.HasFlag(TileShape.Center) &&
                cells[_xCenterIndex, _yCenterIndex] != CellState.Empty)
                return false;

            // Top
            if ((_tileData.Shape.HasFlag(TileShape.TopLeft) || _tileData.Shape.HasFlag(TileShape.Top) || _tileData.Shape.HasFlag(TileShape.TopRight)) &&
                (_yCenterIndex - 1 < 0 || (_yCenterIndex -1 >= 0 && cells[_xCenterIndex, _yCenterIndex - 1] != CellState.Empty)))
                return false; 

            // Bottom
            if ((_tileData.Shape.HasFlag(TileShape.BottomLeft) || _tileData.Shape.HasFlag(TileShape.Bottom) || _tileData.Shape.HasFlag(TileShape.BottomRight)) &&
               (_yCenterIndex + 1 >= cells.GetLength(1) || cells[_xCenterIndex, _yCenterIndex + 1] != CellState.Empty))
                return false;

            // Left 
            if ((_tileData.Shape.HasFlag(TileShape.BottomLeft) || _tileData.Shape.HasFlag(TileShape.Left) || _tileData.Shape.HasFlag(TileShape.TopLeft)) &&
                (_xCenterIndex - 1 < 0 || cells[_xCenterIndex - 1, _yCenterIndex] != CellState.Empty))
                return false;

            // Right 
            if ((_tileData.Shape.HasFlag(TileShape.BottomRight) || _tileData.Shape.HasFlag(TileShape.Right) || _tileData.Shape.HasFlag(TileShape.TopRight)) &&
                (_xCenterIndex +1  >= cells.GetLength(0) || cells[_xCenterIndex + 1, _yCenterIndex] != CellState.Empty))
                return false;

            // Top Left
            if (_tileData.Shape.HasFlag(TileShape.TopLeft) &&
                _yCenterIndex - 1 >= 0 && _xCenterIndex - 1 >= 0 && cells[_xCenterIndex - 1, _yCenterIndex - 1] != CellState.Empty)
                return false;

            // Bottom Left
            if (_tileData.Shape.HasFlag(TileShape.BottomLeft) &&
                _yCenterIndex + 1 <= cells.GetLength(1) && _xCenterIndex - 1 >= 0 && cells[_xCenterIndex - 1, _yCenterIndex + 1] != CellState.Empty)
                return false;

            // Top Right
            if (_tileData.Shape.HasFlag(TileShape.TopRight) &&
               _yCenterIndex - 1 >= 0 && _xCenterIndex + 1 <= cells.GetLength(0) && cells[_xCenterIndex + 1, _yCenterIndex - 1] != CellState.Empty)
                return false;

            // Bottom Right
            if (_tileData.Shape.HasFlag(TileShape.BottomRight) &&
               _yCenterIndex + 1 <= cells.GetLength(1) && _xCenterIndex + 1 <= cells.GetLength(0) && cells[_xCenterIndex + 1, _yCenterIndex + 1] != CellState.Empty)
                return false;

            if ((_tileData.Shape.HasFlag(TileShape.TopLeft) && CheckNeighbourTiles(_xCenterIndex - 1, _yCenterIndex - 1, _tileData.Data.TopLeftState)) ||
                (_tileData.Shape.HasFlag(TileShape.Top) && CheckNeighbourTiles(_xCenterIndex, _yCenterIndex - 1, _tileData.Data.TopState)) ||
                (_tileData.Shape.HasFlag(TileShape.TopRight) && CheckNeighbourTiles(_xCenterIndex + 1, _yCenterIndex - 1, _tileData.Data.TopRightState)) ||
                (_tileData.Shape.HasFlag(TileShape.Left) && CheckNeighbourTiles(_xCenterIndex - 1, _yCenterIndex, _tileData.Data.LeftState)) ||
                (_tileData.Shape.HasFlag(TileShape.Center) && CheckNeighbourTiles(_xCenterIndex, _yCenterIndex, _tileData.Data.CenterState)) ||
                (_tileData.Shape.HasFlag(TileShape.Right) && CheckNeighbourTiles(_xCenterIndex + 1, _yCenterIndex, _tileData.Data.RightState)) ||
                (_tileData.Shape.HasFlag(TileShape.BottomLeft) && CheckNeighbourTiles(_xCenterIndex - 1, _yCenterIndex + 1, _tileData.Data.BottomLeftState)) ||
                (_tileData.Shape.HasFlag(TileShape.Bottom) && CheckNeighbourTiles(_xCenterIndex, _yCenterIndex + 1, _tileData.Data.BottomState)) ||
                (_tileData.Shape.HasFlag(TileShape.BottomRight) && CheckNeighbourTiles(_xCenterIndex + 1, _yCenterIndex + 1, _tileData.Data.BottomRightState)) )
                return true;

            return false;
        }

        private static bool CheckNeighbourTiles(int _x, int _y, CellState _validState)
        {
            return ((_x - 1 >= 0 && _y < cells.GetLength(1) && (cells[_x - 1, _y] == CellState.Root || cells[_x - 1, _y] == _validState)) ||
                    (_y - 1 >= 0 && (cells[_x, _y - 1] == CellState.Root || cells[_x, _y - 1] == _validState)) ||
                    (_x + 1 < cells.GetLength(0) && (cells[_x + 1, _y] == CellState.Root || cells[_x+1,_y] == _validState)) ||
                    (_y + 1 < cells.GetLength(1) && (cells[_x, _y + 1] == CellState.Root || cells[_x,_y+1] == _validState)) );
        }

        public static void FillPosition(int _x, int _y, TileData _data)
        {
            if (_data.Shape.HasFlag(TileShape.TopLeft))
                cells[_x - 1, _y - 1] = _data.Data.TopLeftState;
            if (_data.Shape.HasFlag(TileShape.Top))
                cells[_x, _y - 1] = _data.Data.TopState;
            if (_data.Shape.HasFlag(TileShape.TopRight))
                cells[_x + 1, _y - 1] = _data.Data.TopRightState;
            if (_data.Shape.HasFlag(TileShape.Left))
                cells[_x - 1, _y] = _data.Data.LeftState;
            if (_data.Shape.HasFlag(TileShape.Center))
                cells[_x, _y] = _data.Data.CenterState;
            if (_data.Shape.HasFlag(TileShape.Right))
                cells[_x + 1, _y] = _data.Data.RightState;
            if (_data.Shape.HasFlag(TileShape.BottomLeft))
                cells[_x - 1, _y + 1] = _data.Data.BottomLeftState;
            if (_data.Shape.HasFlag(TileShape.Bottom))
                cells[_x, _y + 1] = _data.Data.BottomState;
            if (_data.Shape.HasFlag(TileShape.BottomRight))
                cells[_x + 1, _y + 1] = _data.Data.TopLeftState;
            /*
            for (int i = 0; i < cells.GetLength(1); i++)
            {
                for (int j = 0; j < cells.GetLength(0); j++)
                {
                    Debug.Log(cells[j, i]); 
                }
                Debug.Log("---"); 
            }
            */
        }
        #endregion

    }
}

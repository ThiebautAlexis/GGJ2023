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
        public static CellState[,] Cells => cells; 
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        public static void InitGrid(CellState[,] _baseCells)
        {
            cells = _baseCells; 
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
                (_yCenterIndex - 1 < 0) || (_yCenterIndex -1 >= 0 && cells[_xCenterIndex, _yCenterIndex - 1] != CellState.Empty))
            {
                return false; 
            } 

            // Bottom
            if ((_tileData.Shape.HasFlag(TileShape.BottomLeft) || _tileData.Shape.HasFlag(TileShape.Bottom) || _tileData.Shape.HasFlag(TileShape.BottomRight)) &&
               (_yCenterIndex + 1 >= cells.GetLength(1)) || cells[_xCenterIndex, _yCenterIndex + 1] != CellState.Empty)
                return false;

            // Left 
            if ((_tileData.Shape.HasFlag(TileShape.BottomLeft) || _tileData.Shape.HasFlag(TileShape.Left) || _tileData.Shape.HasFlag(TileShape.TopLeft)) &&
                (_xCenterIndex - 1 < 0) || cells[_xCenterIndex - 1, _yCenterIndex] != CellState.Empty)
                return false;

            // Right 
            if ((_tileData.Shape.HasFlag(TileShape.BottomRight) || _tileData.Shape.HasFlag(TileShape.Right) || _tileData.Shape.HasFlag(TileShape.TopRight)) &&
                (_xCenterIndex +1  >= cells.GetLength(0)) || cells[_xCenterIndex + 1, _yCenterIndex] != CellState.Empty)
                return false;

            // Top Left
            if ((_tileData.Shape.HasFlag(TileShape.TopLeft)) &&
                 (_yCenterIndex - 1 < 0) && (_xCenterIndex - 1 < 0) && cells[_xCenterIndex - 1, _yCenterIndex - 1] != CellState.Empty)
                return false;

            // Bottom Left
            if ((_tileData.Shape.HasFlag(TileShape.BottomLeft)) &&
                  (_yCenterIndex + 1 >= cells.GetLength(1)) && (_xCenterIndex - 1 < 0) && cells[_xCenterIndex - 1, _yCenterIndex + 1] != CellState.Empty)
                return false;

            // Top Right
            if ((_tileData.Shape.HasFlag(TileShape.BottomLeft)) &&
               (_yCenterIndex - 1 < 0) && (_xCenterIndex + 1 >= cells.GetLength(0)) && cells[_xCenterIndex + 1, _yCenterIndex - 1] != CellState.Empty)
                return false;

            // Bottom Right
            if ((_tileData.Shape.HasFlag(TileShape.BottomLeft)) &&
               (_yCenterIndex + 1 >= cells.GetLength(1)) && (_xCenterIndex + 1 >= cells.GetLength(0)) && cells[_xCenterIndex + 1, _yCenterIndex + 1] != CellState.Empty)
                return false;

            if ((_tileData.Shape.HasFlag(TileShape.TopLeft) && CheckNeighbourTiles(_xCenterIndex - 1, _yCenterIndex - 1)) ||
                (_tileData.Shape.HasFlag(TileShape.Top) && CheckNeighbourTiles(_xCenterIndex, _yCenterIndex - 1)) ||
                (_tileData.Shape.HasFlag(TileShape.TopRight) && CheckNeighbourTiles(_xCenterIndex + 1, _yCenterIndex - 1)) ||
                (_tileData.Shape.HasFlag(TileShape.Left) && CheckNeighbourTiles(_xCenterIndex - 1, _yCenterIndex)) ||
                (_tileData.Shape.HasFlag(TileShape.Center) && CheckNeighbourTiles(_xCenterIndex, _yCenterIndex)) ||
                (_tileData.Shape.HasFlag(TileShape.Right) && CheckNeighbourTiles(_xCenterIndex + 1, _yCenterIndex)) ||
                (_tileData.Shape.HasFlag(TileShape.BottomLeft) && CheckNeighbourTiles(_xCenterIndex - 1, _yCenterIndex + 1)) ||
                (_tileData.Shape.HasFlag(TileShape.Bottom) && CheckNeighbourTiles(_xCenterIndex, _yCenterIndex + 1)) ||
                (_tileData.Shape.HasFlag(TileShape.BottomRight) && CheckNeighbourTiles(_xCenterIndex + 1, _yCenterIndex + 1)) )
                return true;

            return false;
        }

        private static bool CheckNeighbourTiles(int _x, int _y)
        {
            return ((_x - 1 >= 0 && _y < cells.GetLength(1) && cells[_x - 1, _y] != CellState.Empty) ||
                    (_y - 1 >= 0 && cells[_x, _y - 1] != CellState.Empty) ||
                    (_x + 1 < cells.GetLength(0) && cells[_x + 1, _y] != CellState.Empty) ||
                    (_y + 1 < cells.GetLength(1) && cells[_x, _y + 1] != CellState.Empty)) ;
        }
        #endregion

    }
}

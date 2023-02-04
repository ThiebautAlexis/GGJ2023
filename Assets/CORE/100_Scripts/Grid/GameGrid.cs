using System;
using System.Collections;
using System.Collections.Generic;
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

        public static bool TryFillPosition(int _xCenterIndex, int _yCenterIndex, TileShape _tileShape)
        {
            // Center 
            if(_tileShape.HasFlag(TileShape.Center) &&
                (cells[_xCenterIndex, _yCenterIndex] != CellState.Empty) ||
                _xCenterIndex < 0 || _xCenterIndex >= Cells.GetLength(0) || 
                _yCenterIndex < 0 || _yCenterIndex >= Cells.GetLength(1))
                return false;

            // Top
            if ((_tileShape.HasFlag(TileShape.TopLeft) || _tileShape.HasFlag(TileShape.Top) || _tileShape.HasFlag(TileShape.TopRight)) && 
                (_yCenterIndex >= Cells.GetLength(1) - 1) || cells[_xCenterIndex, _yCenterIndex + 1] != CellState.Empty)
            {
                return false; 
            }

            // Bottom
            if ((_tileShape.HasFlag(TileShape.BottomLeft) || _tileShape.HasFlag(TileShape.Bottom) || _tileShape.HasFlag(TileShape.BottomRight)) &&
                (_yCenterIndex <= 0) || cells[_xCenterIndex, _yCenterIndex - 1] != CellState.Empty)
            {
                return false;
            }

            // Left 
            if ((_tileShape.HasFlag(TileShape.BottomLeft) || _tileShape.HasFlag(TileShape.Left) || _tileShape.HasFlag(TileShape.TopLeft)) &&
                (_xCenterIndex <= 0) || cells[_xCenterIndex - 1, _yCenterIndex] != CellState.Empty)
            {
                return false;
            }

            // Right 
            if ((_tileShape.HasFlag(TileShape.BottomRight) || _tileShape.HasFlag(TileShape.Right) || _tileShape.HasFlag(TileShape.TopRight)) &&
                (_xCenterIndex >= Cells.GetLength(0) - 1) || cells[_xCenterIndex + 1, _yCenterIndex] != CellState.Empty)
            {
                return false;
            }

            // Top Left 
            if (_tileShape.HasFlag(TileShape.TopLeft) && cells[_xCenterIndex - 1, _yCenterIndex + 1] != CellState.Empty)
                return false;
            // Top Right
            if (_tileShape.HasFlag(TileShape.TopRight) && cells[_xCenterIndex + 1, _yCenterIndex + 1] != CellState.Empty)
                return false;
            // Bottom Left 
            if (_tileShape.HasFlag(TileShape.BottomLeft) && cells[_xCenterIndex - 1, _yCenterIndex - 1] != CellState.Empty)
                return false;
            // Bottom Right 
            if (_tileShape.HasFlag(TileShape.BottomRight) && cells[_xCenterIndex + 1, _yCenterIndex - 1] != CellState.Empty)
                return false;

            return true;
        }
        #endregion

    }
}

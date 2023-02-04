using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2023
{
    #region Enums
    public enum CellState
    {
        Empty   = 0, 
        Root    = 1, 
        Burrow  = 2
    }

    [Flags]
    public enum CellShape
    {
        TopLeft     = 1 << 1,  
        Top         = 1 << 2, 
        TopRight    = 1 << 3, 
        Left        = 1 << 4, 
        Center      = 1 << 5, 
        Right       = 1 << 6, 
        BottomLeft  = 1 << 7, 
        Bottom      = 1 << 8,
        BottomRight = 1 << 9
    }
    #endregion

    public static class GameGrid
    {
        #region Fields and properties
        private static CellState[,] cells;
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        public static void InitGrid(CellState[,] _baseCells)
        {
            cells = _baseCells; 
        }

        public static bool TryFillPosition(int _xCenterIndex, int _yCenterIndex, int _tileShape)
        {

            return false;
        }
        #endregion

    }
}

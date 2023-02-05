using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGJ2023
{
    #region Enums
    public enum CellState
    {
        Empty = 0,
        Root,
        Rabbits, 
        Badgers, 
        Foxes
    }
    #endregion

    public static class GameGrid
    {
        #region Fields and properties
        private static CellState[,] cells;
        private static int[,] multipliers; 
        public static CellState[,] Cells => cells; 
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        public static void InitGrid(CellState[,] _baseCells)
        {
            cells = _baseCells;
            multipliers= new int[cells.GetLength(0), cells.GetLength(1)];
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
            if (_tileData.Shape.HasFlag(TileShape.Top) && (_yCenterIndex - 1 < 0 || cells[_xCenterIndex, _yCenterIndex - 1] != CellState.Empty))
                return false; 

            // Bottom
            if (_tileData.Shape.HasFlag(TileShape.Bottom) && (_yCenterIndex + 1 >= cells.GetLength(1) || cells[_xCenterIndex, _yCenterIndex + 1] != CellState.Empty))
                return false;

            // Left 
            if ( _tileData.Shape.HasFlag(TileShape.Left) && (_xCenterIndex - 1 < 0 || cells[_xCenterIndex - 1, _yCenterIndex] != CellState.Empty))
                return false;

            // Right 
            if (_tileData.Shape.HasFlag(TileShape.Right) && (_xCenterIndex +1  >= cells.GetLength(0) || cells[_xCenterIndex + 1, _yCenterIndex] != CellState.Empty))
                return false;

            // Top Left
            if (_tileData.Shape.HasFlag(TileShape.TopLeft) && (_yCenterIndex - 1 < 0 || cells[_xCenterIndex - 1, _yCenterIndex - 1] != CellState.Empty))
                return false;

            // Bottom Left
            if (_tileData.Shape.HasFlag(TileShape.BottomLeft) && _yCenterIndex + 1 <= cells.GetLength(1) && _xCenterIndex - 1 >= 0 && cells[_xCenterIndex - 1, _yCenterIndex + 1] != CellState.Empty)
                return false;

            // Top Right
            if (_tileData.Shape.HasFlag(TileShape.TopRight) && (_yCenterIndex - 1 < 0 || (_xCenterIndex + 1 <= cells.GetLength(0) && cells[_xCenterIndex + 1, _yCenterIndex - 1] != CellState.Empty)))
                return false;

            // Bottom Right
            if (_tileData.Shape.HasFlag(TileShape.BottomRight) && _yCenterIndex + 1 <= cells.GetLength(1) && _xCenterIndex + 1 <= cells.GetLength(0) && cells[_xCenterIndex + 1, _yCenterIndex + 1] != CellState.Empty)
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
            {
                cells[_x - 1, _y - 1] = _data.Data.TopLeftState;
                multipliers[_x - 1, _y - 1] = 1;
            }
            if (_data.Shape.HasFlag(TileShape.Top))
            {
                cells[_x, _y - 1] = _data.Data.TopState;
                multipliers[_x, _y - 1] = 1;
            }
            if (_data.Shape.HasFlag(TileShape.TopRight))
            {
                cells[_x + 1, _y - 1] = _data.Data.TopRightState;
                multipliers[_x + 1, _y - 1] = 1;
            }
            if (_data.Shape.HasFlag(TileShape.Left))
            {
                cells[_x - 1, _y] = _data.Data.LeftState;
                multipliers[_x - 1, _y] = 1;
            }
            if (_data.Shape.HasFlag(TileShape.Center))
            {
                cells[_x, _y] = _data.Data.CenterState;
                multipliers[_x, _y] = 1;
            }
            if (_data.Shape.HasFlag(TileShape.Right))
            {
                cells[_x + 1, _y] = _data.Data.RightState;
                multipliers[_x + 1, _y] = 1;
            }
            if (_data.Shape.HasFlag(TileShape.BottomLeft))
            {
                cells[_x - 1, _y + 1] = _data.Data.BottomLeftState;
                multipliers[_x - 1, _y + 1] = 1;
            }
            if (_data.Shape.HasFlag(TileShape.Bottom))
            {
                cells[_x, _y + 1] = _data.Data.BottomState;
                multipliers[_x, _y + 1] = 1;
            }
            if (_data.Shape.HasFlag(TileShape.BottomRight))
            {
                cells[_x + 1, _y + 1] = _data.Data.TopLeftState;
                multipliers[_x + 1, _y + 1] = 1;
            }

            UpdateScore(_data, _x, _y, CellState.Rabbits);
            UpdateScore(_data, _x, _y, CellState.Badgers);
            UpdateScore(_data, _x, _y, CellState.Foxes);
        }

        private static void UpdateScore(TileData _data, int _x, int _y, CellState _state)
        {
            List<CellScoreData> frontier = new List<CellScoreData>();
            if (_data.Shape.HasFlag(TileShape.TopLeft) && _data.Data.TopLeftState == _state)
            {
                CellScoreData _scoreData = new CellScoreData() { Score = multipliers[_x - 1, _y - 1], X = _x - 1, Y = _y - 1 };
                frontier.Add(_scoreData); 
            }
            if (_data.Shape.HasFlag(TileShape.Top) && _data.Data.TopState == _state)
            {
                CellScoreData _scoreData = new CellScoreData() { Score = multipliers[_x, _y - 1], X = _x, Y = _y - 1 };
                frontier.Add(_scoreData);
            }
            if (_data.Shape.HasFlag(TileShape.TopRight) && _data.Data.TopRightState == _state)
            {
                CellScoreData _scoreData = new CellScoreData() { Score = multipliers[_x + 1, _y - 1], X = _x + 1, Y = _y - 1 };
                frontier.Add(_scoreData);
            }
            if (_data.Shape.HasFlag(TileShape.Left) && _data.Data.LeftState == _state)
            {
                CellScoreData _scoreData = new CellScoreData() { Score = multipliers[_x - 1, _y], X = _x -1, Y = _y };
                frontier.Add(_scoreData);
            }
            if (_data.Shape.HasFlag(TileShape.Center) && _data.Data.CenterState == _state)
            {
                CellScoreData _scoreData = new CellScoreData() { Score = multipliers[_x, _y], X = _x, Y = _y };
                frontier.Add(_scoreData);
            }
            if (_data.Shape.HasFlag(TileShape.Right) && _data.Data.RightState == _state)
            {
                CellScoreData _scoreData = new CellScoreData() { Score = multipliers[_x + 1, _y], X = _x + 1, Y = _y };
                frontier.Add(_scoreData);
            }
            if (_data.Shape.HasFlag(TileShape.BottomLeft) && _data.Data.BottomLeftState == _state)
            {
                CellScoreData _scoreData = new CellScoreData() { Score = multipliers[_x - 1, _y + 1], X = _x - 1, Y = _y + 1 };
                frontier.Add(_scoreData);
            }
            if (_data.Shape.HasFlag(TileShape.Bottom) && _data.Data.BottomState == _state)
            {
                CellScoreData _scoreData = new CellScoreData() { Score = multipliers[_x, _y + 1], X = _x, Y = _y + 1 };
                frontier.Add(_scoreData);
            }
            if (_data.Shape.HasFlag(TileShape.BottomRight) && _data.Data.BottomRightState == _state)
            {
                CellScoreData _scoreData = new CellScoreData() { Score = multipliers[_x + 1, _y + 1], X = _x + 1, Y = _y + 1 };
                frontier.Add(_scoreData);
            }

            List<CellScoreData> _cluster = new List<CellScoreData>(); 
            while (frontier.Count > 0)
            {
                CellScoreData _scoreData = frontier[0];
                CellScoreData _testedData; 
                // top 
                int _index = _scoreData.Y - 1; 
                if(_index >= 0 && cells[_scoreData.X, _index] == _state)
                {
                    _testedData = new CellScoreData()
                    {
                        Score = multipliers[_scoreData.X, _index],
                        X = _scoreData.X,
                        Y = _index
                    };
                    if (!frontier.Contains(_testedData) && !_cluster.Contains(_testedData))
                        frontier.Add(_testedData);
                }                
                // bottom 
                _index = _scoreData.Y + 1;
                if (_index < cells.GetLength(1) && cells[_scoreData.X, _index] == _state)
                {
                    _testedData = new CellScoreData()
                    {
                        Score = multipliers[_scoreData.X, _index],
                        X = _scoreData.X,
                        Y = _index
                    };
                    if (!frontier.Contains(_testedData) && !_cluster.Contains(_testedData))
                        frontier.Add(_testedData);
                }

                // left 
                _index = _scoreData.X - 1;
                if (_index >= 0 && cells[_index, _scoreData.Y] == _state)
                {
                    _testedData = new CellScoreData()
                    {
                        Score = multipliers[_index, _scoreData.Y],
                        X = _index,
                        Y = _scoreData.Y
                    };
                    if (!frontier.Contains(_testedData) && !_cluster.Contains(_testedData))
                        frontier.Add(_testedData);
                }
                // right
                _index = _scoreData.X + 1;
                if (_index < cells.GetLength(0) && cells[_index, _scoreData.Y] == _state)
                {
                    _testedData = new CellScoreData()
                    {
                        Score = multipliers[_index, _scoreData.Y],
                        X = _index,
                        Y = _scoreData.Y
                    };
                    if (!frontier.Contains(_testedData) && !_cluster.Contains(_testedData))
                        frontier.Add(_testedData);
                }

                _cluster.Add(frontier[0]);
                frontier.RemoveAt(0); 
            }
            if(_cluster.Count > 0) 
            {
                int _max = _cluster.Max(c => c.Score) + 1; 
                foreach (CellScoreData _score in _cluster)
                {
                    multipliers[_score.X, _score.Y] = _max;
                }
            }
        }

        internal static bool CanPlaceNextTile(TileData currentTile)
        {
            bool _display; 
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                for (int x = 0; x < cells.GetLength(0); x++)
                {
                    if (TryFillPosition(x, y, currentTile, out _display))
                        return true; 
                }
            }

            return false; 
        }


        public static int GetScore()
        {
            int _score = 0;
            for (int _y = 0; _y < cells.GetLength(1); _y++)
            {
                for (int _x = 0; _x < cells.GetLength(0); _x++)
                {
                    Debug.Log(multipliers[_x, _y]);
                    _score += multipliers[_x, _y]; 
                }
            }
            return _score; 
        }
        #endregion

        private struct CellScoreData
        {
            public int X, Y, Score;

            public override bool Equals(object obj)
            {
                if(obj == null) return false;
                if (obj is CellScoreData _data)
                    return _data.X == X && _data.Y == Y;
                return
                    false; 
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }

    
}

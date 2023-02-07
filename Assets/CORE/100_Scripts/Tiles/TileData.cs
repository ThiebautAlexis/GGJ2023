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
        [SerializeField] TileShape shape = TileShape.Center;
        [SerializeField] CellData data = new CellData();
        public Tile Tile = null;

        [SerializeField] private ParticleSystem VFX;

        public TileShape GetShape(int _rotation)
        {
            TileShape _shape = 0;
            TileShape _tempShape = 0; 
            if(shape.HasFlag(TileShape.TopLeft))
            {
                _tempShape = TileShape.TopLeft;
                if (_rotation == 90) _tempShape = TileShape.BottomLeft;
                if (_rotation == 180) _tempShape = TileShape.BottomRight;
                if (_rotation == 270) _tempShape = TileShape.TopRight;
                _shape |= _tempShape; 
            }
            if (shape.HasFlag(TileShape.Top))
            {
                _tempShape = TileShape.Top;
                if (_rotation == 90) _tempShape = TileShape.Left;
                if (_rotation == 180) _tempShape = TileShape.Bottom;
                if (_rotation == 270) _tempShape = TileShape.Right;
                _shape |= _tempShape;
            }
            if (shape.HasFlag(TileShape.TopRight))
            {
                _tempShape = TileShape.TopRight;
                if (_rotation == 90) _tempShape = TileShape.TopLeft;
                if (_rotation == 180) _tempShape = TileShape.BottomLeft;
                if (_rotation == 270) _tempShape = TileShape.BottomRight;
                _shape |= _tempShape;
            }
            if (shape.HasFlag(TileShape.Left))
            {
                _tempShape = TileShape.Left;
                if (_rotation == 90) _tempShape = TileShape.Bottom;
                if (_rotation == 180) _tempShape = TileShape.Right;
                if (_rotation == 270) _tempShape = TileShape.Top;
                _shape |= _tempShape;
            }
            if (shape.HasFlag(TileShape.Center))
            {
                _shape |= TileShape.Center;
            }
            if (shape.HasFlag(TileShape.Right))
            {
                _tempShape = TileShape.Right;
                if (_rotation == 90) _tempShape = TileShape.Top;
                if (_rotation == 180) _tempShape = TileShape.Left;
                if (_rotation == 270) _tempShape = TileShape.Bottom;
                _shape |= _tempShape;
            }
            if (shape.HasFlag(TileShape.BottomLeft))
            {
                _tempShape = TileShape.BottomLeft;
                if (_rotation == 90) _tempShape = TileShape.BottomRight;
                if (_rotation == 180) _tempShape = TileShape.TopRight;
                if (_rotation == 270) _tempShape = TileShape.TopLeft;
                _shape |= _tempShape;
            }
            if (shape.HasFlag(TileShape.Bottom))
            {
                _tempShape = TileShape.Bottom;
                if (_rotation == 90) _tempShape = TileShape.Right;
                if (_rotation == 180) _tempShape = TileShape.Top;
                if (_rotation == 270) _tempShape = TileShape.Left;
                _shape |= _tempShape;
            }
            if (shape.HasFlag(TileShape.BottomRight))
            {
                _tempShape = TileShape.BottomRight;
                if (_rotation == 90) _tempShape = TileShape.TopRight;
                if (_rotation == 180) _tempShape = TileShape.BottomLeft;
                if (_rotation == 270) _tempShape = TileShape.TopLeft;
                _shape |= _tempShape;
            }
            return _shape; 
        }

        public CellState GetCellState(TileShape _shape, int _rotation)
        {
            CellState _state = CellState.Empty; 
            switch (_shape)
            {
                case TileShape.TopLeft:
                    if (_rotation == 0) _state = data.TopLeftState; 
                    if (_rotation == 90) _state = data.TopRightState; 
                    if (_rotation == 180) _state = data.BottomLeftState; 
                    if (_rotation == 270) _state = data.BottomRightState; 
                    return _state;
                case TileShape.Top:
                    if (_rotation == 0) _state = data.TopState;
                    if (_rotation == 90)_state = data.RightState;
                    if (_rotation == 180) _state = data.BottomState;
                    if (_rotation == 270) _state = data.BottomState;
                    return _state;
                case TileShape.TopRight:
                    if (_rotation == 0) _state = data.TopRightState;
                    if (_rotation == 90) _state = data.BottomRightState;
                    if (_rotation == 180) _state = data.BottomLeftState;
                    if (_rotation == 270) _state = data.TopLeftState;
                    return _state;
                case TileShape.Left:
                    if (_rotation == 0) _state = data.LeftState;
                    if (_rotation == 90) _state = data.TopState;
                    if (_rotation == 180) _state = data.RightState;
                    if (_rotation == 270) _state = data.BottomState;
                    return _state;
                case TileShape.Center:
                    return data.CenterState; 
                case TileShape.Right:
                    if (_rotation == 0) _state = data.RightState;
                    if (_rotation == 90) _state = data.BottomState;
                    if (_rotation == 180) _state = data.LeftState;
                    if (_rotation == 270) _state = data.TopState;
                    return _state;
                case TileShape.BottomLeft:
                    if (_rotation == 0) _state = data.BottomLeftState;
                    if (_rotation == 90) _state = data.TopLeftState;
                    if (_rotation == 180) _state = data.TopRightState;
                    if (_rotation == 270) _state = data.BottomRightState;
                    return _state;
                case TileShape.Bottom:
                    if (_rotation == 0) _state = data.BottomState;
                    if (_rotation == 90) _state = data.LeftState;
                    if (_rotation == 180) _state = data.TopState;
                    if (_rotation == 270) _state = data.RightState;
                    return _state;
                case TileShape.BottomRight:
                    if (_rotation == 0) _state = data.BottomRightState;
                    if (_rotation == 90) _state = data.BottomLeftState;
                    if (_rotation == 180) _state = data.TopLeftState;
                    if (_rotation == 270) _state = data.TopRightState;
                    return _state;
                default:
                    return CellState.Empty;
            }
        }
    }

}

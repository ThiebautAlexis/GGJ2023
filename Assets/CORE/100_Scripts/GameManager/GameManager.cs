using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GGJ2023
{
    public class GameManager : MonoBehaviour
    {
        #region Fields and Properties
        public static GameManager Instance; 

        [Header("Grid")]
        [SerializeField] private GridData baseGrid;
        [SerializeField] private Grid grid; 
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private TileData currentTile;

        [SerializeField] private Tile rootTile;
        [SerializeField] private TileData[] deck; 

        [Header("Camera")]
        [SerializeField] private new Camera camera;

        [Header("Previsualisation")]
        [SerializeField] private Tilemap previsualisationTilemap;
        [SerializeField] private Color validColor;
        [SerializeField] private Color invalidColor; 
        #endregion

        #region Events 
        public static event Action OnGameStarted;
        public static event Action OnGameStopped;
        #endregion


        #region Private Methods
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(this);
        }

        private void Start()
        {
            InitGame();
            StartGame(); 
        }

        private void InitGame()
        {
            GameGrid.InitGrid(baseGrid.GetConvertedCells());
            for (int y = 0; y < GameGrid.Cells.GetLength(1); y++)
            {
                for (int x = 0; x < GameGrid.Cells.GetLength(0); x++)
                {
                    if (GameGrid.Cells[x, y] == CellState.Empty) continue;
                    tilemap.SetTile(new Vector3Int(x, -y, 0), rootTile);
                }
            }

            UIManager.Instance.SetPrevisualisation(currentTile.Tile.sprite); 
        }

        private void ProceedToNextTile()
        {
            currentTile = deck[UnityEngine.Random.Range(0, deck.Length)]; // Get a new tile here
            if (!GameGrid.CanPlaceNextTile(currentTile))
            {
                StopGame(); 
            } 
            else
                UIManager.Instance.SetPrevisualisation(currentTile.Tile.sprite);
        }

        //private void FillTileFromData(TileData _tileData, int _xPos, int _yPos)
        //{
        //    tilemap.SetTile(new Vector3Int(_xPos, - _yPos, 0), _tileData.Tile); 

        //    /* USE THIS PART OF THE CODE IF THE TILES ARE COMPOSED WITH MULTIPLE TILES TOGETHER 
             

        //    // Top Left
        //    if(_tileData.Shape.HasFlag(TileShape.TopLeft))
        //    {
        //        tilemap.SetTile(new Vector3Int(_xPos - 1, -(_yPos + 1), 0), tileData.Tile);
        //    }
        //    // Top
        //    if (_tileData.Shape.HasFlag(TileShape.Top))
        //    {
        //        tilemap.SetTile(new Vector3Int(_xPos, -(_yPos + 1), 0), tileData.Tile);
        //    }
        //    // Top Right
        //    if (_tileData.Shape.HasFlag(TileShape.TopRight))
        //    {
        //        tilemap.SetTile(new Vector3Int(_xPos + 1, -(_yPos + 1), 0), tileData.Tile);
        //    }
        //    // Left
        //    if (_tileData.Shape.HasFlag(TileShape.Left))
        //    {
        //        tilemap.SetTile(new Vector3Int(_xPos - 1, _yPos, 0), tileData.Tile);
        //    }
        //    // Center
        //    if (_tileData.Shape.HasFlag(TileShape.Center))
        //    {
        //        tilemap.SetTile(new Vector3Int(_xPos , -_yPos, 0), tileData.Tile);
        //    }
        //    // Right 
        //    if (_tileData.Shape.HasFlag(TileShape.Right))
        //    {
        //        tilemap.SetTile(new Vector3Int(_xPos + 1, -_yPos, 0), tileData.Tile);
        //    }
        //    // Bottom Left
        //    if (_tileData.Shape.HasFlag(TileShape.BottomLeft))
        //    {
        //        tilemap.SetTile(new Vector3Int(_xPos - 1, -(_yPos - 1), 0), tileData.Tile);
        //    }
        //    // Bottom
        //    if (_tileData.Shape.HasFlag(TileShape.Bottom))
        //    {
        //        tilemap.SetTile(new Vector3Int(_xPos, -(_yPos - 1), 0), tileData.Tile);
        //    }
        //    // Bottom Right
        //    if (_tileData.Shape.HasFlag(TileShape.BottomRight))
        //    {
        //        tilemap.SetTile(new Vector3Int(_xPos + 1, -(_yPos - 1), 0), tileData.Tile);
        //    }
        //    */
        //}

        private void StartGame()
        {
            OnGameStarted?.Invoke();
        }

        private void StopGame()
        {
            Debug.Log(GameGrid.GetScore()); 
            OnGameStopped?.Invoke();
        }
        #endregion

        #region Public Method
        Vector3Int previousGridPosition = Vector3Int.zero; 
        Vector3Int gridPosition = Vector3Int.zero;
        private bool _isValidTile = false; 
        public void UpdatePrevisualisation(Vector2 _mousePosition)
        {
            gridPosition = grid.WorldToCell(camera.ScreenToWorldPoint(_mousePosition));
            if(gridPosition == previousGridPosition) 
            {
                return;
            }
            previousGridPosition = gridPosition;
            _isValidTile = GameGrid.TryFillPosition(gridPosition.x, -gridPosition.y, currentTile, out bool _displayTile);
            previsualisationTilemap.ClearAllTiles(); 
            if(_displayTile)
            {
                previsualisationTilemap.color = _isValidTile ? validColor : invalidColor;
                previsualisationTilemap.SetTile(gridPosition, currentTile.Tile);
                // SNAP SOUND
                AudioManager.Instance.PlaySFX(AudioManager.Instance.SnapClip); 
            }
        }

        public void PlaceTile()
        {
            if(_isValidTile) 
            {
                Vector3Int _position = gridPosition; 
                GameGrid.FillPosition(_position.x, -_position.y, currentTile);
                AudioManager.Instance.PlaySFX(AudioManager.Instance.TilePoseClip); 
                Sequence _sequence = DOTween.Sequence();
                _sequence.Append(UIManager.Instance.RemovePrevisualisation()); 
                _sequence.AppendCallback(() => OnSequenceValidate(_position)); 
            }

            void OnSequenceValidate(Vector3Int _position)
            {
                tilemap.SetTile(_position, currentTile.Tile);
                previsualisationTilemap.ClearAllTiles();
                ProceedToNextTile(); 
            }
        }

        Sequence rotationSequence = null; 
        public void RotateTile()
        {
            if(rotationSequence == null)
            {
                currentTile = currentTile.RotatedTile; 
                rotationSequence = DOTween.Sequence();
                rotationSequence.Append(UIManager.Instance.RotatePrevisualisation());
                rotationSequence.AppendCallback(ResetRotationSequence); 
            }

            void ResetRotationSequence()
            {
                _isValidTile = GameGrid.TryFillPosition(gridPosition.x, -gridPosition.y, currentTile, out bool _displayTile);
                previsualisationTilemap.ClearAllTiles();
                if (_displayTile)
                {
                    previsualisationTilemap.color = _isValidTile ? validColor : invalidColor;
                    previsualisationTilemap.SetTile(gridPosition, currentTile.Tile);
                }
                rotationSequence.Kill(true);
                rotationSequence = null; 
            }
        }
        #endregion
    }
}

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

        [Header("Camera")]
        [SerializeField] private new Camera camera;

        [Header("Previsualisation")]
        [SerializeField] private Tilemap previsualisationTilemap;
        [SerializeField] private Color validColor;
        [SerializeField] private Color invalidColor; 
        
        [Header("Tests")]
        [SerializeField] private Tile rootTile;
        [SerializeField] private TileData tileData;
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
            InitGameManager();
            StartGame(); 
        }

        private void InitGameManager()
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
        }

        private void FillTileFromData(TileData _tileData, int _xPos, int _yPos)
        {
            //tilemap.SetTile(new Vector3Int(_xPos, - _yPos, 0), _tileData.Tile); 

            /* USE THIS PART OF THE CODE IF THE TILES ARE COMPOSED WITH MULTIPLE TILES TOGETHER 
             */ 

            // Top Left
            if(_tileData.Shape.HasFlag(TileShape.TopLeft))
            {
                tilemap.SetTile(new Vector3Int(_xPos - 1, -(_yPos + 1), 0), tileData.Tile);
            }
            // Top
            if (_tileData.Shape.HasFlag(TileShape.Top))
            {
                tilemap.SetTile(new Vector3Int(_xPos, -(_yPos + 1), 0), tileData.Tile);
            }
            // Top Right
            if (_tileData.Shape.HasFlag(TileShape.TopRight))
            {
                tilemap.SetTile(new Vector3Int(_xPos + 1, -(_yPos + 1), 0), tileData.Tile);
            }
            // Left
            if (_tileData.Shape.HasFlag(TileShape.Left))
            {
                tilemap.SetTile(new Vector3Int(_xPos - 1, _yPos, 0), tileData.Tile);
            }
            // Center
            if (_tileData.Shape.HasFlag(TileShape.Center))
            {
                tilemap.SetTile(new Vector3Int(_xPos , -_yPos, 0), tileData.Tile);
            }
            // Right 
            if (_tileData.Shape.HasFlag(TileShape.Right))
            {
                tilemap.SetTile(new Vector3Int(_xPos + 1, -_yPos, 0), tileData.Tile);
            }
            // Bottom Left
            if (_tileData.Shape.HasFlag(TileShape.BottomLeft))
            {
                tilemap.SetTile(new Vector3Int(_xPos - 1, -(_yPos - 1), 0), tileData.Tile);
            }
            // Bottom
            if (_tileData.Shape.HasFlag(TileShape.Bottom))
            {
                tilemap.SetTile(new Vector3Int(_xPos, -(_yPos - 1), 0), tileData.Tile);
            }
            // Bottom Right
            if (_tileData.Shape.HasFlag(TileShape.BottomRight))
            {
                tilemap.SetTile(new Vector3Int(_xPos + 1, -(_yPos - 1), 0), tileData.Tile);
            }
            
        }

        private void StartGame()
        {
            OnGameStarted?.Invoke();
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
            _isValidTile = GameGrid.TryFillPosition(gridPosition.x, -gridPosition.y, tileData, out bool _displayTile);
            previsualisationTilemap.ClearAllTiles(); 
            if(_displayTile)
            {
                previsualisationTilemap.color = _isValidTile ? validColor : invalidColor;
                previsualisationTilemap.SetTile(gridPosition, tileData.Tile); 
            }
        }
        #endregion
    }
}

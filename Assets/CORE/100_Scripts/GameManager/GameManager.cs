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
        [SerializeField] private TileData currentTileData;
        [SerializeField] private Tile currentTile;

        private int currentRotation = 0; 

        [SerializeField] private Tile rootTile;
        [SerializeField] private TileData[] deck; 

        [Header("Camera")]
        [SerializeField] private new Camera camera;
        [SerializeField] private float topLimit;
        [SerializeField] private float bottomLimit;
        [SerializeField] private float speed;  

        [Header("Previsualisation")]
        [SerializeField] private Tilemap previsualisationTilemap;
        [SerializeField] private Color validColor;
        [SerializeField] private Color invalidColor;
        #endregion

        #region Events 
        public static event Action OnGameReady; 
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
            currentTile = currentTileData.Tile; 
            ResetRotation(); 
        }

        private void ProceedToNextTile()
        {
            currentTileData = deck[UnityEngine.Random.Range(0, deck.Length)]; // Get a new tile here
            currentTile= currentTileData.Tile;
            if (!GameGrid.CanPlaceNextTile(currentTileData, 0) &&
                !GameGrid.CanPlaceNextTile(currentTileData, 90) &&
                !GameGrid.CanPlaceNextTile(currentTileData, 180) &&
                !GameGrid.CanPlaceNextTile(currentTileData, 270))
            {
                StopGame(); 
            } 
            else
                UIManager.Instance.SetPrevisualisation(currentTileData.Tile.sprite);
        }

        private void ResetRotation()
        {
            currentRotation = 0;
            UIManager.Instance.ResetPrevisualisationRotation(); 
        }

        public void StartGame()
        {
            OnGameReady?.Invoke(); 
            _cameraSequence = DOTween.Sequence();
            float _distance = (camera.transform.position.y - topLimit);
            _cameraSequence.Append(camera.transform.DOLocalMoveY(topLimit, _distance / speed));
            _cameraSequence.AppendCallback(OnGameStarting); 

            void OnGameStarting()
            {
                UIManager.Instance.SetPrevisualisation(currentTileData.Tile.sprite);
                OnGameStarted?.Invoke();
            }
        }

        private void StopGame()
        {
            UIManager.Instance.SetScore(GameGrid.GetScore()); 
            OnGameStopped?.Invoke();
        }
        #endregion

        #region Public Method
        Vector3Int previousGridPosition = Vector3Int.zero; 
        Vector3Int gridPosition = Vector3Int.zero;
        private bool _isValidTile = false;
        private Sequence _cameraSequence;
        public void UpdatePrevisualisation(Vector2 _mousePosition)
        {
            if (_placingSequence.IsActive() || rotationSequence.IsActive())
            {
                return; 
            }

            if (_cameraSequence.IsActive())
                _cameraSequence.Kill(false);

            _cameraSequence = DOTween.Sequence(); 
            if (_mousePosition.y <= (Screen.height * .15f))
            {
                // v = d/t => t = v/d
                float _distance = (camera.transform.position.y - bottomLimit); 
                if(_distance > 0)
                    _cameraSequence.Append(camera.transform.DOLocalMoveY(bottomLimit, _distance/speed)); 
            }
            else if(_mousePosition.y >= Screen.height - (Screen.height * .15f ))
            {
                float _distance = (topLimit - camera.transform.position.y);
                if (_distance > 0)
                    _cameraSequence.Append(camera.transform.DOLocalMoveY(topLimit, _distance/speed));
            }
            gridPosition = grid.WorldToCell(camera.ScreenToWorldPoint(_mousePosition));
            if(gridPosition == previousGridPosition) 
            {
                return;
            }
            previousGridPosition = gridPosition;
            _isValidTile = GameGrid.TryFillPosition(gridPosition.x, -gridPosition.y, currentRotation, currentTileData, out bool _displayTile);
            previsualisationTilemap.ClearAllTiles(); 
            if(_displayTile)
            {
                previsualisationTilemap.color = _isValidTile ? validColor : invalidColor;
                previsualisationTilemap.SetTile(gridPosition, currentTile);
                // SNAP SOUND
                AudioManager.Instance.PlaySFX(AudioManager.Instance.SnapClip); 
            }
        }

        private Sequence _placingSequence; 
        public void PlaceTile()
        {
            if(_isValidTile) 
            {
                Vector3Int _position = gridPosition; 
                GameGrid.FillPosition(_position.x, -_position.y, currentTileData, currentRotation);
                AudioManager.Instance.PlaySFX(AudioManager.Instance.TilePoseClip);
                _placingSequence = DOTween.Sequence();
                _placingSequence.Append(UIManager.Instance.RemovePrevisualisation());
                _placingSequence.AppendCallback(() => OnSequenceValidate(_position)); 
            }

            void OnSequenceValidate(Vector3Int _position)
            {
                tilemap.SetTile(_position, currentTileData.Tile);
                previsualisationTilemap.ClearAllTiles();
                var _t = currentTile.transform;
                _t.SetTRS(Vector3.zero, Quaternion.identity, Vector3.one);
                currentTile.transform = _t;
                ResetRotation();

                ProceedToNextTile(); 
            }
        }

        Sequence rotationSequence = null; 
        public void RotateTile()
        {
            if(rotationSequence == null)
            {
                currentRotation += 90;
                if (currentRotation >= 360) currentRotation = 0;

                rotationSequence = DOTween.Sequence();
                rotationSequence.Append(UIManager.Instance.RotatePrevisualisation(currentRotation));
                rotationSequence.AppendCallback(ResetRotationSequence); 
            }

            void ResetRotationSequence()
            {
                _isValidTile = GameGrid.TryFillPosition(gridPosition.x, -gridPosition.y, currentRotation, currentTileData, out bool _displayTile);
                previsualisationTilemap.ClearAllTiles();
                if (_displayTile)
                {
                    var _t = currentTile.transform; 
                    _t.SetTRS(Vector3.zero, Quaternion.Euler(0, 0, currentRotation), Vector3.one);
                    currentTile.transform = _t;

                    previsualisationTilemap.SetTile(gridPosition, currentTile); 
                    previsualisationTilemap.color = _isValidTile ? validColor : invalidColor;
                }
                else

                rotationSequence.Kill(true);
                rotationSequence = null; 
            }
        }
        #endregion
    }
}

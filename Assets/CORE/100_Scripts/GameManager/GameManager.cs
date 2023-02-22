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
        public static event Action OnGameEnded; 
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

        private void LateUpdate()
        {
            if (placingSequence.IsActive() || rotationSequence.IsActive())
                return;

            if (mouseToGridPosition == previousGridPosition)
                return;

            gridPosition = mouseToGridPosition;
            previousGridPosition = gridPosition;
            isValidTile = GameGrid.TryFillPosition(gridPosition.x, -gridPosition.y, currentRotation, currentTileData, out bool _displayTile);
            previsualisationTilemap.ClearAllTiles();
            if (_displayTile)
            {
                previsualisationTilemap.color = isValidTile ? validColor : invalidColor;
                previsualisationTilemap.SetTile(gridPosition, currentTile);
                // SNAP SOUND
                AudioManager.Instance.PlaySFX(AudioManager.Instance.SnapClip);
            }
        }

        private void InitGame()
        {
            GameGrid.InitGrid(baseGrid.GetConvertedCells());
#if UNITY_EDITOR
            for (int y = 0; y < GameGrid.Cells.GetLength(1); y++)
            {
                for (int x = 0; x < GameGrid.Cells.GetLength(0); x++)
                {
                    if (y == 0 || x == 0 || y == GameGrid.Cells.GetLength(1) - 1 || x == GameGrid.Cells.GetLength(0) - 1) 
                        tilemap.SetTile(new Vector3Int(x, -y, 0), rootTile);

                    if (GameGrid.Cells[x, y] == CellState.Empty) continue;
                    tilemap.SetTile(new Vector3Int(x, -y, 0), rootTile);
                }
            }
#endif
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

            placingSequence.Kill(true);
            placingSequence = null; 
        }

        private void ResetRotation()
        {
            var _t = currentTile.transform;
            _t.SetTRS(Vector3.zero, Quaternion.identity, Vector3.one);
            currentTile.transform = _t;
            currentRotation = 0;
            UIManager.Instance.ResetPrevisualisationRotation(); 
        }

        public void StartGame()
        {
            OnGameReady?.Invoke(); 
            cameraSequence = DOTween.Sequence();
            float _distance = (camera.transform.position.y - topLimit);
            cameraSequence.Append(camera.transform.DOLocalMoveY(topLimit, _distance / speed));
            cameraSequence.AppendCallback(OnGameStarting); 

            void OnGameStarting()
            {
                UIManager.Instance.SetPrevisualisation(currentTileData.Tile.sprite);
                OnGameStarted?.Invoke();
            }
        }

        private void StopGame()
        {
            OnGameStopped?.Invoke();
            UIManager.Instance.SetScore(GameGrid.GetScore());
            if (cameraSequence.IsActive())
                cameraSequence.Kill(false);
            cameraSequence = DOTween.Sequence();
            float _distance = Mathf.Abs(bottomLimit - camera.transform.position.y);
            cameraSequence.Append(camera.transform.DOLocalMoveY(bottomLimit, _distance / (speed * 2))); 
            _distance = Mathf.Abs(bottomLimit - topLimit);
            cameraSequence.Append(camera.transform.DOLocalMoveY(topLimit, _distance / (speed * .5f) ));
            cameraSequence.AppendCallback(() => OnGameEnded?.Invoke()); 
        }
        #endregion

        #region Public Method
        Vector3Int previousGridPosition = Vector3Int.zero; 
        Vector3Int gridPosition = Vector3Int.zero;
        Vector3Int mouseToGridPosition = Vector3Int.zero;
        private bool isValidTile = false;
        private Sequence cameraSequence;
        public void UpdatePrevisualisation(Vector2 _mousePosition)
        {
            if (cameraSequence.IsActive())
                cameraSequence.Kill(false);

            cameraSequence = DOTween.Sequence(); 
            if (_mousePosition.y <= (Screen.height * .15f))
            {
                // v = d/t => t = v/d
                float _distance = (camera.transform.position.y - bottomLimit); 
                if(_distance > 0)
                    cameraSequence.Append(camera.transform.DOLocalMoveY(bottomLimit, _distance/speed)); 
            }
            else if(_mousePosition.y >= Screen.height - (Screen.height * .15f ))
            {
                float _distance = (topLimit - camera.transform.position.y);
                if (_distance > 0)
                    cameraSequence.Append(camera.transform.DOLocalMoveY(topLimit, _distance/speed));
            }

            mouseToGridPosition = grid.WorldToCell(camera.ScreenToWorldPoint(_mousePosition));         
        }

        private Sequence placingSequence;
        private static Vector3 gridOffset = Vector2.one;
        public void PlaceTile()
        {
            if (rotationSequence.IsActive() || placingSequence.IsActive()) return; 

            if(isValidTile) 
            {
                Vector3Int _position = gridPosition; 
                GameGrid.FillPosition(_position.x, -_position.y, currentTileData, currentRotation);
                AudioManager.Instance.PlaySFX(AudioManager.Instance.TilePoseClip);
                placingSequence = DOTween.Sequence();
                placingSequence.Append(UIManager.Instance.RemovePrevisualisation());
                placingSequence.onComplete += () => OnSequenceValidate(_position);
            }

            void OnSequenceValidate(Vector3Int _position)
            {
                tilemap.SetTile(_position, currentTile);
                Instantiate(currentTileData.VFX, grid.CellToLocalInterpolated(_position) + gridOffset, Quaternion.Euler(0, 0, currentRotation));
                previsualisationTilemap.ClearAllTiles();
                ResetRotation();
                ProceedToNextTile(); 
            }
        }

        Sequence rotationSequence = null; 
        public void RotateTile()
        {
            if(!placingSequence.IsActive())
            {
                if (rotationSequence.IsActive())
                    return;
                currentRotation += 90;
                if (currentRotation >= 360) currentRotation = 0;


                float _transitionDuration = UIManager.Instance.RotatePrevisualisation(currentRotation); 
                rotationSequence = DOTween.Sequence();
                rotationSequence.AppendInterval(_transitionDuration);
                rotationSequence.AppendCallback(ResetRotationSequence); 

                void ResetRotationSequence()
                {
                    isValidTile = GameGrid.TryFillPosition(gridPosition.x, -gridPosition.y, currentRotation, currentTileData, out bool _displayTile);
                    previsualisationTilemap.ClearAllTiles();
                    if (_displayTile)
                    {
                        var _t = currentTile.transform; 
                        _t.SetTRS(Vector3.zero, Quaternion.Euler(0, 0, currentRotation), Vector3.one);
                        currentTile.transform = _t;

                        previsualisationTilemap.SetTile(gridPosition, currentTile); 
                        previsualisationTilemap.color = isValidTile ? validColor : invalidColor;
                    }
                    rotationSequence = null;
                }
            }
        }
        #endregion
    }
}

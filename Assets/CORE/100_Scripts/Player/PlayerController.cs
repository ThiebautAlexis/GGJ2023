using System;
using UnityEngine;
using UnityEngine.InputSystem; 

namespace GGJ2023
{
    public class PlayerController : MonoBehaviour
    {
        #region Static 
        public static readonly string MousePositionInput = "MousePosition";
        public static readonly string MouseClickInput = "MouseClick";
        public static readonly string SpaceBarInput = "Space";
        public static readonly string HelpInput = "Help";
        #endregion 

        #region Fields and Properties
        [Header("Game Inputs")]
        [SerializeField] private InputActionMap inputClick = null;
        #endregion

        #region Private Methods
        private void OnMousePosition(InputAction.CallbackContext _context)
        {
            if(_context.performed)
                GameManager.Instance.UpdatePrevisualisation(_context.ReadValue<Vector2>());
        }
        private void OnMouseClick(InputAction.CallbackContext _context)
        {
            if (_context.performed)
                GameManager.Instance.PlaceTile(); 
        }

        private void OnSpaceBarPressed(InputAction.CallbackContext _context)
        {
            if (_context.performed)
                GameManager.Instance.RotateTile(); 
        }
        private void ClickStartGame(InputAction.CallbackContext _context) => GameManager.Instance.StartGame();
        private void ResetGame(InputAction.CallbackContext _context)
        {
            if(_context.performed)
            {
                inputClick.Disable(); 
                UnityEngine.SceneManagement.SceneManager.LoadScene(0, UnityEngine.SceneManagement.LoadSceneMode.Single); 
            }
        }
        private void OnHelpHeld(InputAction.CallbackContext _context)
        {
            if (_context.started)
                UIManager.Instance.DisplayTuto(true); 
            if (_context.canceled)
                UIManager.Instance.DisplayTuto(false);
        }

        #endregion

        #region Private Methods
        private void Awake()
        {
            inputClick.Enable();
            inputClick.FindAction(MouseClickInput).performed += ClickStartGame;

            GameManager.OnGameStarted += EnableControls;
            GameManager.OnGameStopped += DisableControls; 
        }

        private void OnDestroy()
        {
            GameManager.OnGameStarted -= EnableControls;
            GameManager.OnGameStopped -= DisableControls;
        }
        #endregion

        #region Public Methods
        public void EnableControls()
        {
            inputClick.FindAction(MouseClickInput).performed -= ClickStartGame;
            inputClick.FindAction(MousePositionInput).performed += OnMousePosition;
            inputClick.FindAction(MouseClickInput).performed += OnMouseClick;
            inputClick.FindAction(SpaceBarInput).performed += OnSpaceBarPressed;
            inputClick.FindAction(HelpInput).started += OnHelpHeld; 
            inputClick.FindAction(HelpInput).canceled += OnHelpHeld; 
        }

        public void DisableControls()
        {
            inputClick.FindAction(MousePositionInput).performed -= OnMousePosition;
            inputClick.FindAction(MouseClickInput).performed -= OnMouseClick;
            inputClick.FindAction(SpaceBarInput).performed -= OnSpaceBarPressed;
            inputClick.FindAction(MouseClickInput).performed += ResetGame; 
            inputClick.FindAction(HelpInput).started -= OnHelpHeld; 
            inputClick.FindAction(HelpInput).canceled -= OnHelpHeld; 
            //inputClick.Disable();
        }
        #endregion 

    }
}

using UnityEngine;
using UnityEngine.InputSystem; 

namespace GGJ2023
{
    public class PlayerController : MonoBehaviour
    {
        #region Static 
        public static readonly string MousePositionInput = "MousePosition";
        public static readonly string MouseClickInput = "MouseClick";
        #endregion 

        #region Fields and Properties
        [Header("Game Inputs")]
        [SerializeField] private InputActionMap inputClick = null;
        #endregion

        #region Private Methods
        private void OnMousePosition(InputAction.CallbackContext _context) => GameManager.Instance.UpdatePrevisualisation(_context.ReadValue<Vector2>());

        private void OnMouseClick(InputAction.CallbackContext _context)
        {
            if (_context.ReadValueAsButton())
            {
                Debug.Log("Button Pressed");                
            }
        }
        #endregion

        #region Private Methods
        private void Start()
        {
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
            inputClick.Enable();
            inputClick.FindAction(MousePositionInput).performed += OnMousePosition;
            inputClick.FindAction(MouseClickInput).performed += OnMouseClick;
        }

        public void DisableControls()
        {
            inputClick.FindAction(MousePositionInput).performed -= OnMousePosition;
            inputClick.FindAction(MouseClickInput).performed -= OnMouseClick;
            inputClick.Disable();
        }
        #endregion 

    }
}

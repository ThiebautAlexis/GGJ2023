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
        private Vector2 mousePosition = Vector2.zero;
        #endregion

        #region Private Methods
        private void OnMousePosition(InputAction.CallbackContext _context) => mousePosition = _context.ReadValue<Vector2>();

        private void OnMouseClick(InputAction.CallbackContext _context)
        {
            Debug.Log("Value => " + _context.ReadValueAsButton()); 
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

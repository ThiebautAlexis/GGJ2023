using UnityEngine;
using UnityEngine.Tilemaps;

namespace GGJ2023
{
    public class GameManager : MonoBehaviour
    {
        #region Fields and Properties
        [SerializeField] private GridData baseGrid;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Tile tile; 
        #endregion

        private void Start()
        {
        }
    }
}

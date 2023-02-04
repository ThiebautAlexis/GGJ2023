using UnityEngine;

namespace GGJ2023
{
    [CreateAssetMenu(fileName = "Grid Data", menuName = "GGJ2023/Grid/GridData", order = 101)]
    public class GridData : ScriptableObject
    {
        #region Fields and Properties
        [SerializeField] private int xLength = 16;
        [SerializeField] private int yLength = 32;

        [SerializeField] private CellState[] cells = new CellState[] { };

        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        /// <summary>
        /// Cells within the grid converted as a 2D Array of CellState
        /// Use this property only once to init the Base Grid at the start of the game.
        /// </summary>
        public CellState[,] GetConvertedCells()
        {
                CellState[,] data = new CellState[xLength, yLength];
                for (int y = 0; y < xLength; y++)
                {
                    for (int x = 0; x < yLength; x++)
                    {
                        data[x, y] = cells[x + y]; 
                    }
                }
                return data; 
        }
        #endregion 
    }

}

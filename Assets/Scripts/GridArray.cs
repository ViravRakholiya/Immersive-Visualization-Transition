using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "GridArray", menuName = "GridArray", order = 0)]
    public class GridArray : ScriptableObject
    {
        public static float cellSize;
        public static double[,] gridArrayList;
    }
}
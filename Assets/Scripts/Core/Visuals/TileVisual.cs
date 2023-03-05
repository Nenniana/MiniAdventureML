using UnityEngine;
using Sirenix.OdinInspector;

namespace MiniAdventure.Visuals
{
    [CreateAssetMenu(fileName = "TileVisual", menuName = "MiniAdventure/TileVisual", order = 0)]
    [InlineEditor(Expanded = true)]
    public class TileVisual : ScriptableObject
    {
        [SerializeField]
        internal string symbol = "*";
        [SerializeField]
        internal Color textColor = new Color(51, 51, 51);
    }
}
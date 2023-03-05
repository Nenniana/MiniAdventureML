using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiniAdventure.Visuals {
    public class TileVisualComponent : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI text, rightText, leftText;

        public void InitializeTileVisual(TileVisual tileVisual, Vector2Int gridPosition)
        {
            UpdateTileVisual(tileVisual);
            transform.localPosition = new Vector3(gridPosition.x, gridPosition.y, 0);
        }

        public void InitializeTileVisual(Vector2Int gridPosition)
        {
            transform.localPosition = new Vector3(gridPosition.x, gridPosition.y, 0);
        }

        public void UpdateTileVisual(TileVisual tileVisual)
        {
            text.text = tileVisual.symbol;
            text.color = tileVisual.textColor;
            rightText.color = tileVisual.textColor;
            leftText.color = tileVisual.textColor;
        }

        public void UpdateRightText(string text)
        {
            rightText.text = text;
        }

        public void UpdateLeftText(string text)
        {
            leftText.text = text;
        }

        public void NullLeftRightText()
        {
            leftText.text = "";
            rightText.text = "";
        }
    }
}
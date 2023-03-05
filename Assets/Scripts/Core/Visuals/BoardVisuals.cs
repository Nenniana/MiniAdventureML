using UnityEngine;

namespace MiniAdventure.Visuals
{
    internal class BoardVisuals
    {
        private GameController gameController;
        private GameObject tileVisualPrefab;
        private TileVisualComponent[] visualComponents;

        internal BoardVisuals (GameController _gameController, GameObject _tileVisualPrefab) {
            gameController = _gameController;
            tileVisualPrefab = _tileVisualPrefab;
            InitializeBoardVisuals();
        }

        private void InitializeBoardVisuals() {
            visualComponents = new TileVisualComponent[gameController.stateSize];

            for (int i = 0; i < gameController.stateSize; i++) {
                int x = i / 8;
                int y = i % 8;

                TileVisualComponent tileComponent = GameObject.Instantiate(tileVisualPrefab, gameController.transform).GetComponent<TileVisualComponent>();
                tileComponent.InitializeTileVisual(GetTileVisual(gameController.state[i]), new Vector2Int(x, y));
                visualComponents[i] = tileComponent;
            }
        }

        internal void ResetBoardVisuals() {
            for (int i = 0; i < gameController.stateSize; i++) {
                UpdateTileVisuals(i);
            }
        }

        private TileVisual GetTileVisual(float tileVisualID) {
            return BoardVisualsDictionary.Instance.tileVisuals[(Helpers.WorldObject)tileVisualID];
        }

        internal void UpdateTileVisuals(int tileIndex) {
            visualComponents[tileIndex].UpdateTileVisual(GetTileVisual(gameController.state[tileIndex]));
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MiniAdventure.Helpers;
using MiniAdventure.Visuals;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MiniAdventure
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        internal GameObject tileVisualPrefab;

        [SerializeField]
        private InfoBoardController infoBoardController;

        internal float[] state;

        internal BoardVisuals boardVisuals;

        internal int stateSize;
        internal int worldObjects;
        internal PlayerController playerController;

        // Start is called before the first frame update
        void Start()
        {
            InitializeState();
            SetInitialPositions();
            InitializeBoardVisuals();
        }

        [Button]
        public void ResetGame() {
            SetInitialPositions();
            boardVisuals.ResetBoardVisuals();
        }

        private void InitializeState() {
            stateSize = GameManager.Instance.height * GameManager.Instance.width;
            worldObjects = GameManager.Instance.initialFlintAmount + GameManager.Instance.initialStickAmount + GameManager.Instance.initialTreeAmount + 1;
        }

        private void SetInitialPositions() {
            state = new float[stateSize];
            int[] initialPositions = Enumerable.Range(0, 63).OrderBy(arg => Guid.NewGuid()).Take(worldObjects).ToArray();

            for (int i = 0; i < initialPositions.Length; i++) { 
                if (i == 0) {
                    state[initialPositions[i]] = ConvertEnumToFloat(WorldObject.Player);
                    CreatePlayer(initialPositions[i]);
                }
                else if (i < GameManager.Instance.initialFlintAmount + 1) {
                    state[initialPositions[i]] = ConvertEnumToFloat(WorldObject.Flint);
                } else if (i < GameManager.Instance.initialFlintAmount + GameManager.Instance.initialStickAmount + 1) {
                    state[initialPositions[i]] = ConvertEnumToFloat(WorldObject.Stick);
                } else {
                    state[initialPositions[i]] = ConvertEnumToFloat(WorldObject.TreeFull);
                    // PlantTree(initialPositions[i]);
                }
            }
        }

        private void CreatePlayer(int index)
        {
            if (playerController == null) {
                playerController = new PlayerController(TranslateToVector2Int(index), this);
                infoBoardController.InitializeBoardVisuals(playerController);
            } else {
                playerController.ResetPlayerController(TranslateToVector2Int(index));
                infoBoardController.ResetBoardVisuals();
            }
        }

        private Vector2Int TranslateToVector2Int(int index) {
            return new Vector2Int(index / 8, index % 8);
        }

        private int TranslateToInt(Vector2Int position) {
            return position.x * 8 + position.y;
        }

        private bool IsValidTile(int x, int y)
        {
            return x >= 0 && x < GameManager.Instance.width && y >= 0 && y < GameManager.Instance.height;
        }

        private bool IsValidTile(Vector2Int position)
        {
            return position.x >= 0 && position.x < GameManager.Instance.width && position.y >= 0 && position.y < GameManager.Instance.height;
        }

        /* private void PlantTree(int index) {
            TreeController tree = new TreeController(index);
            tree.OnGrownFully += OnTreeGrownFully;
        }

        private void OnTreeGrownFully(int index)
        {
            state[index] = ConvertEnumToFloat(WorldObject.TreeFull);
            boardVisuals.UpdateTileVisuals(index);
        } */

        private float ConvertEnumToFloat(WorldObject worldObject) {
            return (float)worldObject;
        }

        private void InitializeBoardVisuals() {
            boardVisuals = new BoardVisuals(this, tileVisualPrefab);
        }

        private bool OccupiedTile(Vector2Int position) {
            return (state[TranslateToInt(position)] != (float)WorldObject.Ground);
        }

        internal bool PlayerAction(Vector2Int newPosition, InteractionType type)
        {
            if (!IsValidTile(newPosition))
                return false;

            if (type == InteractionType.Interact && OccupiedTile(newPosition))
                return AttemptInteract(newPosition);

            if (OccupiedTile(newPosition))
                return false;

            if (type == InteractionType.Move) {
                MovePlayer(newPosition);
                return true;
            }

            if (type == InteractionType.ContructFire) {
                ConstructFire(newPosition);
                return true;
            }

            return false;
        }

        private void ConstructFire(Vector2Int newPosition)
        {
            if (playerController.inventory.ConstructFire()) {
                UpdatePosition(TranslateToInt(newPosition), WorldObject.Fireplace);
                playerController.AddFire(newPosition);
            }
        }

        private bool AttemptInteract(Vector2Int newPlayerPosition)
        {
            int indexOfInteraction = TranslateToInt(newPlayerPosition);
            
            if (state[indexOfInteraction] == (float)WorldObject.Flint) {
                UpdatePosition(indexOfInteraction, WorldObject.Ground);
                playerController.inventory.AddFlint();
                return true;
            }
            else if (state[indexOfInteraction] == (float)WorldObject.WoodLog) {
                UpdatePosition(indexOfInteraction, WorldObject.Ground);
                playerController.inventory.AddWood();
                return true;
            }
            else if (state[indexOfInteraction] == (float)WorldObject.Stick)
            {
                UpdatePosition(indexOfInteraction, WorldObject.Ground);
                playerController.inventory.AddStick();
                return true;
            }
            else if (state[indexOfInteraction] == (float)WorldObject.TreeFull) {
                return ChopTree(indexOfInteraction);
            }

            return false;
        }

        private bool ChopTree(int indexOfInteraction)
        {
            if (playerController.inventory.AxeAmount <= 0)
                return false;

            UpdatePosition(indexOfInteraction, WorldObject.Ground);
            SpawnWood(TranslateToVector2Int(indexOfInteraction));
            return true;
        }

        private void SpawnWood(Vector2Int treePosition) {
            CheckWood(treePosition + new Vector2Int(0, 1));
            CheckWood(treePosition + new Vector2Int(0, -1));
            CheckWood(treePosition + new Vector2Int(1, 0));
            CheckWood(treePosition + new Vector2Int(-1, 0));
        }

        private void CheckWood(Vector2Int woodPosition) {
            if (IsValidTile(woodPosition) && !OccupiedTile(woodPosition))
                UpdatePosition(TranslateToInt(woodPosition), WorldObject.WoodLog);
        }

        private void PickUpItem(int indexOfInteraction)
        {
            WorldObject objectType = (WorldObject)state[indexOfInteraction];
            UpdatePosition(indexOfInteraction, WorldObject.Ground);
        }

        private void MovePlayer(Vector2Int playerPosition) {
            UpdatePosition(TranslateToInt(playerController.gridPosition), WorldObject.Ground);

            playerController.gridPosition = playerPosition;

            UpdatePosition(TranslateToInt(playerController.gridPosition), WorldObject.Player);
        }

        private void UpdatePosition(int playerPosition, WorldObject worldObject) {
            state[playerPosition] = (float)worldObject;
            boardVisuals.UpdateTileVisuals(playerPosition);
        }
    }
}
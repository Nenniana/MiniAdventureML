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
        public Action OnWoodChopped, OnFireConstructed, OnWoodAttemptFail, OnInteractFail;
        public Action<int,int> PlayerMoved, NullObject;

        [SerializeField]
        internal GameObject tileVisualPrefab;

        [SerializeField]
        private InfoBoardController infoBoardController;
        internal float[] state;
        internal BoardVisuals boardVisuals;
        internal int stateSize;
        internal int worldObjects;
        internal PlayerController playerController;
        internal AlphaAgent agent;
        internal List<DistanceStruct> closestFlints;
        internal List<DistanceStruct> closestTrees;
        internal List<DistanceStruct> closestSticks;
        internal List<DistanceStruct> closestFires;
        internal List<DistanceStruct> closestLogs;

        // Start is called before the first frame update
        /* void Start()
        {
            FirstGame();
        } */

        [Button]
        public void FireRequestDecision()
        {
            agent.RequestDecision();
        }
        
        public void FirstGame(AlphaAgent _agent)
        {
            agent = _agent;
            closestTrees = new List<DistanceStruct>();
            closestFires = new List<DistanceStruct>();
            closestFlints = new List<DistanceStruct>();
            closestLogs = new List<DistanceStruct>();
            closestSticks = new List<DistanceStruct>();
            InitializeState();
            SetInitialPositions();
            InitializeBoardVisuals();
        }

        [Button]
        public void ResetGame()
        {
            closestTrees.Clear();
            closestFires.Clear();
            closestFlints.Clear();
            closestLogs.Clear();
            closestSticks.Clear();
            SetInitialPositions();
            ResetBoardVisuals();
        }

        private void ResetBoardVisuals()
        {
            if (GameManager.Instance.showBoard)
                boardVisuals.ResetBoardVisuals();
        }

        private void InitializeState()
        {
            stateSize = GameManager.Instance.height * GameManager.Instance.width;
            worldObjects = GameManager.Instance.initialFlintAmount + GameManager.Instance.initialStickAmount + GameManager.Instance.initialTreeAmount + 1;
        }

        private void SetInitialPositions()
        {
            state = new float[stateSize];
            int[] initialPositions = Enumerable.Range(0, 63).OrderBy(arg => Guid.NewGuid()).Take(worldObjects).ToArray();

            for (int i = 0; i < initialPositions.Length; i++)
            {
                if (i == 0)
                {
                    state[initialPositions[i]] = ConvertEnumToFloat(WorldObject.Player);
                    CreatePlayer(initialPositions[i]);
                }
                else if (i < GameManager.Instance.initialFlintAmount + 1)
                {
                    float type = ConvertEnumToFloat(WorldObject.Flint);
                    state[initialPositions[i]] = type;
                    RecordObjectPosition(initialPositions[i], type, ref closestFlints);
                }
                else if (i < GameManager.Instance.initialFlintAmount + GameManager.Instance.initialStickAmount + 1)
                {
                    float type = ConvertEnumToFloat(WorldObject.Stick);
                    state[initialPositions[i]] = type;
                    RecordObjectPosition(initialPositions[i], type, ref closestSticks);
                }
                else
                {
                    float type = ConvertEnumToFloat(WorldObject.TreeFull);
                    state[initialPositions[i]] = type;
                    RecordObjectPosition(initialPositions[i], type, ref closestTrees);
                }
            }

            for (int i = 0; i < GameManager.Instance.maxFiresToKeep; i++) {
                NullObjectConstruct(ref closestFires, WorldObject.Fireplace);
            }

            for (int i = 0; i < GameManager.Instance.maxLogsToKeep; i++) {
                NullObjectConstruct(ref closestLogs, WorldObject.WoodLog);
            }
        }

        private void NullObjectConstruct(ref List<DistanceStruct> dict, WorldObject type) {
            dict.Add(new DistanceStruct(100, 100, 100, ConvertEnumToFloat(type), this, true));
        }

        private void RecordObjectPosition(int index, float type, ref List<DistanceStruct> dict) {
            RecordObjectPosition(TranslateToVector2Int(index), type, ref dict);
        }

        private void RecordObjectPosition(int x, int y, float type, ref List<DistanceStruct> dict) {
            RecordObjectPosition(new Vector2Int(x, y), type, ref dict);
        }

        private void RecordObjectPosition(Vector2Int position, float type, ref List<DistanceStruct> dict)
        {
            float distance = GetDistanceToPlayer(position);
            dict.Add(new DistanceStruct(position.x, position.y, distance, type, this));
        }

        private void SwitchOutIfCloser(ref List<DistanceStruct> listDistance) {
            // Debug.Log("Before reorder:");
            // LogOrder(listDistance);
            listDistance = listDistance.OrderBy(object1 => object1.dist).ToList();
            // Debug.Log("After reorder:");
            // LogOrder(listDistance);
        }

        private void LogOrder(List<DistanceStruct> listDistance) {
            foreach (DistanceStruct distance in listDistance) {
                Debug.Log(distance.dist);
            }
            Debug.Log("End list.");
        }

        private void NullObjectByIndex(int index) {
            Vector2Int position = TranslateToVector2Int(index);
            NullObject?.Invoke(position.x, position.y);
        }

        internal void SortLists() {
            SwitchOutIfCloser(ref closestTrees);
            SwitchOutIfCloser(ref closestFlints);
            SwitchOutIfCloser(ref closestLogs);
            SwitchOutIfCloser(ref closestSticks);
            SwitchOutIfCloser(ref closestFires);
        }

        private float GetDistanceToPlayer(Vector2Int objectPosition)
        {
            return Vector2Int.Distance(objectPosition, playerController.GridPosition);
        }

        private void CreatePlayer(int index)
        {
            if (playerController == null)
            {
                playerController = new PlayerController(TranslateToVector2Int(index), this);
                if (GameManager.Instance.showInfoBoard)
                    infoBoardController.InitializeBoardVisuals(playerController, agent);
            }
            else
            {
                playerController.ResetPlayerController(TranslateToVector2Int(index));
                if (GameManager.Instance.showInfoBoard)
                    infoBoardController.ResetBoardVisuals();
            }
        }

        private Vector2Int TranslateToVector2Int(int index)
        {
            return new Vector2Int(index / 8, index % 8);
        }

        private int TranslateToInt(Vector2Int position)
        {
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

        private float ConvertEnumToFloat(WorldObject worldObject)
        {
            return (float)worldObject;
        }

        private void InitializeBoardVisuals()
        {
            if (GameManager.Instance.showBoard)
                boardVisuals = new BoardVisuals(this, tileVisualPrefab);
        }

        private bool OccupiedTile(Vector2Int position)
        {
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

            if (type == InteractionType.Move)
            {
                MovePlayer(newPosition);
                PlayerMoved?.Invoke(newPosition.x, newPosition.y);
                return true;
            }

            if (type == InteractionType.ContructFire)
            {
                ConstructFire(newPosition);
                return true;
            }

            return false;
        }

        internal bool CanPerformAction(Vector2Int newPosition, InteractionType type) 
        {
            if (!IsValidTile(newPosition))
                return false;

            if (type == InteractionType.Interact && OccupiedTile(newPosition))
                return CanInteract(newPosition);

            if (OccupiedTile(newPosition))
                return false;

            if (type == InteractionType.Move || type == InteractionType.ContructFire)
                return true;

            return false;
        }

        private void ConstructFire(Vector2Int newPosition)
        {
            if (playerController.inventory.ConstructFire())
            {
                UpdatePosition(TranslateToInt(newPosition), WorldObject.Fireplace);
                RecordObjectPosition(newPosition, ConvertEnumToFloat(WorldObject.WoodLog), ref closestFires);
                playerController.AddFire(newPosition);
                OnFireConstructed?.Invoke();
            }
        }

        private bool AttemptInteract(Vector2Int newPlayerPosition)
        {
            int indexOfInteraction = TranslateToInt(newPlayerPosition);

            if (state[indexOfInteraction] == (float)WorldObject.Flint)
            {
                UpdatePosition(indexOfInteraction, WorldObject.Ground);
                NullObjectByIndex(indexOfInteraction);
                playerController.inventory.AddFlint();
                return true;
            }
            else if (state[indexOfInteraction] == (float)WorldObject.WoodLog)
            {
                UpdatePosition(indexOfInteraction, WorldObject.Ground);
                NullObjectByIndex(indexOfInteraction);
                playerController.inventory.AddWood();
                return true;
            }
            else if (state[indexOfInteraction] == (float)WorldObject.Stick)
            {
                UpdatePosition(indexOfInteraction, WorldObject.Ground);
                NullObjectByIndex(indexOfInteraction);
                playerController.inventory.AddStick();
                return true;
            }
            else if (state[indexOfInteraction] == (float)WorldObject.TreeFull)
            {
                return ChopTree(indexOfInteraction);
            }

            OnInteractFail?.Invoke();
            return false;
        }

        private bool CanInteract(Vector2Int newPlayerPosition) {
            int indexOfInteraction = TranslateToInt(newPlayerPosition);

            if (state[indexOfInteraction] == (float)WorldObject.Flint
                || state[indexOfInteraction] == (float)WorldObject.WoodLog
                || state[indexOfInteraction] == (float)WorldObject.Stick)
                return true;
            else if (state[indexOfInteraction] == (float)WorldObject.TreeFull)
                return playerController.inventory.AxeAmount > 0;

            return false;
        }

        private bool ChopTree(int indexOfInteraction)
        {
            if (playerController.inventory.AxeAmount <= 0)
            {
                OnWoodAttemptFail?.Invoke();
                return false;
            }

            UpdatePosition(indexOfInteraction, WorldObject.Ground);
            NullObjectByIndex(indexOfInteraction);
            SpawnWood(TranslateToVector2Int(indexOfInteraction));
            OnWoodChopped?.Invoke();
            return true;
        }

        private void SpawnWood(Vector2Int treePosition)
        {
            CheckWood(treePosition + new Vector2Int(0, 1));
            CheckWood(treePosition + new Vector2Int(0, -1));
            CheckWood(treePosition + new Vector2Int(1, 0));
            CheckWood(treePosition + new Vector2Int(-1, 0));
        }

        private void CheckWood(Vector2Int woodPosition)
        {
            if (IsValidTile(woodPosition) && !OccupiedTile(woodPosition)) {
                UpdatePosition(TranslateToInt(woodPosition), WorldObject.WoodLog);
                RecordObjectPosition(woodPosition, ConvertEnumToFloat(WorldObject.WoodLog), ref closestLogs);
            }
        }

        private void MovePlayer(Vector2Int playerPosition)
        {
            UpdatePosition(TranslateToInt(playerController.GridPosition), WorldObject.Ground);

            playerController.GridPosition = playerPosition;

            UpdatePosition(TranslateToInt(playerController.GridPosition), WorldObject.Player);
        }

        private void UpdatePosition(int newObjectPosition, WorldObject worldObject)
        {
            state[newObjectPosition] = (float)worldObject;
            if (GameManager.Instance.showBoard)
                boardVisuals.UpdateTileVisuals(newObjectPosition);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using MiniAdventure.Helpers;
using UnityEngine;

namespace MiniAdventure
{
    public class PlayerController
    {
        public Action OnWarmthChange, OnDeath, OnWarmingUp, OnCoolingDown, OnWarmAgain, PlayerStoodStill;

        internal Inventory inventory;
        private Vector2Int gridPosition;
        internal float[] playerInfo;
        private GameController gameController;
        private float warmth;
        internal float Warmth { get => warmth; private set { warmth = value; if (warmth > GameManager.Instance.InitialWarmth) OnWarmAgain?.Invoke(); } }
        internal Vector2Int GridPosition { get => gridPosition; set { gridPosition = value; SetPlayerInfo(gridPosition.x, gridPosition.y); }}
        private int counterHeat = 0;
        private int counterCold = 0;
        private List<Vector2Int> fires;
        internal bool isWarmingUp = false;
        

        public PlayerController(Vector2Int _gridPosition, GameController _gameController)
        {
            GridPosition = _gridPosition;
            gameController = _gameController;
            fires = new List<Vector2Int>();
            Warmth = GameManager.Instance.InitialWarmth;
            inventory = new Inventory();
            inventory.ResetInventory(0, 0, 0, 0);

            GameManager.Instance.OnContructAxe += ConstructAxe;
            GameManager.Instance.OnAction += PerformAction;
            GameManager.Instance.OnTick += OnTicked;
        }

        private void SetPlayerInfo(float x, float y) {
            if (playerInfo == null)
                playerInfo = new float[]{x, y, 70f};
            else {
                playerInfo[0] = x;
                playerInfo[1] = y;
                playerInfo[2] = 70;
            }

            // Debug.Log("PlayerInfo: " + playerInfo[0] + ", " + playerInfo[1]);
        }

        internal void ResetPlayerController(Vector2Int _gridPosition)
        {
            counterHeat = 0;
            counterCold = 0;
            isWarmingUp = false;
            fires.Clear();
            GridPosition = _gridPosition;
            inventory.ResetInventory(0, 0, 0, 0);
            Warmth = GameManager.Instance.InitialWarmth;
        }

        private void OnTicked()
        {
            if(!GameManager.Instance.disableWarmthDecrease)
                if (CloseToFire())
                {
                    counterHeat++;
                    if (Warmth <= GameManager.Instance.InitialWarmth && counterHeat >= GameManager.Instance.warmthIncreaseRate)
                    {
                        isWarmingUp = true;
                        counterHeat = 0;
                        counterCold = 0;
                        Warmth++;
                        OnWarmthChange?.Invoke();
                        OnWarmingUp?.Invoke();
                    }
                }
                else
                {
                    counterCold++;
                    if (counterCold >= GameManager.Instance.warmthDecreaseRate)
                    {
                        counterHeat = 0;
                        counterCold = 0;
                        isWarmingUp = false;
                        Warmth--;
                        OnWarmthChange?.Invoke();
                        OnCoolingDown?.Invoke();

                        if (warmth <= 0)
                        {
                            OnDeath?.Invoke();
                        }
                    }
                }
        }

        internal void AddFire(Vector2Int _position)
        {
            fires.Add(_position);
        }

        private bool CloseToFire()
        {
            foreach (var fire in fires)
            {
                if (Vector2Int.Distance(GridPosition, fire) < GameManager.Instance.fireplaceMaxDistance)
                {
                    return true;
                }
            }

            return false;
        }

        internal void PerformAction(int dir, int interactionType)
        {
            interactionType = interactionType * 10;

            // Debug.Log("PerformAction " + dir + " " + interactionType);

            if (dir == 0)
                gameController.PlayerAction(GridPosition + new Vector2Int(0, 1), (InteractionType)interactionType);
            else if (dir == 1)
                gameController.PlayerAction(GridPosition + new Vector2Int(1, 0), (InteractionType)interactionType);
            else if (dir == 2)
                gameController.PlayerAction(GridPosition + new Vector2Int(0, -1), (InteractionType)interactionType);
            else if (dir == 3)
                gameController.PlayerAction(GridPosition + new Vector2Int(-1, 0), (InteractionType)interactionType);
            else
                PlayerStoodStill?.Invoke();

            gameController.SortLists();
        }

        internal bool CanPerformAction(int dir, int interactionType)
        {
            interactionType = interactionType * 10;

            if (dir == 0)
                return gameController.CanPerformAction(GridPosition + new Vector2Int(0, 1), (InteractionType)interactionType);
            else if (dir == 1)
                return gameController.CanPerformAction(GridPosition + new Vector2Int(1, 0), (InteractionType)interactionType);
            else if (dir == 2)
                return gameController.CanPerformAction(GridPosition + new Vector2Int(0, -1), (InteractionType)interactionType);
            else if (dir == 3)
                return gameController.CanPerformAction(GridPosition + new Vector2Int(-1, 0), (InteractionType)interactionType);
            
            return false;
        }

        internal void ConstructAxe(int choice)
        {
            if (choice == 1)
                inventory.BuildAxe();
        }

        public void Die()
        {
            GameManager.Instance.OnAction -= PerformAction;
            GameManager.Instance.OnTick -= OnTicked;
            GameManager.Instance.OnContructAxe -= ConstructAxe;
        }
    }
}
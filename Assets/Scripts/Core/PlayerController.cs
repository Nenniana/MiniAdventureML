using System;
using System.Collections;
using System.Collections.Generic;
using MiniAdventure.Helpers;
using UnityEngine;

namespace MiniAdventure
{
    public class PlayerController
    {
        public Action OnWarmthChange, OnDeath, OnWarmingUp, OnCoolingDown;

        internal Inventory inventory;
        internal Vector2Int gridPosition;
        private GameController gameController;
        private float warmth;
        public float Warmth { get => warmth; private set => warmth = value; }
        private int counterHeat = 0;
        private int counterCold = 0;

        private List<Vector2Int> fires;

        public PlayerController (Vector2Int _gridPosition, GameController _gameController) {
            gridPosition = _gridPosition;
            gameController = _gameController;
            fires = new List<Vector2Int>();
            Warmth = GameManager.Instance.InitialWarmth;
            inventory = new Inventory();
            inventory.ResetInventory(0,0,0,0);

            GameManager.Instance.OnContructAxe += ConstructAxe;
            GameManager.Instance.OnAction += PerformAction;
            GameManager.Instance.OnTick += OnTicked;
        }

        internal void ResetPlayerController(Vector2Int _gridPosition)
        {
            counterHeat = 0;
            counterCold = 0;
            fires.Clear();
            gridPosition = _gridPosition;
            inventory.ResetInventory(0, 0, 0, 0);
            Warmth = GameManager.Instance.InitialWarmth;
        }

        private void OnTicked()
        {
            if (CloseToFire()) {
                counterHeat++;
                if (warmth < GameManager.Instance.InitialWarmth && counterHeat >= GameManager.Instance.warmthIncreaseRate) {
                    counterHeat = 0;
                    counterCold = 0;
                    warmth++;
                    OnWarmthChange?.Invoke();
                    OnWarmingUp?.Invoke();
                }
            } else {
                counterCold++;
                if (counterCold >= GameManager.Instance.warmthDecreaseRate) {
                    counterHeat = 0;
                    counterCold = 0;
                    warmth--;
                    OnWarmthChange?.Invoke();
                    OnCoolingDown?.Invoke();

                    if (warmth <= 0) {
                        OnDeath?.Invoke();
                    }
                }
            }
        }

        internal void AddFire(Vector2Int _position) {
            fires.Add(_position);
        }

        private bool CloseToFire() {
            foreach (var fire in fires) {
                if (Vector2Int.Distance(gridPosition, fire) < GameManager.Instance.fireplaceMaxDistance) {
                    return true;
                }
            }

            return false;
        }

        internal void PerformAction(int dir, int interactionType) {
            interactionType = interactionType * 10;

            // Debug.Log("PerformAction " + dir + " " + interactionType);

            if (dir == 0)
                gameController.PlayerAction(gridPosition + new Vector2Int(0, 1), (InteractionType)interactionType);
            else if (dir == 1)
                gameController.PlayerAction(gridPosition + new Vector2Int(1, 0), (InteractionType)interactionType);
            else if (dir == 2)
                gameController.PlayerAction(gridPosition + new Vector2Int(0, -1), (InteractionType)interactionType);
            else if (dir == 3)
                gameController.PlayerAction(gridPosition + new Vector2Int(-1, 0), (InteractionType)interactionType);
        }

        internal void ConstructAxe(int choice) {
            if (choice == 1)
                inventory.AddAxe();
        }

        public void Die() {
            GameManager.Instance.OnAction -= PerformAction;
            GameManager.Instance.OnTick -= OnTicked;
            GameManager.Instance.OnContructAxe -= ConstructAxe;
        }
    }
}
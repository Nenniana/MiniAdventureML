using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniAdventure {
    public class InfoBoardController : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI reward, warmth, wood, flint, stick, axe;

        private PlayerController playerController;
        private AlphaAgent agent;

        internal void InitializeBoardVisuals(PlayerController _playerController, AlphaAgent _agent) {
            agent = _agent;
            playerController = _playerController;
            playerController.inventory.OnFlintAdded += OnFlintAdded;
            playerController.inventory.OnWoodAdded += OnWoodAdded;
            playerController.inventory.OnStickAdded += OnStickAdded;
            playerController.inventory.OnAxeAdded += OnAxeAdded;
            playerController.OnWarmthChange += OnWarmthChanged;
            agent.OnRewardUpdated += OnRewardUpdated;
        }

        private void OnDisable() {
            playerController.inventory.OnFlintAdded -= OnFlintAdded;
            playerController.inventory.OnWoodAdded -= OnWoodAdded;
            playerController.inventory.OnStickAdded -= OnStickAdded;
            playerController.inventory.OnAxeAdded -= OnAxeAdded;
            playerController.OnWarmthChange -= OnWarmthChanged;
            agent.OnRewardUpdated -= OnRewardUpdated;
        }

        internal void ResetBoardVisuals() {
            OnStickAdded();
            OnFlintAdded();
            OnWoodAdded();
            OnAxeAdded();
            OnWarmthChanged();
        }

        private void OnStickAdded()
        {
            stick.text = "Stick: " + playerController.inventory.StickAmount;
        }

        private void OnWoodAdded()
        {
            wood.text = "Wood: " + playerController.inventory.WoodAmount;
        }

        private void OnFlintAdded()
        {
            flint.text = "Flint: " + playerController.inventory.FlintAmount;
        }

        private void OnAxeAdded()
        {
            axe.text = "Axe: " + playerController.inventory.AxeAmount;
        }

        private void OnWarmthChanged()
        {
            warmth.text = "Warmth: " + playerController.Warmth;
        }

        private void OnRewardUpdated(float obj)
        {
            reward.text = "Reward: " + obj;
        }
    }
}
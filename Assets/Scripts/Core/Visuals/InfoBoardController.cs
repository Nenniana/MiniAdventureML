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

        internal void InitializeBoardVisuals(PlayerController _playerController) {
            playerController = _playerController;
            playerController.inventory.OnFlintAdded += OnFlintAdded;
            playerController.inventory.OnWoodAdded += OnWoodAdded;
            playerController.inventory.OnStickAdded += OnStickAdded;
            playerController.inventory.OnAxeAdded += OnAxeAdded;
            playerController.OnWarmthChange += OnWarmthChanged;
        }

        private void OnDisable() {
            playerController.inventory.OnFlintAdded -= OnFlintAdded;
            playerController.inventory.OnWoodAdded -= OnWoodAdded;
            playerController.inventory.OnStickAdded -= OnStickAdded;
            playerController.inventory.OnAxeAdded -= OnAxeAdded;
            playerController.OnWarmthChange -= OnWarmthChanged;
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
    }
}
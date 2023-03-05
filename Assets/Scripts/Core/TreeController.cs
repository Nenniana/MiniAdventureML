using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniAdventure
{
    public class TreeController
    {
        public event Action<int> OnGrownFully;

        internal int gridIndex;
        private bool isGrown;
        private int ticksAlive;
        public TreeController(int index) {
            gridIndex = index;
            isGrown = false;
            ticksAlive = 0;
            // GameManager.Instance.OnTick += OnTicked;
        }

        private void Die() {
            // GameManager.Instance.OnTick -= OnTicked;
        }

        private void OnTicked()
        {
            ticksAlive++;

            if (!isGrown && ticksAlive >= GameManager.Instance.TreeGrowTime) {
                isGrown = true;
                OnGrownFully?.Invoke(gridIndex);
            }
        }
    }
}
using System;

namespace MiniAdventure
{
    internal class Inventory
    {
        public Action OnWoodAdded;
        public Action OnFlintAdded;
        public Action OnStickAdded;
        public Action OnAxeAdded;

        internal void ResetInventory (int flint, int stick, int wood, int axe) {
            FlintAmount = flint;
            StickAmount = stick;
            WoodAmount = wood;
            AxeAmount = axe;
        }

        private int flintAmount;
        private int stickAmount;
        private int woodAmount;
        private int axeAmount;

        public int FlintAmount { get => flintAmount; private set { flintAmount = value; OnFlintAdded?.Invoke(); } }
        public int StickAmount { get => stickAmount; private set { stickAmount = value; OnStickAdded?.Invoke(); } }
        public int WoodAmount { get => woodAmount; private set { woodAmount = value; OnWoodAdded?.Invoke(); } }
        public int AxeAmount { get => axeAmount; private set { axeAmount = value; OnAxeAdded?.Invoke(); } }

        internal void AddFlint () {
            FlintAmount++;
        }

        internal void AddStick () {
            StickAmount++;
        }

        internal void AddWood () {
            WoodAmount++;
        }

        internal void AddAxe () {
            if (FlintAmount > 0 && StickAmount > 0) {
                FlintAmount--;
                StickAmount--;
                AxeAmount++;
            }  
        }

        internal bool ConstructFire() {
            if (FlintAmount > 0 && WoodAmount > 1) {
                WoodAmount = WoodAmount - 2;
                FlintAmount--;
                return true;
            }

            return false;
        }
    }
}
using System;

namespace MiniAdventure
{
    internal class Inventory
    {
        public Action OnWoodAdded, OnFlintAdded, OnStickAdded, OnAxeAdded, OnAxeAttemptFail, OnFireAttemptFail;

        internal void ResetInventory(int flint, int stick, int wood, int axe)
        {
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

        internal void AddFlint()
        {
            FlintAmount++;
        }

        internal void AddStick()
        {
            StickAmount++;
        }

        internal void AddWood()
        {
            WoodAmount++;
        }

        internal bool IsAxeBuildable()
        {
            if (FlintAmount >= GameManager.Instance.FlintToCreateAxe && StickAmount >= GameManager.Instance.SticksToCreateAxe)
                return true;
            return false;
        }

        internal bool BuildAxe()
        {
            if (IsAxeBuildable())
            {
                FlintAmount = FlintAmount - GameManager.Instance.FlintToCreateAxe;
                StickAmount = StickAmount - GameManager.Instance.SticksToCreateAxe;
                AxeAmount++;
                return true;
            }
            OnAxeAttemptFail?.Invoke();
            return false;
        }

        internal bool IsFireConstructable()
        {
            if (FlintAmount >= GameManager.Instance.FlintToCreateFire && WoodAmount >= GameManager.Instance.WoodToCreateFire)
                return true;
            return false;
        }

        internal bool ConstructFire()
        {
            if (IsFireConstructable())
            {
                WoodAmount = WoodAmount - GameManager.Instance.WoodToCreateFire;
                FlintAmount = FlintAmount - GameManager.Instance.FlintToCreateFire;
                return true;
            }
            OnFireAttemptFail?.Invoke();
            return false;
        }
    }
}
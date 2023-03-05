using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MiniAdventure.Helpers;
using System;
using System.Linq;

namespace MiniAdventure.Visuals
{
    public class BoardVisualsDictionary : SerializedMonoBehaviour
    {
        [SerializeField]
        internal Dictionary<WorldObject, TileVisual> tileVisuals;

        [HideInInspector]
        public static BoardVisualsDictionary Instance { get; private set; }
        // Start is called before the first frame update

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        [Button]
        public void FillDictonary()
        {
            if (tileVisuals == null || tileVisuals.Count == 0)
                tileVisuals = new Dictionary<WorldObject, TileVisual>();

            foreach (WorldObject tileType in Enum.GetValues(typeof(WorldObject)).Cast<WorldObject>().ToArray())
            {
                if (!tileVisuals.ContainsKey(tileType))
                {
                    tileVisuals.Add(tileType, ScriptableObject.CreateInstance<TileVisual>());
                }
            }
        }
    }
}
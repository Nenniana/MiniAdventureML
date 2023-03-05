using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using MiniAdventure.Helpers;

namespace MiniAdventure
{
    public class GameManager : MonoBehaviour
    {
        public Action<int, float> OnAction;
        public Action<int> OnContructAxe;
        public Action OnTick;

        [HideInInspector]
        public static GameManager Instance { get; private set; }

        [SerializeField]
        [FoldoutGroup("Board Settings")]
        internal int height = 8;
        [SerializeField]
        [FoldoutGroup("Board Settings")]
        internal int width = 8;
        [SerializeField]
        [FoldoutGroup("World Settings")]
        internal int initialTreeAmount = 3;
        [SerializeField]
        [FoldoutGroup("World Settings")]
        internal int initialFlintAmount = 2;
        [SerializeField]
        [FoldoutGroup("World Settings")]
        internal int initialStickAmount = 2;
        [SerializeField]
        [FoldoutGroup("World Settings")]
        internal float InitialWarmth = 100;
        [SerializeField]
        [FoldoutGroup("Time Settings")]
        internal int TreeGrowTime = 2000;
        [SerializeField]
        [FoldoutGroup("Time Settings")]
        internal float warmthDecreaseRate = 1;
        [SerializeField]
        [FoldoutGroup("Time Settings")]
        internal float warmthIncreaseRate = 0.5f;
        [SerializeField]
        [FoldoutGroup("Distance Settings")]
        internal float fireplaceMaxDistance = 2f;

        private InteractionType interactionType = InteractionType.Move;        

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

        private void FixedUpdate() {
            OnTick?.Invoke();
        }

        private void Update() {
            if (Input.GetKey(KeyCode.I)) {
                interactionType = InteractionType.Interact;
            } else if (Input.GetKey(KeyCode.C))
            {
                interactionType = InteractionType.ContructFire;
            } else {
                interactionType = InteractionType.Move;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                OnAction?.Invoke(0, (float)interactionType);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                OnAction?.Invoke(1, (float)interactionType);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                OnAction?.Invoke(2, (float)interactionType);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                OnAction?.Invoke(3, (float)interactionType);
            } 
            else if (Input.GetKeyDown(KeyCode.A)) 
            {
                OnContructAxe?.Invoke(1);
            }
        }
    }
}
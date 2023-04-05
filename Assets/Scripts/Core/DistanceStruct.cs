using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniAdventure {
    public class DistanceStruct
    {
        internal float x;
        internal float y;
        internal float dist;
        internal float type;
        GameController gameController;
        private bool nulled = false;
        private float nulledfloat = 1f;

        public DistanceStruct(float _x, float _y, float _dist, float _type, GameController _gameController, bool _nulled = false) {
            x = _x;
            y = _y;
            type = _type;
            gameController = _gameController;
            nulled = _nulled;

            if (!nulled) {
                gameController.PlayerMoved += OnPlayerMoved;
                gameController.NullObject += OnNullObject;
                dist = _dist;
            } else {
                nulledfloat = 0f;
                dist = 1000;
            }
        }

        ~DistanceStruct()
        {
            if (!nulled) {
                gameController.PlayerMoved -= OnPlayerMoved;
                gameController.NullObject -= OnNullObject;
            }
        }

        internal float[] GetInformation() {
            return new float[] { x, y, nulledfloat, type };
        }

        internal void OnNullObject(int _x, int _y) {
            if (x == _x && y == _y) {
                dist = 1000;
                gameController.PlayerMoved -= OnPlayerMoved;
                gameController.NullObject -= OnNullObject;
                nulled = true;
                nulledfloat = 0f;
            }
        }

        private void OnPlayerMoved(int arg1, int arg2) {
            dist = Vector2.Distance(new Vector2(x, y), new Vector2(arg1, arg2));
        }
    }
}
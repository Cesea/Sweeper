﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{

    [System.Serializable]
    public class Timer
    {
        public float TargetTime { get; set; }
        private float _currentTime = 0.0f;

        public Timer(float targetTime)
        {
            TargetTime = targetTime;
        }

        public bool Tick(float deltaTime)
        {
            _currentTime += deltaTime;

            if (_currentTime >= TargetTime)
            {
                Debug.Log("haha");
                _currentTime -= TargetTime;
                return true;
            }

            return false;
        }

        public void ResetTimer()
        {
            _currentTime = 0.0f;
        }
    }
}

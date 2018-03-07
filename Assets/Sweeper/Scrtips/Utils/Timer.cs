using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{

    [System.Serializable]
    public class Timer
    {
        private float _targetTime;
        public float TargetTime {
            get { return _targetTime; }
            set { _targetTime = value; Reset(); }
        }

        private float _currentTime = 0.0f;

        public float Percent
        {
            get { return _currentTime / TargetTime; }
        }

        public Timer(float targetTime)
        {
            _targetTime = targetTime;
        }

        public bool Tick(float deltaTime)
        {
            _currentTime += deltaTime;

            if (_currentTime >= TargetTime)
            {
                _currentTime -= TargetTime;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _currentTime = 0.0f;
        }
    }
}

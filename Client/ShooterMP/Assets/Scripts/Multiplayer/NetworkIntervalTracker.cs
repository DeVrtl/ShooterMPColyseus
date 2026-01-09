using System.Collections.Generic;
using UnityEngine;

namespace ShooterMP.Multiplayer
{
    public class NetworkIntervalTracker
    {
        private const int IntervalSamplesCount = 5;
        
        private float _lastReceiveTime = 0f;
        private List<float> _receiveTimeIntervals = new(IntervalSamplesCount);
        private float _cachedAverageInterval = 0f;
        
        public float AverageInterval => _cachedAverageInterval;

        public NetworkIntervalTracker()
        {
            for (int i = 0; i < IntervalSamplesCount; i++)
            {
                _receiveTimeIntervals.Add(0f);
            }
        }

        public void RecordReceiveTime()
        {
            float interval = Time.time - _lastReceiveTime;
            _lastReceiveTime = Time.time;
            
            _receiveTimeIntervals.Add(interval);
            _receiveTimeIntervals.RemoveAt(0);
            
            float sum = 0f;
            for (int i = 0; i < _receiveTimeIntervals.Count; i++)
            {
                sum += _receiveTimeIntervals[i];
            }
            _cachedAverageInterval = sum / _receiveTimeIntervals.Count;
        }
    }
}


using System;
using System.Collections.Generic;
using GoodLuckValley.Core.Input.Interfaces;

namespace GoodLuckValley.Tests.EditMode.Input
{
    /// <summary>                                                                     
    /// Controllable time provider for advancing time in input buffer tests.             
    /// Replaces Time.time so tests don't depend on real frame timing.                 
    /// </summary>                                                                       
    public class ManualClock
    {
        private float _currentTime;

        public Func<float> TimeProvider => () => _currentTime;

        public ManualClock(float startTime = 0f) => _currentTime = startTime;

        public void Advance(float seconds) => _currentTime += seconds;
    }

    /// <summary>
    /// Records which map-switching methods were called for verifying
    /// context-switch adapter notifications in tests.
    /// </summary>
    public class StubInputMapSwitcher : IInputMapSwitcher
    {
        private readonly List<string> _calls = new List<string>();

        public IReadOnlyList<string> Calls => _calls;
        public string LastCall => _calls.Count > 0 ? _calls[^1] : null;

        public void EnablePlayerMap() => _calls.Add("EnablePlayerMap");

        public void EnableUIMap() => _calls.Add("EnableUIMap");

        public void Reset() => _calls.Clear();
    }
}
using System.Diagnostics;

namespace Novae.Core
{
    public static class Time
    {
        #region Internal Data
        private static Stopwatch _gameTimer;
        private static TimeSpan _accumulatedElapsedTime;
        private static long _previousTicks = 0;

        private static TimeSpan _totalGametime;
        private static TimeSpan _elapsedGametime;
        #endregion

        #region Internal API
        internal static void Calculate()
        {
            if(_gameTimer == null)
            {
                _gameTimer = new Stopwatch();
                _gameTimer.Start();
            }

            long currentTicks = _gameTimer.Elapsed.Ticks;
            _accumulatedElapsedTime += TimeSpan.FromTicks(currentTicks - _previousTicks);
            _previousTicks = currentTicks;

            _elapsedGametime = _accumulatedElapsedTime;
            _totalGametime += _accumulatedElapsedTime;

            _accumulatedElapsedTime = TimeSpan.Zero;    
        }
        #endregion

        #region Public Data
        public static float DeltaTime => (float)_elapsedGametime.TotalSeconds;

        public static float TotalTime => (float)_totalGametime.TotalSeconds;

        public static float FixedDeltaTime { get; set; } = 1.0f / 60.0f;
        #endregion
    }
}

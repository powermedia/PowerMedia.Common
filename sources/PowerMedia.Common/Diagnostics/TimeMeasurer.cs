using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Timers; 
using System.Runtime.Serialization;

namespace PowerMedia.Common.Diagnostics
{
    [global::System.Serializable]
    public class MensurationIncompleteException : Exception
    {

        public MensurationIncompleteException() { }
        public MensurationIncompleteException(string message) : base(message) { }
        public MensurationIncompleteException(string message, Exception inner) : base(message, inner) { }
        protected MensurationIncompleteException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }

    public class TimeMeasurer : IDisposable
    {
        private readonly long UpdateAverageEveryCount;
        private readonly TimeSpan UpdateAverageEverySpan;
        
        private Stopwatch _watch;
        private Stopwatch Watch
        {
            get
            {
                if (_watch == null)
                {
                    _watch = new Stopwatch();
                }

                return _watch;
            }
        }

        private DateTime? _lastEvaluated;
        private DateTime LastEvaluated
        {
            get
            {
                if (_lastEvaluated.HasValue == false)
                {
                    _lastEvaluated = DateTime.Now;
                }

                return _lastEvaluated.Value;
            }
        }

        private List<double> _samples;
        private List<double> Samples
        {
            get
            {
                if (_samples == null)
                {
                    _samples = new List<double>((int)UpdateAverageEveryCount);
                }

                return _samples;
            }
        }

        public TimeMeasurer()
            : this(1, TimeSpan.FromSeconds(1))
        {
        }

        public TimeMeasurer(long updateAverageEveryCount, TimeSpan updateAverageEveryTimeSpan)
        {
            UpdateAverageEveryCount = updateAverageEveryCount;
            UpdateAverageEverySpan = updateAverageEveryTimeSpan;
            Reset();
        }

        public void Start()
        {
            Watch.Reset();
            Watch.Start();
        }

        private TimeSpan TimeFromLastUpdate
        {
            get
            {
                return DateTime.Now - LastEvaluated;
            }
        }

        public void Stop()
        {
            if (Watch.IsRunning == false)
            {
                return;
            }

            Watch.Stop();

            Samples.Add(Watch.Elapsed.TotalMilliseconds);

            if (Samples.Count >= UpdateAverageEveryCount || TimeFromLastUpdate > UpdateAverageEverySpan)
            {
                ComputeAverageTime();
            }            
        }

        private void ComputeAverageTime()
        {
            AverageTime = Samples.Average();
            Samples.Clear();
            _lastEvaluated = DateTime.Now;
        }

        public void Reset()
        {
            _watch = null;

            AverageTime = null;
            Samples.Clear();
            _lastEvaluated = null;
        }

        public double? AverageTime { get; private set; }

        void IDisposable.Dispose()
        {
            Stop();
        }
    }
}

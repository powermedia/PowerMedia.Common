using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Runtime.Remoting.Contexts;

namespace PowerMedia.Common.Diagnostics
{
    /// <summary>
    /// This class is threadsafe
    /// </summary>
    public class ResultCollectingStopwatch : IDisposable
    {
        private Stopwatch _watch = new Stopwatch();

        private object _resultsLockObject = new object();

        private List<TimeSpan> _results = new List<TimeSpan>(1000);
        public ReadOnlyCollection<TimeSpan> Results
        {
            get
            {
                return _results.AsReadOnly();
            }
        }

        public ReadOnlyCollection<TimeSpan> GetResultsSnapshot()
        {
            lock (_resultsLockObject)
            {
                var resultsCopy = new List<TimeSpan>(_results);

                if (_watch.IsRunning == true)
                {
                    resultsCopy.Add(_watch.Elapsed);
                }

                return resultsCopy.AsReadOnly();
            }
        }

        public void ResetResults()
        {
            lock (_resultsLockObject)
            {
                _results.Clear();
            }
        }

        public void Start()
        {
            _watch.Reset();
            _watch.Start();            
        }

        /// <summary>
        /// Stops the stopwatch without registering measured result
        /// </summary>
        public void StopDontRegisterResult()
        {
            _watch.Stop();
        }

        /// <summary>
        /// Calls the Stop() method
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        /// <summary>
        /// Stops the stopwatch and registers the result
        /// </summary>
        public void Stop()
        {
            if (_watch.IsRunning == false)
            {
                return;
            }

            StopDontRegisterResult();

            lock (_resultsLockObject)
            {
                _results.Add(_watch.Elapsed);
            }
        }
    }
}

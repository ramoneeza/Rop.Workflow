using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rop.Wokflow
{
    //Defer part is about uncouple operations (Mostly on UI Threat)
    //Workflow waits until Defer is signaled as handled.
    public partial class BaseWorkflow
    {
        private readonly ConcurrentQueue<IDeferArgs> _currentdefers = new ConcurrentQueue<IDeferArgs>();
        internal AutoResetEvent DeferSignal { get; } = new AutoResetEvent(false);
        
        /// <summary>
        /// External Routine Can try get a pending Defer
        /// </summary>
        /// <param name="defer"></param>
        /// <returns></returns>
        public bool TryGetDefer(out IDeferArgs? defer)
        {
            return _currentdefers.TryDequeue(out defer);
        }
        /// <summary>
        /// External routine can wait for a Defer with timeout
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="defer"></param>
        /// <returns></returns>
        public bool WaitDefer(int timeout, out IDeferArgs? defer)
        {
            if (TryGetDefer(out defer)) return true;
            defer = null;
            if (!DeferSignal.WaitOne(timeout)) return false;
            return TryGetDefer(out defer);
        }
        /// <summary>
        /// External routine can wait for a Defer with timeout and cancellation token
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="ct"></param>
        /// <param name="defer"></param>
        /// <returns></returns>
        public bool WaitDefer(int timeout,CancellationToken ct, out IDeferArgs? defer)
        {
            if (TryGetDefer(out defer)) return true;
            defer = null;
            if (!DeferSignal.WaitOne(timeout,ct)) return false;
            return TryGetDefer(out defer);
        }
        /// <summary>
        /// External routine locked waiting for a Defer
        /// </summary>
        public bool WaitDefer(CancellationToken ct, out IDeferArgs? defer)
        {
            return WaitDefer(Timeout.Infinite, ct, out defer);
        }

        /// <summary>
        /// Internal use only. Put a Defer and signal that a defer is available
        /// </summary>
        /// <param name="defer"></param>
        internal void PushDefer(IDeferArgs defer)
        {
            _currentdefers.Enqueue(defer); 
            DeferSignal.Set();
        } 
        /// <summary>
        /// STEP only use.
        /// Launch a Defer inside STEP and Wait to completed.
        /// </summary>
        /// <param name="deferType"></param>
        /// <param name="ct"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected bool LaunchAndWaitDefer(string deferType,CancellationToken ct, object? parameter=null)
        {
            var defer = new DeferArgs(this, deferType, parameter);
            PushDefer(defer);
            if (!defer.IsCompleted && !CancellationToken.IsCancellationRequested)
            {
                defer.Signal.WaitOne(ct);
            }
            return defer.IsCompleted;
        }
    }
}

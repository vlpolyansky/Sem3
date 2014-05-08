using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Sem3.Contracts;

namespace Sem3.Server
{
    public class MyThreadPool
    {
        private QueryQueue _queue = new QueryQueue();
        private bool _working = true;
        private List<Thread> _threads = new List<Thread>();

        public MyThreadPool(int threads) {
            for (int i = 0; i < threads; ++i)
            {
                Thread t = new Thread(new ThreadStart(work));
                _threads.Add(t);
                t.Start();
            }
        }

        private void work()
        {
            while (true)
            {
                ICallbackService owner;
                long value;

                Monitor.Enter(_queue);
                if (!_working)
                {
                    Monitor.Exit(_queue);
                    return;
                }
                if (_queue.isEmpty())
                {
                    Monitor.Wait(_queue);
                }
                if (!_working)
                {
                    Monitor.Exit(_queue);
                    return;
                }
                _queue.pollQuery(out owner, out value);
                Monitor.Exit(_queue);

                long answer = Sem3.Core.Algorithms.isPrime(value);
                owner.ReturnPrimality(answer);
            }
        }

        public void Stop()
        {
            Monitor.Enter(_queue);
            _working = false;
            Monitor.PulseAll(_queue);
            Monitor.Exit(_queue);
            for (int i = 0; i < _threads.Count; ++i)
            {
                _threads[i].Join();
            }
        }

        public void AddQuery(ICallbackService owner, long value, bool vip)
        {
            Monitor.Enter(_queue);
            _queue.putQuery(owner, value, vip);
            Monitor.Pulse(_queue);
            Monitor.Exit(_queue);
        }
    }
}

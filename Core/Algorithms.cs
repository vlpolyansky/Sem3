using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Sem3.Core
{
	public class Algorithms
	{

		private static List<long> _primes = new List<long>();
		private static int _lastCounted = 2;
		private static Mutex _mutex = new Mutex();

		static Algorithms()
		{
			_primes.Add(2);
		}

		public static long isPrime(long value)
		{
			_mutex.WaitOne();
			while (_lastCounted < value && _primes.Count < 1e5)
			{
				int i = _lastCounted + 1;
				bool flag = true;
				for (int j = 0; j < _primes.Count && _primes[j] * _primes[j] <= i; ++j)
				{
					if (i % _primes[j] == 0)
					{
						flag = false;
					break;
					}
				}
				if (flag)
				{
					_primes.Add(i);
				}
				_lastCounted = i;
			}
			_mutex.ReleaseMutex();
			if (_lastCounted < value)
			{
				for (int i = 0; i < _primes.Count && _primes[i] * _primes[i] <= value; ++i)
				{
					if (value % _primes[i] == 0)
					{
						return _primes[i];
					}
				}
				for (long j = _primes[_primes.Count - 1]; j * j <= value; ++j)
				{
					if (value % j == 0)
					{
						return j;
					}
				}
				return 1;
			}
			int l = 0;
			int r = _primes.Count;
			while (r - l > 1)
			{
				int m = (l + r) / 2;
				if (_primes[m] <= value)
				{
					l = m;
				}
				else
				{
					r = m;
				}
			}
			return _primes[l] == value ? 1 : 0;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sem3.Contracts;

namespace Sem3.Server
{
	using QueryPair = KeyValuePair<ICallbackService, long>;

	public class QueryQueue
	{
		private Queue<QueryPair> 
			_vipQueue = new Queue<QueryPair>(),
			_regularQueue = new Queue<QueryPair>();

		public void putQuery(ICallbackService owner, long value, bool vip)
		{
			(vip ? _vipQueue : _regularQueue).Enqueue(new QueryPair(owner, value));
		}

		public void pollQuery(out ICallbackService owner, out long value)
		{
			QueryPair qp = _vipQueue.Count != 0 ? _vipQueue.Dequeue() : _regularQueue.Dequeue();
			owner = qp.Key;
			value = qp.Value;
		}

		public bool isEmpty()
		{
			return _vipQueue.Count == 0 && _regularQueue.Count == 0;
		}


	}
}

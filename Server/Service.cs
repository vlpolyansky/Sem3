using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;

using Sem3.Contracts;

namespace Sem3.Server
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
	public class Service : Sem3.Contracts.IService
	{
        private static int _requests = 0;

		public void PrimalityRequest(long value, bool vip)
		{
			Console.WriteLine("[" + (++_requests) + "] Request with number " + value + (vip ? " [vip] " : " ") + "accepted");
			var callback = OperationContext.Current.GetCallbackChannel<ICallbackService>();
          ServerProgram.ThreadPool.AddQuery(callback, value, vip);
		}
	}
}

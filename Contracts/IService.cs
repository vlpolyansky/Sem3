using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Sem3.Contracts
{
	[ServiceContract(CallbackContract = typeof(ICallbackService))]
	public interface IService
	{
		[OperationContract]
		void PrimalityRequest(long value, bool vip);
	}

}

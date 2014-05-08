using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Sem3.Contracts
{
	[ServiceContract]
	public interface ICallbackService
	{
		[OperationContract]
		void ReturnPrimality(long answer);
	}
}

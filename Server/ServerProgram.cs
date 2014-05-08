using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;

using Sem3.Contracts;

namespace Sem3.Server
{
	public class ServerProgram
	{
        public static MyThreadPool ThreadPool {get; private set; }
        private ServiceHost _host;

        public ServerProgram()
        {
            _host = new ServiceHost(typeof(Service));
            _host.AddServiceEndpoint(
                typeof(IService),
                new NetTcpBinding(),
                "net.tcp://localhost:9000/MyEndpoint");
        }

        public void Start(int threads)
        {
            _host.Open();
            ThreadPool = new MyThreadPool(threads);
        }

        public void Stop()
        {
            ThreadPool.Stop();
//            _host.Abort();
            _host.Close();
        }

        static void Main(string[] args)
        {
            ServerProgram server = new ServerProgram();
            server.Start(THREADS);
            Console.WriteLine("Server started");
            Console.ReadKey();
            server.Stop();

        }

		public const int THREADS = 4;
	}
}

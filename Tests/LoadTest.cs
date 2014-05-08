using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sem3.Tests
{
    [TestClass]
    public class LoadTest
    {
        private static Random _random = new Random(23);
        private static Exception _exception = null;
        private static int _ok = 0;

        [TestMethod, Timeout(60000)]
        public void LoadTestMethod()
        {
            int[] questions = new int[] {1, 2, 3, 4, 5, 6, 7, 98, 100, 169, 239017};
            bool[] answers = new bool[] {false, true, true, false, true, false, true, false, false, false, true};
            int testClients = 300;
            Server.ServerProgram server = new Server.ServerProgram();
            try
            {
                server.Start(4);
                List<TestClient> clients = new List<TestClient>(testClients);
                for (int i = 0; i < testClients; ++i)
                {
                    int idx = _random.Next(questions.Length - 1);
                    var client = new TestClient(questions[idx], answers[idx]);
                    clients.Add(client);
                    client.Request();
                }
                foreach (var client in clients)
                {
                    client.Join();
                }
                foreach (var client in clients)
                {
                    Assert.IsTrue(client.GetResult());
                }
            }
            finally
            {
                server.Stop();
            }
            Trace.Write("finished = " + _ok);
            if (_exception != null)
            {
                throw new AssertFailedException("Exception in one of client threads", _exception);
            }
        }

        public class TestClient : Contracts.ICallbackService
        {
            public long _question;
            public bool _answer;
            private bool _result = false;
            private ManualResetEvent _event = new ManualResetEvent(false);
            private Contracts.IService _proxy;
            private Thread _thread;

            public TestClient(long q, bool a)
            {
                _thread = new Thread(new ThreadStart(ThreadMethod));
                _question = q;
                _answer = a;
                _proxy = DuplexChannelFactory<Contracts.IService>.CreateChannel(this, new NetTcpBinding(),
                        new EndpointAddress("net.tcp://localhost:9000/MyEndpoint"));
            }

            public void Request()
            {
                _thread.Start();
            }

            private void ThreadMethod()
            {
                try
                {
                    _proxy.PrimalityRequest(_question, (_random.Next() % 2) == 1);
                    _ok++;
                }
                catch (Exception ex)
                {
                    _exception = ex;
                }
            }

            public void ReturnPrimality(long answer)
            {
                _result = (!_answer) ^ (answer == 1);
                Console.WriteLine("Got answer");
                _event.Set();
            }

            public void Join()
            {
                _thread.Join();
                _event.WaitOne();
            }

            public bool GetResult()
            {
                return _result;
            }
        }
    }
}

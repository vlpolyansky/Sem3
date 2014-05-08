using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ServiceModel;
using System.Threading;

namespace MvcClient.Controllers
{
    public class HomeController : Controller, Sem3.Contracts.ICallbackService
    {
        //
        // GET: /Home/

        private AutoResetEvent _event = new AutoResetEvent(false);

        public ActionResult Index()
        {
            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        public ActionResult Index(string NumberInput)
        {
            ViewBag.Number = NumberInput;
            ViewBag.Message = "Waiting...";
            _sendRequest(NumberInput);
            _event.WaitOne();
            return View();
        }

        private void _sendRequest(string text)
        {
            string value = text;
            long valueLong = 0;
            if (!long.TryParse(value, out valueLong))
            {
                ViewBag.Message = "Not a number";
                _event.Set();
                return;
            }
            try
            {
                var _proxy = DuplexChannelFactory<Sem3.Contracts.IService>.CreateChannel(this, new NetTcpBinding(),
                        new EndpointAddress("net.tcp://localhost:9000/MyEndpoint"));
                _proxy.PrimalityRequest(valueLong, false);
                ViewBag.Message = "Waiting for answer...";
            }
            catch (EndpointNotFoundException)
            {
                ViewBag.Message = "Couldn't connect to server";
                _event.Set();
                return;
            }
        }

        public void ReturnPrimality(long answer)
        {
            ViewBag.Message = answer == 0 ? "Number is not prime" :
                    (answer == 1 ? "Number is prime" : "Number is divided by " + answer);
            _event.Set();
        }

    }
}

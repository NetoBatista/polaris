using Microsoft.AspNetCore.Mvc;
using Polaris.Controllers;
using System.Net;

namespace Polaris.Test.Controller
{
    [TestClass]
    public class PingControllerTest
    {
        [TestMethod("Should be able ping")]
        public void Ping()
        {
            var controller = new PingController();
            var response = controller.Get();
            var result = (OkResult)response;
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.OK);
        }
    }
}

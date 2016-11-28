using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViaGogoTicketInterview.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Http;

namespace ViaGogoTicketInterview.Controllers.Tests
{
    [TestClass()]
    public class ArtistControllerTests
    {
        [TestMethod()]
        public async Task IndexTest()
        {
            ArtistController controller = new ArtistController();
            var result = await controller.Index() as ViewResult;
            Assert.AreEqual("Index", result.ViewName);
        }
    }
}
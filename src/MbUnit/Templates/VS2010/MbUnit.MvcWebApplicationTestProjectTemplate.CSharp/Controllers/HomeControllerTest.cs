using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Gallio.Framework;
using MbUnit.Framework;
using $mvcprojectnamespace$;
using $mvcprojectnamespace$.Controllers;

namespace $safeprojectname$.Controllers
{
    /// <summary>
    /// Summary description for HomeControllerTest
    /// </summary>
    [TestFixture]
    public class HomeControllerTest
    {
        [Test]
        public void Index()
        {
            // Setup
            HomeController controller = new HomeController();

            // Execute
            ViewResult result = controller.Index() as ViewResult;

            // Verify
            ViewDataDictionary viewData = result.ViewData as ViewDataDictionary;

            Assert.AreEqual("Home Page", viewData["Title"]);
            Assert.AreEqual("Welcome to ASP.NET MVC!", viewData["Message"]);
        }

        [Test]
        public void About()
        {
            // Setup
            HomeController controller = new HomeController();

            // Execute
            ViewResult result = controller.About() as ViewResult;

            // Verify
            ViewDataDictionary viewData = result.ViewData as ViewDataDictionary;

            Assert.AreEqual("About Page", viewData["Title"]);
        }
    }
}

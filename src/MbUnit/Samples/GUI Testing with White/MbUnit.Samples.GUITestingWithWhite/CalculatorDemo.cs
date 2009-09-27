using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core;
using Core.Factory;
using Core.UIItems;
using Core.UIItems.WindowItems;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Samples.GUITestingWithWhite.Framework;
using Repository;

namespace MbUnit.Samples.GUITestingWithWhite
{
    /// <summary>
    /// Demonstrates the use of White with MbUnit by automating the Windows
    /// Calculator application.
    /// </summary>
    public class CalculatorDemo : GUITestFixture
    {
        public override Application LaunchApplication()
        {
            return Application.Launch(Path.Combine(Environment.SystemDirectory, "calc.exe"));
        }

        /// <summary>
        /// Performs a calculation which we pretend returns an incorrect result.
        /// The base test fixture class captures a screen recording.
        /// </summary>
        [Test]
        public void FailedCalculation()
        {
            var screen = ScreenRepository.Get<CalculatorScreen>("Calculator", InitializeOption.NoCache);

            screen.ClickDigits("23");
            screen.Minus.Click();
            screen.ClickDigits("16");
            screen.Times.Click();
            screen.ClickDigits("7");
            screen.Equal.Click();

            Assert.AreEqual("43", screen.Result.Text);
        }
    }
}

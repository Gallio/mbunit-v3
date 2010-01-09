// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Core.UIItems;
using Core.UIItems.WindowItems;
using Core.UIItems.WindowStripControls;
using Repository.ScreenAttributes;
using Repository;

namespace MbUnit.Samples.GUITestingWithWhite
{
    public class CalculatorScreen : AppScreen
    {
        [AutomationId("MenuBar")]
        public MenuBar MenuBar;

        [AutomationId("403")]
        public TextBox Result;

        [AutomationId("83")]
        public Button Backspace;

        [AutomationId("82")]
        public Button ClearEntry;

        [AutomationId("81")]
        public Button Clear;

        [AutomationId("113")]
        public Button MemoryClear;

        [AutomationId("131")]
        public Button Digit7;

        [AutomationId("132")]
        public Button Digit8;

        [AutomationId("133")]
        public Button Digit9;

        [AutomationId("90")]
        public Button Divide;

        [AutomationId("103")]
        public Button SquareRoot;

        [AutomationId("114")]
        public Button MemoryRecall;

        [AutomationId("128")]
        public Button Digit4;

        [AutomationId("129")]
        public Button Digit5;

        [AutomationId("130")]
        public Button Digit6;

        [AutomationId("91")]
        public Button Times;

        [AutomationId("109")]
        public Button Percent;

        [AutomationId("115")]
        public Button MemorySave;

        [AutomationId("125")]
        public Button Digit1;

        [AutomationId("126")]
        public Button Digit2;

        [AutomationId("127")]
        public Button Digit3;

        [AutomationId("93")]
        public Button Minus;

        [AutomationId("80")]
        public Button Invert;

        [AutomationId("116")]
        public Button MemoryPlus;

        [AutomationId("124")]
        public Button Digit0;

        [AutomationId("80")]
        public Button Negate;

        [AutomationId("85")]
        public Button Period;

        [AutomationId("92")]
        public Button Plus;

        [AutomationId("112")]
        public Button Equal;

        protected CalculatorScreen()
        {
        }

        public CalculatorScreen(Window window, ScreenRepository screenRepository)
            : base(window, screenRepository)
        {
        }

        public virtual void ClickDigits(string digits)
        {
            foreach (char ch in digits)
            {
                switch (ch)
                {
                    case '0':
                        Digit0.Click();
                        break;
                    case '1':
                        Digit1.Click();
                        break;
                    case '2':
                        Digit2.Click();
                        break;
                    case '3':
                        Digit3.Click();
                        break;
                    case '4':
                        Digit4.Click();
                        break;
                    case '5':
                        Digit5.Click();
                        break;
                    case '6':
                        Digit6.Click();
                        break;
                    case '7':
                        Digit7.Click();
                        break;
                    case '8':
                        Digit8.Click();
                        break;
                    case '9':
                        Digit9.Click();
                        break;
                    case '.':
                        Period.Click();
                        break;
                }
            }
        }
    }
}

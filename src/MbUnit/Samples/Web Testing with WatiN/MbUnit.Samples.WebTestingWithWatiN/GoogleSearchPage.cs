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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatiN.Core;

namespace MbUnit.Samples.WebTestingWithWatiN
{
    /// <summary>
    /// The Google search page.
    /// </summary>
    [Page(UrlRegex = "http://www.google.com/.*")]
    public class GoogleSearchPage : Page
    {
        [FindBy(Name = "q")]
        public TextField QueryTextField;

        [FindBy(Name = "btnG")]
        public Button SearchButton;

        [FindBy(Name = "btnI")]
        public Button ImFeelingLuckyButton;

        public static void GoTo(Browser browser)
        {
            browser.GoTo("http://www.google.com");
        }

        public void Search(string query)
        {
            QueryTextField.TypeText(query);
            SearchButton.Click();
        }
    }
}

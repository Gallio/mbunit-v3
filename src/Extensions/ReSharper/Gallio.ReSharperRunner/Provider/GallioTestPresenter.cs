// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

using System.Drawing;
using Gallio.Model;
using JetBrains.CommonControls;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.UI.TreeView;

#if RESHARPER_31
using JetBrains.ReSharper.TreeModelBrowser;
using JetBrains.Util.DataStructures.TreeModel;
#elif RESHARPER_40 || RESHARPER_41
using JetBrains.TreeModels;
using JetBrains.ReSharper.CodeView.TreePsiBrowser;
#else
using JetBrains.TreeModels;
using JetBrains.ReSharper.Features.Common.TreePsiBrowser;
#endif

namespace Gallio.ReSharperRunner.Provider
{
    public class GallioTestPresenter : TreeModelBrowserPresenter
    {
        public GallioTestPresenter()
        {
            Present<GallioTestElement>(PresentTestElement);
        }

        private void PresentTestElement(GallioTestElement value, IPresentableItem item,
            TreeModelNode modelNode, PresentationState state)
        {
            item.Clear();

            ITest test = value.Test;

            item.RichText = test.Name;

            if (value.IsExplicit)
                item.RichText.SetForeColor(SystemColors.GrayText);

            Image image = UnitTestManager.GetStateImage(state);

            if (image == null)
                image = UnitTestManager.GetStandardImage(test.IsTestCase ? UnitTestElementImage.Test : UnitTestElementImage.TestCategory);

            if (image != null)
                item.Images.Add(image);

            if (! test.IsTestCase)
                AppendOccurencesCount(item, modelNode, "test");
        }
    }
}
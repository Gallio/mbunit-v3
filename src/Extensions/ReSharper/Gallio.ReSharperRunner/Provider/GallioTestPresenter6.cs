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

using System.Drawing;
using JetBrains.CommonControls;
using JetBrains.ReSharper.Features.Common.TreePsiBrowser;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.UI.TreeView;
using JetBrains.TreeModels;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.UI;

namespace Gallio.ReSharperRunner.Provider
{
    [UnitTestPresenter]
    public class GallioTestPresenter : TreeModelBrowserPresenter, IUnitTestPresenter
    {
        public GallioTestPresenter()
        {
            Present<GallioTestElement>(PresentTestElement);
        }

        private static void PresentTestElement(GallioTestElement value, IPresentableItem item, TreeModelNode modelNode, PresentationState state)
        {
            item.Clear();

            item.RichText = value.TestName;

            if (value.Explicit)
                item.RichText.SetForeColor(SystemColors.GrayText);

            var typeImage = UnitTestIconManager.GetStandardImage(UnitTestElementImage.Test);
            var stateImage = UnitTestIconManager.GetStateImage(state);
            if (stateImage != null)
            {
                item.Images.Add(stateImage);
            }
            else if (typeImage != null)
            {
                item.Images.Add(typeImage);
            }
        }

        public void Present(IUnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state)
        {
            if (element is GallioTestElement)
            {
                UpdateItem(element, node, item, state);
            }
        }
    }
}
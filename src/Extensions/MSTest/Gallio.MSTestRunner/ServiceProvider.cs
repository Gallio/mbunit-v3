// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.ComponentModel.Design;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Vsip;

namespace Gallio.MSTestRunner
{
    internal sealed class ServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider inner;
        private DTE2 dte;
        private ITestManagement testManagement;
        private GallioTip tip;
        private GallioTuip tuip;

        public ServiceProvider(IServiceProvider inner)
        {
            this.inner = inner;

            RegisterServices();
        }

        public DTE2 DTE
        {
            get
            {
                if (dte == null)
                    dte = GetService<DTE2>(typeof(SDTE));
                return dte;
            }
        }

        public ITestManagement TestManagement
        {
            get
            {
                if (testManagement == null)
                    testManagement = GetService<ITestManagement>(typeof(STestManagement));
                return testManagement;
            }
        }

        public ITmi Tmi
        {
            get { return TestManagement.TmiInstance; }
        }

        public GallioTip Tip
        {
            get
            {
                if (tip == null)
                    tip = (GallioTip)Tmi.FindTipForTestType(Guids.GallioTestType);
                return tip;
            }
        }

        public GallioTuip Tuip
        {
            get
            {
                if (tuip == null)
                    tuip = GetService<GallioTuip>(typeof(SGallioTestService));
                return tuip;
            }
        }

        public object GetService(Type serviceType)
        {
            return inner.GetService(serviceType);
        }

        public T GetService<T>(Type serviceInterface)
        {
            return (T)GetService(serviceInterface);
        }

        private void RegisterServices()
        {
            IServiceContainer container = GetService<IServiceContainer>(typeof(IServiceContainer));
            if (container != null)
            {
                ServiceCreatorCallback callback = OnCreateService;
                container.AddService(typeof(SGallioTestService), callback, true);
            }
        }

        private object OnCreateService(IServiceContainer container, Type serviceType)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            if (serviceType == typeof(SGallioTestService))
                return new GallioTuip(this);
            else
                return null;
        }
    }
}

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

using System.Collections.Generic;
using JetBrains.ReSharper.TaskRunnerFramework.UnitTesting;

namespace Gallio.ReSharperRunner.Provider.TestElements
{
    /// <summary>
    /// Represents a Gallio test.
    /// </summary>
    public abstract class GallioTestElementBase : IUnitTestElement
    {
    	protected IList<IUnitTestElement> children;
    	
    	protected GallioTestElementBase(IUnitTestRunnerProvider provider, string id, IUnitTestElement parent)
        {
    		Provider = provider;
    		Id = id;
    		Parent = parent;
    		children = new List<IUnitTestElement>();
        }

    	public abstract bool Equals(IUnitTestElement other);
    	public abstract void Invalidate();
    	public abstract IList<UnitTestTask> GetTaskSequence(IEnumerable<IUnitTestElement> explicitElements);
    	
		public string Id { get; private set; }
    	
		public IUnitTestRunnerProvider Provider { get; private set; }
    	
		public IUnitTestElement Parent { get; set; }

    	public ICollection<IUnitTestElement> Children
    	{
    		get { return children; }
    	}

    	public abstract string ShortName { get; }
    	
		public virtual bool Explicit
    	{
    		get { return ExplicitReason != null; }
    	}

    	public virtual string ExplicitReason { get; set; }

    	public UnitTestElementState State { get; set; }

		public abstract bool Valid { get; set; }

		public IEnumerable<UnitTestElementCategory> Categories { get; set; }

		public override string ToString()
		{
			return Id;
		}
    }
}

// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal abstract class PsiWrapper<TTarget>
        where TTarget : class
    {
        private readonly PsiReflector reflector;
        private readonly TTarget target;

        public PsiWrapper(PsiReflector reflector, TTarget target)
        {
            if (reflector == null)
                throw new ArgumentNullException("reflector");
            if (target == null)
                throw new ArgumentNullException("target");

            this.reflector = reflector;
            this.target = target;
        }

        public PsiReflector Reflector
        {
            get { return reflector; }
        }

        public TTarget Target
        {
            get { return target; }
        }

        public override int GetHashCode()
        {
            return target.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            PsiWrapper<TTarget> other = obj as PsiWrapper<TTarget>;
            return other != null && target.Equals(other.target);
        }
    }
}
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

using System;
using System.Collections.Generic;
using Gallio.Model;
using Gallio.ReSharperRunner.Reflection;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Provider
{
    /// <summary>
    /// Describes an annotation associated with a declared element.
    /// </summary>
    internal class AnnotationState
    {
        private readonly AnnotationType type;
        private readonly string message;
        private readonly string details;
        private readonly IDeclaredElementResolver declaredElementResolver;

        private AnnotationState(AnnotationType type, string message, string details,
            IDeclaredElementResolver declaredElementResolver)
        {
            this.type = type;
            this.message = message;
            this.details = details;
            this.declaredElementResolver = declaredElementResolver;
        }

        public static AnnotationState CreateFromAnnotation(Annotation annotation)
        {
            return new AnnotationState(annotation.Type,
                annotation.Message, annotation.Details,
                ReSharperReflectionPolicy.GetDeclaredElementResolver(annotation.CodeElement));
        }

        public AnnotationType Type
        {
            get { return type; }
        }

        public string Message
        {
            get { return message; }
        }

        public string Details
        {
            get { return details; }
        }

        public IDeclaredElement GetDeclaredElement()
        {
            return declaredElementResolver.ResolveDeclaredElement();
        }
    }
}

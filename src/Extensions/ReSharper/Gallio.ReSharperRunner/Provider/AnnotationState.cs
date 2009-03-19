using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

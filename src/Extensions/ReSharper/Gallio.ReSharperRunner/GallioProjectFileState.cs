using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gallio.Model;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace Gallio.ReSharperRunner
{
    /// <summary>
    /// Provides state information to the <see cref="GallioDaemonStageProcess" />
    /// when associated with a <see cref="IProjectFile" />.  This allows the <see cref="GallioTestProviderDelegate" />
    /// to feed annotation information to the daemon for use in generating custom highlights.
    /// </summary>
    internal class GallioProjectFileState
    {
        private static readonly Key<GallioProjectFileState> key = new Key<GallioProjectFileState>(typeof(GallioProjectFileState).Name);
        private readonly IList<Annotation> annotations;

        /// <summary>
        /// Creates a state object with the specified annotations. 
        /// </summary>
        /// <param name="annotations">The annotations</param>
        public GallioProjectFileState(IList<Annotation> annotations)
        {
            this.annotations = new ReadOnlyCollection<Annotation>(annotations);
        }

        /// <summary>
        /// Sets the state associated with a file.
        /// </summary>
        /// <param name="file">The file</param>
        /// <param name="state">The associated state, or null if none</param>
        public static void SetFileState(IProjectFile file, GallioProjectFileState state)
        {
            file.PutData(key, state);
        }

        /// <summary>
        /// Gets the state associated with a file.
        /// </summary>
        /// <param name="file">The file</param>
        /// <returns>The associated state, or null if none</returns>
        public static GallioProjectFileState GetFileState(IProjectFile file)
        {
            return file.GetData(key);
        }

        /// <summary>
        /// Gets a readonly list of annotations.
        /// </summary>
        public IList<Annotation> Annotations
        {
            get { return annotations; }
        }
    }
}

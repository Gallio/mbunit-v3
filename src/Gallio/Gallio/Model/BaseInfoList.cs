// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Collections;
using System.Collections.Generic;
using Gallio.Collections;

namespace Gallio.Model
{
    /// <summary>
    /// The abstract base class of a list that wraps model objects
    /// with their corresponding reflection types derived from <see cref="BaseInfo" />.
    /// </summary>
    /// <typeparam name="TModel">The model object type</typeparam>
    /// <typeparam name="TInfo">The reflection type</typeparam>
    public abstract class BaseInfoList<TModel, TInfo> : IList<TInfo>
        where TModel : class where TInfo : BaseInfo, TModel
    {
        private readonly IList<TModel> inputList;

        /// <summary>
        /// Creates a wrapper for the specified input list of model objects.
        /// </summary>
        /// <param name="inputList">The input list</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inputList"/> is null</exception>
        internal BaseInfoList(IList<TModel> inputList)
        {
            if (inputList == null)
                throw new ArgumentNullException(@"inputList");

            this.inputList = inputList;
        }

        /// <inheritdoc />
        public int Count
        {
            get { return inputList.Count; }
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <inheritdoc />
        public TInfo this[int index]
        {
            get { return Wrap(inputList[index]); }
        }

        /// <inheritdoc />
        public void CopyTo(TInfo[] array, int arrayIndex)
        {
            foreach (TInfo item in this)
                array[arrayIndex++] = item;
        }

        /// <inheritdoc />
        public int IndexOf(TInfo item)
        {
            TModel inputItem = Unwrap(item);
            return inputItem != null ? inputList.IndexOf(inputItem) : -1;
        }

        /// <inheritdoc />
        public bool Contains(TInfo item)
        {
            TModel inputItem = Unwrap(item);
            return inputItem != null && inputList.Contains(inputItem);
        }

        /// <inheritdoc />
        public IEnumerator<TInfo> GetEnumerator()
        {
            foreach (TModel item in inputList)
                yield return Wrap(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns a wrapper for the list of elements using the model object's type
        /// instead of the reflection object's type.
        /// </summary>
        /// <returns>The model list</returns>
        public IList<TModel> AsModelList()
        {
            return new CovariantList<TInfo, TModel>(this);
        }

        /// <summary>
        /// Wraps the specified input item.
        /// </summary>
        /// <param name="inputItem">The input item</param>
        /// <returns>The output item</returns>
        protected abstract TInfo Wrap(TModel inputItem);

        /// <summary>
        /// Unwraps the specified output item.
        /// </summary>
        /// <param name="infoItem">The output item</param>
        /// <returns>The corresponding input item or null if the output item is a valid wrapper</returns>
        protected static TModel Unwrap(TInfo infoItem)
        {
            if (infoItem == null)
                return null;
            return infoItem.Source as TModel;
        }

        void IList<TInfo>.Insert(int index, TInfo item)
        {
            throw new NotSupportedException();
        }

        void IList<TInfo>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        TInfo IList<TInfo>.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        void ICollection<TInfo>.Add(TInfo item)
        {
            throw new NotSupportedException();
        }

        void ICollection<TInfo>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<TInfo>.Remove(TInfo item)
        {
            throw new NotSupportedException();
        }
    }
}

﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* NonNullCollection.cs -- collection with items that can't be null
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// <see cref="Collection{T}"/> with items that can't be <c>null</c>.
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public class NonNullCollection<T>
        : Collection<T>
        where T : class
    {
        #region Collection<T> members

        /// <summary>
        /// Inserts an element into the 
        /// <see cref="Collection{T}"/>
        /// at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item 
        /// should be inserted.</param>
        /// <param name="item">The object to insert. The value can 
        /// be null for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.-or-index is greater than 
        /// <see cref="Collection{T}.Count"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item"/> is <c>null</c>.
        /// </exception>
        protected override void InsertItem
            (
                int index,
                T item
            )
        {
            Code.NotNull(item, "item");

            base.InsertItem(index, item);
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the 
        /// element to replace.</param>
        /// <param name="item">The new value for the element 
        /// at the specified index. The value can be null for reference types.
        /// </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.-or-index is greater 
        /// than <see cref="Collection{T}.Count"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item"/> is <c>null</c>.
        /// </exception>
        protected override void SetItem(int index, T item)
        {
            Code.NotNull(item, "item");

            base.SetItem(index, item);
        }

        #endregion

        #region Public members

        /// <summary>
        /// Converts the collection to <see cref="Array"/> of elements
        /// of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>Array of items of type <typeparamref name="T"/>.
        /// </returns>
        [NotNull]
        public T[] ToArray()
        {
            List<T> result = new List<T>(this);

            return result.ToArray();
        }

        /// <summary>
        /// Add several elements to the collection.
        /// </summary>
        [NotNull]
        public NonNullCollection<T> AddRange
            (
                [NotNull] IEnumerable<T> range
            )
        {
            Code.NotNull(range, "range");

            foreach (T item in range)
            {
                Add(item);
            }

            return this;
        }

        #endregion
    }
}

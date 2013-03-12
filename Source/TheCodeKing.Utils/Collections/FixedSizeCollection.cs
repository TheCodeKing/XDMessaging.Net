/*=============================================================================
*
*	(C) Copyright 2013, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expressed or implied.
*
*=============================================================================
*/
using System.Collections;

namespace TheCodeKing.Utils.Collections
{
    /// <summary>
    /// A buffer of first-in-last-out unique strings with a fixed size. New values push older
    /// values out of the queue.
    /// </summary>
    internal sealed class FixedSizeCollection : IEnumerable
    {
        #region Constants and Fields

        private readonly int maxSize;
        private readonly Queue queue;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of FixedSizeCollection, where maxSize is the
        /// maximum size of the buffer.
        /// </summary>
        /// <param name="maxSize"></param>
        public FixedSizeCollection(int maxSize)
        {
            this.maxSize = maxSize;
            queue = new Queue();
        }

        #endregion

        #region Properties

        public int Count
        {
            get { return queue.Count; }
        }

        #endregion

        #region Public Methods

        public void Clear()
        {
            lock (queue)
            {
                queue.Clear();
            }
        }

        public bool Contains(string item)
        {
            lock (queue)
            {
                return queue.Contains(item);
            }
        }

        /// <summary>
        /// Adds a unique value to the collection in a thread safe manner. Returns true on success
        /// or false on fail. 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryAddValue(string item)
        {
            lock (queue)
            {
                // if already contains the value return false
                if (queue.Contains(item))
                {
                    return false;
                }
                // add the value
                queue.Enqueue(item);
                // if the new collection size exceeds the max size
                // remove the first in
                if (queue.Count > maxSize)
                {
                    queue.Dequeue();
                }
                return true;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IEnumerable

        public IEnumerator GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        #endregion

        #endregion
    }
}
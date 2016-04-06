using System.Collections.Generic;

namespace Util
{
    public class FixedQueue<T> : Queue<T>
    {
        private int limit = -1;

        public int Limit
        {
            get { return limit; }
            set { limit = value; }
        }

        public FixedQueue(int limit) : base(limit)
        {
            this.Limit = limit;
        }

        public new void Enqueue(T item)
        {
            if (this.Count >= this.Limit)
            {
                this.Dequeue();
            }
            base.Enqueue(item);
        }
    }
}

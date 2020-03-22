using System.Collections.Concurrent;
using System.Threading;

namespace Assets.Scripts.Core
{
    public class IntDispenser
    {
        readonly ConcurrentStack<int> freeInts;
        int lastInt;

        public int LastInt => lastInt;

        public IntDispenser(int startInt)
        {
            freeInts = new ConcurrentStack<int>();

            lastInt = startInt;
        }

        public int GetFreeInt()
        {
            if (!freeInts.TryPop(out int freeInt))
            {
                freeInt = Interlocked.Increment(ref lastInt);
            }

            return freeInt;
        }

        public void ReleaseInt(int releasedInt) => freeInts.Push(releasedInt);
    }
}

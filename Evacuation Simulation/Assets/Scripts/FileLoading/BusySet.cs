using System;
using System.Collections.Generic;

namespace PedestrianSimulation.Import
{
    public class BusySet<T>
    {
        public HashSet<T> Items { get; }

        public event Action<bool> OnStatusChange;
        public bool IsBusy => Items.Count != 0;

        public BusySet()
        {
            Items = new HashSet<T>();
        }

        public BusySet(HashSet<T> item)
        {
            Items = item;
        }

        public bool AddItem(T item) => CommitTransatction(item, Items.Add);
        public bool RemoveItem(T item) => CommitTransatction(item, Items.Remove);

        private bool CommitTransatction(T item, Func<T,bool> action)
        {
            bool startState = IsBusy;
            bool b = action(item);
            if (IsBusy != startState)
            {
                OnStatusChange?.Invoke(IsBusy);
            }
            return b;
        }

    }
}

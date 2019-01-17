using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace Irixi_Aligner_Common.Classes.Base
{
    public class ObservableCollectionThreadSafe<T>: ObservableCollection<T>
    {
        /// <summary> 
        /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
        /// </summary> 
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            foreach (var i in collection)
                Items.Add(i);

            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                });
            }
            else
            {
                // may be running in unit test
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            
        }

        protected override void ClearItems()
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    base.ClearItems();
                });
            }
            else
            {
                // may be running in unit test
                base.ClearItems();
            }


        }

        protected override void InsertItem(int index, T item)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    base.InsertItem(index, item);
                });
            }
            else
            {
                // may be running in unit test
                base.InsertItem(index, item);
            }
        }
    }
}

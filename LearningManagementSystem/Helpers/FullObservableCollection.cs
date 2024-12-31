using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace LearningManagementSystem.Helpers
{
    /// <summary>
    /// Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection. Must implement INotifyPropertyChanged.</typeparam>
    public sealed class FullObservableCollection<T> : ObservableCollection<T>
    where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the FullObservableCollection class that is empty.
        /// </summary>
        public FullObservableCollection()
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        /// <summary>
        /// Initializes a new instance of the FullObservableCollection class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="pItems">The collection whose elements are copied to the new list.</param>
        public FullObservableCollection(IEnumerable<T> pItems) : this()
        {
            foreach (var item in pItems)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event to attach or detach PropertyChanged event handlers for the items in the collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A NotifyCollectionChangedEventArgs that contains the event data.</param>
        private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Object item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Object item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of an item in the collection to raise a CollectionChanged event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A PropertyChangedEventArgs that contains the event data.</param>
        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
            OnCollectionChanged(args);
        }
    }

}

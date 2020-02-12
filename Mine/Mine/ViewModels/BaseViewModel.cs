using Mine.Models;
using Mine.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mine.ViewModels
{
    /// <summary>
    /// Base View Model for Data
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {

        // The mock data source
        private IDataStore<ItemModel> DataSource_Mock => new MockDataStore();

        // The SQL data source
        private IDataStore<ItemModel> DataSource_SQL => new DatabaseService();

        // Accessible data source (switches between mock and SQL)
        public IDataStore<ItemModel> DataStore;
        
        // Which source we are using right now
        public int CurrentDataSource = 0;

        // The Data set of records
        public ObservableCollection<ItemModel> Dataset { get; set; }

        // Track if the system needs refreshing
        public bool _needsRefresh;

        // Command to force a Load of data
        public Command LoadDatasetCommand { get; set; }

        /// <summary>
        /// Mark if the view model is busy loading or done loading
        /// </summary>
        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        /// <summary>
        /// The String to show on the page
        /// </summary>
        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        /// <summary>
        /// Tracking what has changed in the dataset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingStore"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <param name="onChanged"></param>
        /// <returns></returns>
        protected bool SetProperty<T>(ref T backingStore,
            T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
            {
                return false;
            }

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);

            return true;
        }

        #region INotifyPropertyChanged
        /// <summary>
        /// Notify when changes happen
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
            {
                return;
            }

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
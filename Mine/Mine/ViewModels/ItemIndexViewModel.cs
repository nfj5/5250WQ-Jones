using Mine.Services;
using Mine.Models;
using Mine.Views;
using System.Diagnostics;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;

namespace Mine.ViewModels
{
    /// <summary>
    /// Index View Model
    /// Manages the list of data records
    /// </summary>
    public class ItemIndexViewModel : BaseViewModel
    {
        #region Attributes

        // The Mock DataStore
        private IDataStore<ItemModel> DataSource_Mock => new MockDataStore();

        // The SQL DataStore
        private IDataStore<ItemModel> DataSource_SQL => new DatabaseService();

        // Which DataStore to use
        public IDataStore<ItemModel> DataStore;

        // The Data set of records
        public ObservableCollection<ItemModel> Dataset { get; set; }

        // Tack the current data source, SQL, Mock
        public int CurrentDataSource = 0;

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

        #endregion Attributes

        #region Singleton

        private static volatile ItemIndexViewModel instance;
        private static readonly object syncRoot = new Object();

        public static ItemIndexViewModel Instance
        {
            get {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new ItemIndexViewModel();
                            instance.Initialize();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion Singleton

        #region Constructor

        /// <summary>
        /// Constructor
        /// 
        /// The constructor subscribes message listeners for crudi operations
        /// </summary>
        public ItemIndexViewModel()
        {
            Title = "Items";

            // Register the Create Message
            MessagingCenter.Subscribe<ItemCreatePage, ItemModel>(this, "Create", async (obj, data) =>
            {
                await Add(data as ItemModel);
            });

            // Register the Delete Message
            MessagingCenter.Subscribe<ItemDeletePage, ItemModel>(this, "Delete", async (obj, data) =>
            {
                await Delete(data as ItemModel);
            });

            // Register the Update Message
            MessagingCenter.Subscribe<ItemUpdatePage, ItemModel>(this, "Update", async (obj, data) =>
            {
                await Update(data as ItemModel);
            });

            // Register the Set Data Source Message
            MessagingCenter.Subscribe<AboutPage, int>(this, "SetDataSource", async (obj, data) =>
            {
                await SetDataSource(data);
            });

            // Register the Wipe Data List Message
            MessagingCenter.Subscribe<AboutPage, bool>(this, "WipeDataList", async (obj, data) =>
            {
                await WipeDataListAsync();
            });
        }

        #endregion Constructor

        /// <summary>
        /// Initialize the ViewModel
        /// Sets the collection Dataset
        /// Sets the Load command
        /// Sets the default data source
        /// </summary>
        public async void Initialize()
        {
            Dataset = new ObservableCollection<ItemModel>();
            LoadDatasetCommand = new Command(async () => await ExecuteLoadDataCommand());

            await SetDataSource(CurrentDataSource);   // Set to Mock to start with
        }

        #region DataSource

        /// <summary>
        /// Update the data source
        /// </summary>
        /// <param name="isSQL"></param>
        /// <returns></returns>
        async public Task<bool> SetDataSource(int isSQL)
        {
            if (isSQL == 1)
            {
                DataStore = DataSource_SQL;
                CurrentDataSource = 1;
            }
            else
            {
                DataStore = DataSource_Mock;
                CurrentDataSource = 0;
            }

            await LoadDefaultDataAsync();

            SetNeedsRefresh(true);

            return await Task.FromResult(true);
        }

        /// <summary>
        /// Load the default data asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LoadDefaultDataAsync()
        {
            if (Dataset.Count > 0)
            {
                return false;
            }

            foreach (var data in GetDefaultData())
            {
                await CreateUpdateAsync(data);
            }

            return true;
        }

        /// <summary>
        /// Load the default data for the model
        /// </summary>
        /// <returns></returns>
        public virtual List<ItemModel> GetDefaultData()
        {
            return DataStore.IndexAsync().Result.ToList<ItemModel>();
        }

        #endregion DataSource

        #region Refresh

        /// <summary>
        /// Command to load the data
        /// </summary>
        /// <returns></returns>
       // [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task ExecuteLoadDataCommand()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            try
            {
                Dataset.Clear();
                var dataset = await DataStore.IndexAsync();

                dataset = SortDataSet(dataset);

                foreach (var data in dataset)
                {
                    Dataset.Add(data);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Sort the dataset by Name
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public virtual List<ItemModel> SortDataSet(List<ItemModel> dataset)
        {
            return dataset.OrderBy(a => a.Name).ThenBy(a => a.Description).ToList();
        }

        // Indicated whether or not to refresh
        public void SetNeedsRefresh(bool value)
        {
            _needsRefresh = value;
        }

        public bool NeedsRefresh()
        {
            if (_needsRefresh)
            {
                _needsRefresh = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Force the data to be refreshed
        /// </summary>
        public void ForceDataRefresh()
        {
            var canExecute = LoadDatasetCommand.CanExecute(null);
            LoadDatasetCommand.Execute(null);
        }

        #endregion Refresh

        #region CRUDi

        /// <summary>
        /// API to add the Data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> CreateAsync(ItemModel data)
        {
            Dataset.Add(data);
            var result = await DataStore.CreateAsync(data);

            SetNeedsRefresh(true);

            return result;
        }

        /// <summary>
        /// Get the data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ItemModel> ReadAsync(string id)
        {
            var myData = await DataStore.ReadAsync(id);
            return myData;
        }

        /// <summary>
        /// Update the data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(ItemModel data)
        {
            // Check that the record exists, if it does not, then exit with false
            var record = await ReadAsync(data.Id);
            if (record == null)
            {
                return false;
            }

            // Save the change to the Data Store
            var result = await DataStore.UpdateAsync(record);

            SetNeedsRefresh(true);

            return result;
        }

        /// <summary>
        /// Delete the data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(ItemModel data)
        {
            // Check that the record exists, if it does not, then exit with false
            var record = await ReadAsync(data.Id);
            if (record == null)
            {
                return false;
            }

            // remove the record from the current data set in the viewmodel
            Dataset.Remove(data);

            // Have the record deleted from the data source
            var result = await DataStore.DeleteAsync(record.Id);

            SetNeedsRefresh(true);

            return result;
        }

        /// <summary>
        /// Having this at the ViewModel, because it has the DataStore
        /// That allows the feature to work for both SQL and the Mock datastores...
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> CreateUpdateAsync(ItemModel data)
        {
            // Check to see if the data exist
            var oldData = await ReadAsync(data.Id);
            if (oldData == null)
            {
                await CreateAsync(data);
                return true;
            }

            // Compare it, if different update in the DB
            var UpdateResult = await UpdateAsync(data);
            if (UpdateResult)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Wipes the current Data from the Data Store
        /// </summary>
        public async Task<bool> WipeDataListAsync()
        {
            await DataStore.WipeDataListAsync();

            // Load the Sample Data
            await LoadDefaultDataAsync();

            SetNeedsRefresh(true);

            return await Task.FromResult(true);
        }

        #endregion CRUDi

        /// <summary>
        /// API to add the Data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> Add(ItemModel data)
        {
            Dataset.Add(data);
            var result = await DataStore.CreateAsync(data);

            return true;
        }

        /// <summary>
        /// API to delete the Data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> Delete(ItemModel data)
        {
            var record = await Read(data.Id);
            if (record == null)
            {
                return false;
            }

            Dataset.Remove(data);
            var result = await DataStore.DeleteAsync(data.Id);

            return true;
        }

        /// <summary>
        /// API to read the Data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ItemModel> Read(String id)
        {
            var result = await DataStore.ReadAsync(id);
            return result;
        }

        /// <summary>
        /// API to update the Data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> Update(ItemModel data)
        {
            var record = await Read(data.Id);
            if (record == null)
            {
                return false;
            }

            record.Update(data);

            var result = await DataStore.UpdateAsync(record);

            await ExecuteLoadDataCommand();

            return result;
        }
    }
}
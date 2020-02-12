using SQLite;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mine.Models;

namespace Mine.Services
{
    /// <summary>
    /// Database Services
    /// Will write to the local data store
    /// </summary>
    public class DatabaseService
    {
        /// <summary>
        /// Set the class to load on demand
        /// Saves app boot time
        /// </summary>
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });

        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        /// <summary>
        /// Constructor
        /// All the database to start up
        /// </summary>
        public DatabaseService()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        /// <summary>
        /// Create the Table if it does not exist
        /// </summary>
        /// <returns></returns>
        async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(ItemModel).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(ItemModel)).ConfigureAwait(false);
                    initialized = true;
                }
            }
        }

        /// <summary>
        /// Create a new record for the data passed in
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> CreateAsync(ItemModel data)
        {
            var result = await Database.InsertAsync(data);
            return (result == 1);
        }

        /// <summary>
        /// Return the record for the ID passed in
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ItemModel> ReadAsync(string id)
        {
            ItemModel data;

            try
            {
                data = await Database.Table<ItemModel>().Where((ItemModel arg) => ((ItemModel)(object)arg).Id.Equals(id)).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                data = default(ItemModel);
            }

            return data;
        }

        /// <summary>
        /// Update the record passed in if it exists
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(ItemModel data)
        {
            var myRead = await ReadAsync(((ItemModel)(object)data).Id);
            if (myRead == null)
            {
                return false;
            }

            var result = await Database.UpdateAsync(data);

            return (result == 1);
        }

        /// <summary>
        /// Delete the record of the ID passed in
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string id)
        {
            var data = await ReadAsync(id);
            if (data == null)
            {
                return false;
            }

            var result = await Database.DeleteAsync(data);

            return (result == 1);
        }

        /// <summary>
        /// Return all records in the database
        /// </summary>
        /// <returns></returns>
        public async Task<List<ItemModel>> IndexAsync()
        {
            return await Database.Table<ItemModel>().ToListAsync();
        }

        
    }
}
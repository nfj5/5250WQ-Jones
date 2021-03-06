﻿namespace Mine.Models
{
    /// <summary>
    /// Item for the Game
    /// </summary>
    public class ItemModel : BaseModel
    {
        // Add Unique attributes for Item

        // The Value of the Item
        public int Value { get; set; } = 0;

        public bool Update(ItemModel data)
        {
            Name = data.Name;
            Description = data.Description;
            Value = data.Value;

            return true;
        }
    }
}
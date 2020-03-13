package com.example.getitems.models;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.UUID;

/**
 * Helper class for providing sample content for user interfaces created by
 * Android template wizards.
 * <p>
 * TODO: Replace all uses of this class before publishing your app.
 */
public class ItemViewModel {

    /**
     * An array of sample (dummy) items.
     */
    public static final List<ItemModel> ITEMS = new ArrayList<ItemModel>();

    /**
     * A map of sample (dummy) items, by ID.
     */
    public static final Map<String, ItemModel> ITEM_MAP = new HashMap<String, ItemModel>();

    private static final int COUNT = 25;

    static {
        // Add some sample items.
        addItem(new ItemModel("Smelly Jersey", "Smells so bad the monster won't come near you.", 5, 9, 9, "smelly_jersey"));
        addItem(new ItemModel("Pad Lock", "Strong enough to lock anyone down.", 3, 0, 9, "padlock"));
        addItem(new ItemModel("Shoes", "No one will be able to catch you in these.", 2, 0, 9, "shoe"));
    }

    private static void addItem(ItemModel item) {
        ITEMS.add(item);
        ITEM_MAP.put(item.id, item);
    }

    /**
     * A dummy item representing a piece of content.
     */
    public static class ItemModel {
        public final int Range;
        public final int Damage;
        public final int Value;
        public final String Name;
        public final String Description;
        public final String id = UUID.randomUUID().toString();
        public final String Guid = id;
        public final String ImageURI;

        public ItemModel(String Name, String Description, int Range, int Damage, int Value, String ImageURI) {
            this.Name = Name;
            this.Description = Description;
            this.Range = Range;
            this.Damage = Damage;
            this.Value = Value;
            this.ImageURI = ImageURI;
        }

        @Override
        public String toString() {
            return Name;
        }
    }
}

package com.example.getitems.dummy;

import org.w3c.dom.NameList;

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
        for (int i = 1; i <= COUNT; i++) {
            addItem(createDummyItem(i));
        }
    }

    private static void addItem(ItemModel item) {
        ITEMS.add(item);
        ITEM_MAP.put(item.id, item);
    }

    private static ItemModel createDummyItem(int position) {
        return new ItemModel(5, 5, 5, "Item " + String.valueOf(position), "Just another item", "https://s3.amazonaws.com/pix.iemoji.com/images/emoji/apple/ios-12/256/goat.png");
    }

    private static String makeDetails(int position) {
        StringBuilder builder = new StringBuilder();
        builder.append("Details about Item: ").append(position);
        for (int i = 0; i < position; i++) {
            builder.append("\nMore details information here.");
        }
        return builder.toString();
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

        public ItemModel(int Range, int Damage, int Value, String Name, String Description, String ImageURI) {
            this.Range = Range;
            this.Damage = Damage;
            this.Value = Value;
            this.Name = Name;
            this.Description = Description;
            this.ImageURI = ImageURI;
        }

        @Override
        public String toString() {
            return Name;
        }
    }
}

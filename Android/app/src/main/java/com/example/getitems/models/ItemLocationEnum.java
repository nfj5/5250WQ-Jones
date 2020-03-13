package com.example.getitems.models;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public enum ItemLocationEnum {

    // Need to match the service values
    Unknown(0),

    // The head includes, Hats, Helms, Caps, Crowns, Hair Ribbons, Bunny Ears, and anything else that sits on the head
    Head(10),

    // Things to put around the neck, such as necklass, broaches, scarfs, neck ribbons.  Can have at the same time with Head items ex. Ribbon for Hair, and Ribbon for Neck is OK to have
    Necklass(12),

    // The primary hand used for fighting with a sword or a staff.
    PrimaryHand(20),

    // The second hand used for holding a shield or dagger, or wand.  OK to have both primary and offhand loaded at the same time
    OffHand(22),

    // Any finger, used for rings, because they can go on any finger.
    Finger(30),

    // A finger on the Right hand for rings.  Can only have one right on the right hand
    RightFinger(31),

    // A finger on the left hand for rings.  Can only have one ring on the left hand.  Can have ring on left and right at the same time
    LeftFinger(32),

    // Boots, shoes, socks or anything else on the feet
    Feet(40);

    // Returns the Localized String
    // Make sure all strings are in the string table
    @Override
    public String toString() {
        switch(this){
            case Unknown:
                return "Unknown";
            case Head:
                return "Head";
            case Necklass:
                return "Necklass";
            case PrimaryHand:
                return "PrimaryHand";
            case OffHand:
                return "OffHand";
            case Finger:
                return "Any Finger";
            case RightFinger:
                return "Right Finger";
            case LeftFinger:
                return "Left Finger";
            case Feet:
                return "Feet";
        }
        return "Unknown";
    }

    private int intValue;
    private static final List<String> VALUES;

    static {
        VALUES = new ArrayList<>();
        for (ItemLocationEnum someEnum : ItemLocationEnum.values()) {
            VALUES.add(someEnum.toString());
        }
    }

    public static List<String> getValues() {
        return Collections.unmodifiableList(VALUES);
    }

    ItemLocationEnum(int value) {
        intValue = value;
    }

    // Lookup the string for the value
    public static String getString(int value){
        for (ItemLocationEnum item : ItemLocationEnum.values()) {
            if (value == item.intValue){
                return item.toString();
            }
        }
        return null;
    }

    // Lookup the value for the string
    public static int getValue(String string){
        for (ItemLocationEnum item : ItemLocationEnum.values()) {
            if (string.equals(item.toString())){
                return item.intValue;
            }
        }
        return -1;
    }
}
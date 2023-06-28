using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public static class CustomReadWriteFunctions
{

    #region Item

    private static List<Item> _items;
    private static List<Item> Items =>
        _items ??= Resources.LoadAll("Items", typeof(Item)).Cast<Item>().ToList();
    
    public static void WriteItem(this NetworkWriter writer, Item item)
    {
        byte index = 255;
        for (int i = 0; i < Items.Count; i++)
            if (Items[i] == item)
            {
                index = (byte) i;
                break;
            }

        if (index == 255)
            Debug.LogError("No similar weapon found");
        
        writer.WriteByte(index);
    }

    public static Item ReadItem(this NetworkReader reader) => Items[reader.ReadByte()];

    #endregion
    
    #region Gamemode

    private static List<Gamemode> _gamemodes;
    private static List<Gamemode> Gamemodes =>
        _gamemodes ??= Resources.LoadAll<Gamemode>("Gamemodes").ToList();
    
    public static void WriteGamemode(this NetworkWriter writer, Gamemode gamemode) => writer.WriteByte((byte)Gamemodes.IndexOf(gamemode));

    public static Gamemode ReadGamemode(this NetworkReader reader) => Gamemodes[reader.ReadByte()];

    #endregion
}

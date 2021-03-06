﻿//Written by Sam Arutyunyan for Design Patterns Project Spring 2015
using UnityEngine;

namespace inventory
{
    public class Item
    {
        string _name;
        int _value;
        RarityTypes _rarity;
        int _curDur;
        int _maxDur;
        Texture2D _icon;

        public Item()
        {
            _name = "needs name";
            _value = 0;
            _rarity = RarityTypes.Common;
            _maxDur = 50;
            _curDur = _maxDur;
        }
        public Item(string name, int value, RarityTypes rare, int maxDur, int curDur)
        {
            _name = name;
            _value = value;
            _rarity = rare;
            _maxDur = maxDur;
            _curDur = curDur;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public RarityTypes Rarity
        {
            get { return _rarity; }
            set { _rarity = value; }
        }
        public int MaxDurability
        {
            get { return _maxDur; }
            set { _maxDur = value; }
        }
        public int CurDurability
        {
            get { return _curDur; }
            set { _curDur = value; }
        }
        public Texture2D Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }
        public virtual string ToolTip()
        {
            return Name + "\n" +
                    "Value " + Value + "\n" +
                    "Durability " + CurDurability + "/" + MaxDurability + "\n";

        }


    }
}
public enum RarityTypes
{ 
    Common,
    Uncommon,
    Rare
}
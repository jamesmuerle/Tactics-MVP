using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorCounter : MonoBehaviour {
    public Sprite[] armorSprites;

    private int _armorPoints;
    public int armorPoints {
        get {
            return _armorPoints;
        }
        set {
            _armorPoints = value;
            int boundedArmorPoints = Math.Max(0, _armorPoints);
            this.GetComponent<SpriteRenderer>().sprite = armorSprites[boundedArmorPoints];
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHighlighter : MonoBehaviour {
    public SpriteRenderer unitSpriteRenderer;
    private SpriteMask mask;

    void Awake() {
        mask = GetComponent<SpriteMask>();
    }
	
	// Update is called once per frame
	void Update () {
        mask.sprite = unitSpriteRenderer.sprite;
	}
}

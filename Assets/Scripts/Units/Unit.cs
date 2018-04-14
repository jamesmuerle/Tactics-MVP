using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    public UnitHighlighter highlightPrefab;
    private UnitHighlighter currentHighlight;

    void OnMouseEnter () {
        currentHighlight =
            Instantiate(highlightPrefab, Vector3.zero, Quaternion.identity);
        currentHighlight.unitSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        currentHighlight.gameObject.transform.SetParent(this.transform);
        currentHighlight.gameObject.transform.localPosition = Vector3.zero;
    }

    void OnMouseExit () {
        Destroy(currentHighlight.gameObject);
    }
}

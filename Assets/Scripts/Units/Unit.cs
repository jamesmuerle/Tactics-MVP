using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour {
    public UnitHighlighter highlightPrefab;

    private class UnitEvent : UnityEvent<Unit> { }
    public UnityEvent<Unit> unitClicked = new UnitEvent();

    private UnitHighlighter currentHighlight;

    private bool _isSelected = false;
    public bool isSelected {
        get {
            return _isSelected;
        }
        set {
            SetSelected(value);
        }
    }

    private void SetSelected(bool isSelected) {
        _isSelected = isSelected;
        if (!isSelected) {
            DestroyHighlight();
        }
    }

    private void OnMouseEnter () {
        if (!_isSelected) {
            currentHighlight =
                Instantiate(highlightPrefab, Vector3.zero, Quaternion.identity);
            currentHighlight.unitSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
            currentHighlight.gameObject.transform.SetParent(this.transform);
            currentHighlight.gameObject.transform.localPosition = Vector3.zero;
        }
    }

    private void OnMouseExit () {
        if (!_isSelected) {
            DestroyHighlight();
        }
    }

    private void DestroyHighlight() {
        Destroy(currentHighlight.gameObject);
        currentHighlight = null;
    }

    private void OnMouseUpAsButton() {
        unitClicked.Invoke(this);
    }
}

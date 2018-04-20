using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tile : MonoBehaviour {

    private class MousePositionEvent : UnityEvent<Vector2Int> { }
    public UnityEvent<Vector2Int> posEntered = new MousePositionEvent();
    public UnityEvent<Vector2Int> posExited = new MousePositionEvent();
    public UnityEvent<Vector2Int> posClicked = new MousePositionEvent();
    public UnityEvent<Vector2Int> posRightClicked = new MousePositionEvent();

    private void OnMouseEnter() {
        InvokeWithPosition(posEntered);
    }

    private void OnMouseExit() {
        InvokeWithPosition(posExited);
    }

    private void Update() {
        if (Input.GetMouseButtonUp(1)) {
            InvokeWithPosition(posRightClicked);
        }
    }
    private void OnMouseUpAsButton() {
        InvokeWithPosition(posClicked);
    }

    private void InvokeWithPosition(UnityEvent<Vector2Int> unityEvent) {
        Vector3 position = this.gameObject.transform.position;
        unityEvent.Invoke(new Vector2Int((int) position.x, (int) position.y));
    }
}

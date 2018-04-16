using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tile : MonoBehaviour {

    private class MousePositionEvent : UnityEvent<int, int> { }
    public UnityEvent<int, int> posEntered = new MousePositionEvent();
    public UnityEvent<int, int> posExited = new MousePositionEvent();
    public UnityEvent<int, int> posClicked = new MousePositionEvent();
    public UnityEvent<int, int> posRightClicked = new MousePositionEvent();

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

    private void InvokeWithPosition(UnityEvent<int, int> unityEvent) {
        Vector3 position = this.gameObject.transform.position;
        unityEvent.Invoke((int)position.x, (int)position.y);
    }
}

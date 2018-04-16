using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour {
    public UnitHighlighter highlightPrefab;
    public Animator unitAnimator;

    public bool isMoving {
        set {
            unitAnimator.SetBool("isMoving", value);
        }
    }

    public bool isAttacking {
        set {
            unitAnimator.SetBool("isAttacking", value);
        }
    }

    public bool hasMoved = false;
    public int movementRange;
    private float movementSpeed = 4.0f;

    public bool hasAttacked = false;
    public int attackRange;
    public int attackDamage;
    public int maxArmorPoints;

    private int currentArmorPoints;

    private UnitHighlighter currentHighlight;

    private List<Vector2Int> path = new List<Vector2Int>();

    public void MoveThroughPath(List<Vector2Int> path) {
        this.path = path;
        this.hasMoved = true;
        this.enabled = true;
    }

    private void Update() {
        if (path.Count > 0) {
            Vector3 currentTarget = new Vector3(path[0].x, path[0].y, 0);
            Vector3 currentPosition = this.gameObject.transform.position;
            Vector3 animationDirection = currentTarget - currentPosition;
            if (animationDirection.magnitude <= movementSpeed * Time.deltaTime) {
                this.gameObject.transform.position = currentTarget;
                path.RemoveAt(0);
            }
            else {
                Vector3 movementVector = animationDirection.normalized * movementSpeed * Time.deltaTime;
                this.gameObject.transform.position = currentPosition + movementVector;
            }
        }
        else {
            this.enabled = false;
            this.isMoving = false;
        }
    }

    private void Awake() {
        currentArmorPoints = maxArmorPoints;
    }

    private bool _isSelected = false;
    public bool isSelected {
        get {
            return _isSelected;
        }
        set {
            _isSelected = value;
            UpdateHighlightState();
        }
    }

    private bool _isHovered = false;
    public bool isHovered {
        get {
            return _isHovered;
        }
        set {
            _isHovered = value;
            UpdateHighlightState();
        }
    }

    private void UpdateHighlightState() {
        if (currentHighlight && !_isHovered && !_isSelected) {
            DestroyHighlight();
        }
        else if (!currentHighlight && (_isHovered || _isSelected)) {
            CreateHighlight();
        }
    }

    private void DestroyHighlight() {
        Destroy(currentHighlight.gameObject);
        currentHighlight = null;
    }

    private void CreateHighlight() {
        currentHighlight =
            Instantiate(highlightPrefab, Vector3.zero, Quaternion.identity);
        currentHighlight.unitSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        currentHighlight.gameObject.transform.SetParent(this.transform);
        currentHighlight.gameObject.transform.localPosition = Vector3.zero;
    }

    public Vector2Int GetPosition() {
        int pathSize = path.Count;
        if (pathSize > 0) {
            return path[pathSize - 1];
        }
        else {
            Vector3 position = this.gameObject.transform.position;
            return new Vector2Int((int)position.x, (int)position.y);
        }
    }

    public bool IsUnitInAttackRange(Unit otherUnit) {
        Vector2Int posDifference = otherUnit.GetPosition() - GetPosition();
        return Math.Abs(posDifference.x) + Math.Abs(posDifference.y) <= attackRange;
    }

    public void ExecuteAttackOn(Unit otherUnit) {
        this.isAttacking = false;
        this.hasAttacked = true;
        otherUnit.TakeDamage(this.attackDamage);
        print("Unit was attacked");
    }

    public void TakeDamage(int damage) {
        currentArmorPoints -= damage;
        if (currentArmorPoints < 0) {
            Destroy(this.gameObject);
        }
    }
}

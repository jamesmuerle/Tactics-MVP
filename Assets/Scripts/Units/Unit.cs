using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour {
    public UnitHighlighter highlightPrefab;
    public Animator unitAnimator;
    public ArmorCounter armorCounterPrefab;

    public int movementRange;
    private float movementSpeed = 4.0f;
    public int attackRange;
    public int attackDamage;
    public int maxArmorPoints;
    private ArmorCounter currentArmorCounter;
    private UnitHighlighter currentHighlight;
    private List<Vector2Int> path = new List<Vector2Int>();
    private Unit attackTarget;

    public bool isMoving {
        get {
            return unitAnimator.GetBool("isMoving");
        }
        set {
            unitAnimator.SetBool("isMoving", value);
        }
    }

    public bool isAiming {
        get {
            return unitAnimator.GetBool("isAiming");
        }
        set {
            unitAnimator.SetBool("isAiming", value);
        }
    }

    public bool isDead {
        get {
            return unitAnimator.GetBool("isDead");
        }
        set {
            unitAnimator.SetBool("isDead", true);
        }
    }

    private bool _hasMoved = false;
    public bool hasMoved {
        get {
            return _hasMoved;
        }
        set {
            _hasMoved = value;
            unitAnimator.SetBool("isDone", _hasMoved && _hasAttacked);
        }
    }

    public bool _hasAttacked = false;
    public bool hasAttacked {
        get {
            return _hasAttacked;
        }
        set {
            _hasAttacked = value;
            unitAnimator.SetBool("isDone", _hasMoved && _hasAttacked);
        }
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

    private void Awake() {
        currentArmorCounter = Instantiate(armorCounterPrefab, Vector3.zero, Quaternion.identity);
        currentArmorCounter.transform.SetParent(this.transform);
        currentArmorCounter.armorPoints = maxArmorPoints;
        currentArmorCounter.transform.localPosition = new Vector3(0.25f, -0.25f, 0);
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
        else if (_isSelected) {
            if (!hasMoved && !this.isMoving) {
                this.isMoving = true;
            }
            else if (hasMoved && this.isMoving) {
                this.isMoving = false;
            }
            if (hasMoved && !hasAttacked && !this.isAiming) {
                this.isAiming = true;
            }
        }
        else {
            this.isMoving = false;
            if (!hasAttacked && this.isAiming) {
                this.isAiming = false;
            }
        }
    }

    public void MoveThroughPath(List<Vector2Int> path) {
        this.path = path;
        this.hasMoved = true;
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
        return otherUnit != this && Math.Abs(posDifference.x) + Math.Abs(posDifference.y) <= attackRange;
    }

    public void ExecuteAttackOn(Unit otherUnit) {
        unitAnimator.SetTrigger("isFiring");
        this.hasMoved = true;
        this.hasAttacked = true;
        attackTarget = otherUnit;
        attackTarget.TakeDamage(this.attackDamage);
    }

    public void TakeDamage(int damage) {
        currentArmorCounter.armorPoints -= damage;
        if (currentArmorCounter.armorPoints < 0) {
           isDead = true;
        }
    }

    public void OnFiringEnd() {
        print("Firing has ended with aiming: " + this.isAiming);
        this.isAiming = false;
        attackTarget.unitAnimator.SetTrigger("isHit");
        attackTarget = null;
    }

    public void OnDeathEnd() {
        Destroy(this.gameObject);
    }
}

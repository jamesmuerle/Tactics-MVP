using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathContainer : MonoBehaviour {
    public GameObject pathStart;
    public GameObject pathStraight;
    public GameObject pathBend;
    public GameObject pathEnd;

    private Unit sourceUnit;
    private List<Vector2Int> path = new List<Vector2Int>();

    public void SetSourceUnit(Unit unit) {
        Empty();
        sourceUnit = unit;
    }

    public void GoToPosition(Vector2Int targetPos) {
        if (targetPos == sourceUnit.GetPosition()) {
            Empty();
        }
        else {
            int alreadyExistsIndex = path.FindIndex(vector => vector == targetPos);
            if (alreadyExistsIndex != -1) {
                path.RemoveRange(alreadyExistsIndex, path.Count - alreadyExistsIndex);
            }

            Vector2Int changeVector = targetPos - GetTarget();
            while (changeVector.magnitude > 1 && path.Count < sourceUnit.movementRange) {
                if (changeVector.x > 0) {
                    GoToPosition(GetTarget() + Directions.EAST);
                    changeVector -= Directions.EAST;
                }
                else if (changeVector.x < 0) {
                    GoToPosition(GetTarget() + Directions.WEST);
                    changeVector -= Directions.WEST;
                }
                else if (changeVector.y > 0) {
                    GoToPosition(GetTarget() + Directions.NORTH);
                    changeVector -= Directions.NORTH;
                }
                else if (changeVector.y < 0) {
                    GoToPosition(GetTarget() + Directions.SOUTH);
                    changeVector -= Directions.SOUTH;
                }
            }

            if (path.Count < sourceUnit.movementRange) {
                path.Add(targetPos);
                RedrawPath();
            }
        }
    }

    public void Empty() {
        path = new List<Vector2Int>();
        RedrawPath();
    }

    public void SetInvisible() {
        foreach (Transform child in this.transform) {
            Destroy(child.gameObject);
        }
    }

    public void SetVisible() {
        RedrawPath();
    }

    private void RedrawPath() {
        foreach (Transform child in this.transform) {
            Destroy(child.gameObject);
        }

        int pathSize = path.Count;
        if (pathSize > 0) {
            Vector2Int startPosition = sourceUnit.GetPosition();
            AddPathStart(startPosition, path[0]);

            Vector2Int prevPosition = startPosition;
            for (int i = 0; i < pathSize; i += 1) {
                Vector2Int currPosition = path[i];
                if (i == pathSize - 1) {
                    AddPathEnd(currPosition, prevPosition);
                }
                else {
                    Vector2Int nextPosition = path[i + 1];
                    AddPathConnector(prevPosition, currPosition, nextPosition);
                }
                prevPosition = currPosition;
            }
        }
    }

    private void AddPathStart(Vector2Int position, Vector2Int toPosition) {
        int rotAngle = GetRotationAngleForDirection(toPosition - position);
        CreatePathElement(pathStart, position, rotAngle);
    }

    private void AddPathEnd(Vector2Int position, Vector2Int fromPosition) {
        int rotAngle = GetRotationAngleForDirection(position - fromPosition);
        CreatePathElement(pathEnd, position, rotAngle);
    }

    private int GetRotationAngleForDirection(Vector2Int direction) {
        if (direction == Directions.NORTH) {
            return 0;
        }
        else if (direction == Directions.WEST) {
            return 90;
        }
        else if (direction == Directions.SOUTH) {
            return 180;
        }
        else if (direction == Directions.EAST) {
            return 270;
        }
        else {
            throw new System.Exception("Got a bad direction vector: " + direction);
        }
    }

    private void AddPathConnector(Vector2Int prevPosition, Vector2Int currPosition, Vector2Int nextPosition) {
        int rotAngle = 0;
        GameObject pathPrefab = null;

        Vector2Int direction = nextPosition - prevPosition;
        if (direction == Directions.NORTH * 2 || direction == Directions.SOUTH * 2) {
            rotAngle = 0;
            pathPrefab = pathStraight;
        }
        else if (direction == Directions.WEST * 2 || direction == Directions.EAST * 2) {
            rotAngle = 90;
            pathPrefab = pathStraight;
        }
        else if (direction == Directions.NORTHWEST || direction == Directions.SOUTHEAST) {
            pathPrefab = pathBend;

            Vector2Int outDirection = nextPosition - currPosition;
            if (outDirection == Directions.WEST || outDirection == Directions.SOUTH) {
                rotAngle = 0;
            }
            else if (outDirection == Directions.NORTH || outDirection == Directions.EAST) {
                rotAngle = 180;
            }
            else {
                ThrowBadDirectionException(direction);
            }
        }
        else if (direction == Directions.NORTHEAST || direction == Directions.SOUTHWEST) {
            pathPrefab = pathBend;

            Vector2Int outDirection = nextPosition - currPosition;
            if (outDirection == Directions.EAST || outDirection == Directions.SOUTH) {
                rotAngle = 90;
            }
            else if (outDirection == Directions.NORTH || outDirection == Directions.WEST) {
                rotAngle = 270;
            }
            else {
                ThrowBadDirectionException(direction);
            }
        }
        else {
            ThrowBadDirectionException(direction);
        }

        CreatePathElement(pathPrefab, currPosition, rotAngle);
    }

    private void CreatePathElement(GameObject pathPrefab, Vector2Int pos, int rotAngle) {
        Quaternion rotation = Quaternion.AngleAxis(rotAngle, new Vector3(0, 0, 1));
        GameObject pathElem =
            Instantiate(pathPrefab, new Vector3(pos.x, pos.y, 0), rotation);
        pathElem.transform.SetParent(this.transform);
    }

    private int ThrowBadDirectionException(Vector2Int direction) {
        foreach (Vector2Int vector in path) {
            print(vector);
        }
        throw new System.Exception("Got a bad direction vector: " + direction);
    }

    private Vector2Int TranslateTo2D(Vector3 pos) {
        return new Vector2Int((int) pos.x, (int) pos.y);
    }

    public List<Vector2Int> GetPath() {
        return path;
    }

    public Vector2Int GetTarget() {
        if (path.Count > 0) {
            return path[path.Count - 1];
        }
        else {
            return sourceUnit.GetPosition();
        }
    }
}

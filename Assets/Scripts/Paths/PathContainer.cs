using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathContainer : MonoBehaviour {
    public GameObject pathStart;
    public GameObject pathStraight;
    public GameObject pathBend;
    public GameObject pathEnd;

    private static Vector2Int NORTH = new Vector2Int(0, 1);
    private static Vector2Int WEST = new Vector2Int(-1, 0);
    private static Vector2Int SOUTH = new Vector2Int(0, -1);
    private static Vector2Int EAST = new Vector2Int(1, 0);

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
        if (direction == NORTH) {
            return 0;
        }
        else if (direction == WEST) {
            return 90;
        }
        else if (direction == SOUTH) {
            return 180;
        }
        else if (direction == EAST) {
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
        if (direction == NORTH * 2 || direction == SOUTH * 2) {
            rotAngle = 0;
            pathPrefab = pathStraight;
        }
        else if (direction == WEST * 2 || direction == EAST * 2) {
            rotAngle = 90;
            pathPrefab = pathStraight;
        }
        else if (direction == NORTH + WEST || direction == SOUTH + EAST) {
            pathPrefab = pathBend;

            Vector2Int outDirection = nextPosition - currPosition;
            if (outDirection == WEST || outDirection == SOUTH) {
                rotAngle = 0;
            }
            else if (outDirection == NORTH || outDirection == EAST) {
                rotAngle = 180;
            }
            else {
                ThrowBadDirectionException(direction);
            }
        }
        else if (direction == NORTH + EAST || direction == SOUTH + WEST) {
            pathPrefab = pathBend;

            Vector2Int outDirection = nextPosition - currPosition;
            if (outDirection == EAST || outDirection == SOUTH) {
                rotAngle = 90;
            }
            else if (outDirection == NORTH || outDirection == WEST) {
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
        throw new System.Exception("Got a bad direction vector: " + direction);
    }

    private Vector2Int TranslateTo2D(Vector3 pos) {
        return new Vector2Int((int) pos.x, (int) pos.y);
    }

    public List<Vector2Int> GetPath() {
        return path;
    }

    public Vector2Int GetTarget() {
        return path[path.Count - 1];
    }
}

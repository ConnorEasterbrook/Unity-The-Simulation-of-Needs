using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildCore : MonoBehaviour
{
    public static bool isBuilding = true;
    [SerializeField] private GameObject _objectPrefab;
    [SerializeField] private GameObject _plane;
    private Plane _gridPlane;
    [SerializeField] private float _tileSize = 1f;

    private GameObject _currentObject;
    [SerializeField] private GameObject _previewObject;
    private Vector3 _startPoint;
    private Vector3 _endPoint;
    private Vector3 _objectPosition;
    private Vector3 _initialObjectScale;

    private Vector3 _direction;
    private float _length;

    private List<Vector3> corners = new List<Vector3>();
    private int cornerCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        _gridPlane = new Plane(_plane.transform.up, _plane.transform.position);
        _initialObjectScale = _objectPrefab.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBuilding)
        {
            BuildObject();
        }
    }

    /// <summary>
    /// Controls the building of the object
    /// </summary>
    private void BuildObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _startPoint = SnapToGrid(GetMouseWorldPositionOnPlane());
            _previewObject.transform.position = _startPoint;
        }
        else if (Input.GetMouseButton(0) && _previewObject.activeSelf)
        {
            _endPoint = SnapToGrid(GetMouseWorldPositionOnPlane());
            PreviewObject();
        }
        else if (Input.GetMouseButtonUp(0) && _previewObject.activeSelf)
        {
            _endPoint = SnapToGrid(GetMouseWorldPositionOnPlane());
            InstantiateObject();

            _previewObject.transform.localScale = _initialObjectScale; // Reset scale
        }
        else
        {
            Vector3 hitPoint = SnapToGrid(GetMouseWorldPositionOnPlane());
            _objectPrefab.transform.position = new Vector3(hitPoint.x, hitPoint.y, hitPoint.z);
            _objectPrefab.transform.localScale = _initialObjectScale;
        }
    }

    /// <summary>
    /// Previews the object
    /// </summary>
    private void PreviewObject()
    {
        _direction = _endPoint - _startPoint;
        _length = _direction.magnitude;
        _direction.Normalize();

        if (SnapToGrid(_startPoint) != SnapToGrid(_endPoint))
        {
            _length = Mathf.Max(_length, _tileSize);

            _previewObject.transform.localScale = new Vector3(_initialObjectScale.x, _initialObjectScale.y, _length);
            _previewObject.transform.rotation = Quaternion.LookRotation(_direction);
        }
        else
        {
            _previewObject.transform.localScale = _initialObjectScale;
        }

        _objectPosition = _startPoint + _direction * _length * 0.5f;
        _previewObject.transform.position = _objectPosition;
    }

    /// <summary>
    /// Instantiates the object
    /// </summary>
    private void InstantiateObject()
    {
        int numTiles = Mathf.FloorToInt(_length / _tileSize);
        float remainingLength = _length - (numTiles * _tileSize);

        if (numTiles == 0 || CheckIntersections(numTiles, remainingLength))
        {
            return;
        }

        for (int i = 0; i < numTiles; i++)
        {
            Vector3 position = _startPoint + _direction * (_tileSize * 0.5f + _tileSize * i);

            GameObject newObject = Instantiate(_objectPrefab, position, Quaternion.LookRotation(_direction), _plane.transform);
            newObject.AddComponent<BoxCollider>();
            newObject.transform.localScale = new Vector3(_initialObjectScale.x, _initialObjectScale.y, _tileSize);
            newObject.tag = "Construct";
            newObject.name = "Wall";
        }

        if (remainingLength == 0)
        {
            return;
        }

        Vector3 lastPosition = _startPoint + _direction * (_tileSize * numTiles + remainingLength * 0.5f);
        GameObject newObjectLast = Instantiate(_objectPrefab, lastPosition, Quaternion.LookRotation(_direction), _plane.transform);
        newObjectLast.AddComponent<BoxCollider>();
        newObjectLast.transform.localScale = new Vector3(_initialObjectScale.x, _initialObjectScale.y, remainingLength);
        newObjectLast.tag = "Construct";
        newObjectLast.name = "Wall";

        TrackCorners();
    }

    private void TrackCorners()
    {
        if (corners.Count == 0)
        {
            corners.Add(_startPoint);
        }
        else
        {
            corners.Add(_endPoint);
        }

        cornerCount++;

        if (cornerCount > 2)
        {
            if (corners[cornerCount - 1] == corners[0])
            {
                Debug.Log("Closed");
            }
        }
    }

    /// <summary>
    /// Checks if the object intersects with another object
    /// </summary>
    private bool CheckIntersections(int numTiles, float remainingLength)
    {
        bool intersects = false;

        Collider[] collider = Physics.OverlapBox(_objectPosition, new Vector3(_initialObjectScale.x, _initialObjectScale.y, _length * 0.45f), Quaternion.LookRotation(_direction));

        foreach (Collider col in collider)
        {
            if (col.gameObject.tag == "Construct")
            {
                // Ignore Start and End points
                if (col.gameObject.transform.position == _startPoint || col.gameObject.transform.position == _endPoint)
                {
                    continue;
                }
                else
                {
                    Debug.Log("Intersects with " + col.gameObject.name);
                    intersects = true;
                    break;
                }
            }
        }

        return intersects;
    }

    /// <summary>
    /// Gets the mouse position on the grid plane
    /// </summary>
    private Vector3 GetMouseWorldPositionOnPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;

        if (_gridPlane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }

    /// <summary>
    /// Snaps the given point to the grid
    /// </summary>
    private Vector3 SnapToGrid(Vector3 hitPoint)
    {
        float x = Mathf.Round(hitPoint.x / _tileSize) * _tileSize;
        float y = hitPoint.y + (_initialObjectScale.y * 0.5f);
        float z = Mathf.Round(hitPoint.z / _tileSize) * _tileSize;

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Sets the building object to be placed
    /// </summary>
    public void SetBuildingObject(GameObject buildingObject)
    {
        _objectPrefab = buildingObject;
        _initialObjectScale = _objectPrefab.transform.localScale;
        _previewObject.SetActive(true);
    }
}

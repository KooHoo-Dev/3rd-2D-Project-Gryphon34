using System.Collections.Generic;
using UnityEngine;

public class ScrollController1 : MonoBehaviour
{
    public enum ScrollDirection { Left, Right, Up, Down }

    [Header("Scroll Settings")]
    public float speed = 1.0f;
    public ScrollDirection direction = ScrollDirection.Left;
    public int randNum = 1;

    [Header("Resources")]
    public GameObject[] layerPrefabs;

    [Header("Ref Objects")]
    public GameObject startLayer;
    public Transform endPivot;
    public Transform spawnPivot;

    private List<GameObject> enableLayerList = new List<GameObject>();

    private void Start()
    {
        if (layerPrefabs.Length == 0)
        {
            Debug.LogError($"layerPrefabs.Length가 비어있습니다");
            return;
        }
        enableLayerList.Add(startLayer);
    }

    private void Update()
    {
        MoveLayerList();
        CheckDestroyAbleLayer();
        CheckInstantiateLayer();
    }

    private Vector3 GetMoveDirection(ScrollDirection dir)
    {
        switch (dir)
        {
            case ScrollDirection.Left:
                return Vector3.left;
            case ScrollDirection.Right:
                return Vector3.right;
            case ScrollDirection.Up:
                return Vector3.up;
            case ScrollDirection.Down:
                return Vector3.down;
            default:
                return Vector3.left;
        }
    }

    private void MoveLayerList()
    {
        Vector3 dir = GetMoveDirection(direction);
        Vector3 moveVector = dir * (speed * Time.deltaTime);

        for (int i = 0; i < enableLayerList.Count; ++i)
        {
            enableLayerList[i].transform.Translate(moveVector);
        }
    }

    private void CheckDestroyAbleLayer()
    {
        GameObject headLayer = enableLayerList[0];

        bool check = false;

        switch (direction)
        {
            case ScrollDirection.Left:
                check = headLayer.transform.position.x <= endPivot.position.x;
                break;
            case ScrollDirection.Right:
                check = headLayer.transform.position.x >= endPivot.position.x;
                break;
            case ScrollDirection.Up:
                check = headLayer.transform.position.y >= endPivot.position.y;
                break;
            case ScrollDirection.Down:
                check = headLayer.transform.position.y <= endPivot.position.y;
                break;
        }

        if (check)
        {
            enableLayerList.RemoveAt(0);
            Destroy(headLayer);
        }
    }

    private void CheckInstantiateLayer()
    {
        while (enableLayerList.Count < 2)
        {
            int prefabRange = Random.Range(0, randNum);
            GameObject instance = Instantiate(layerPrefabs[prefabRange],
                spawnPivot.transform.position, spawnPivot.rotation);    
            enableLayerList.Add(instance);
        }
    }
}
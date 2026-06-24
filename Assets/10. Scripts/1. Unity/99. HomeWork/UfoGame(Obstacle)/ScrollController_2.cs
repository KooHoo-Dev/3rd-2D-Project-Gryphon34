using System.Collections.Generic;
using UnityEngine;

namespace Study.LayerAndScroll
{
    // ScrollController라는 녀석을 통해서
    // Layer의 속도를 조절해 볼겁니다
    public class ScrollController_2 : MonoBehaviour
    {
        public enum ScrollDirection { Left, Right, Up, Down }

        [Header("Scroll Settings")]
        public float speed = 1.0f;
        public ScrollDirection direction = ScrollDirection.Left;

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
                Debug.LogError("layerPrefabs.Length가 0입니다.");
                return;
            }
            enableLayerList.Add(startLayer);
        }

        private void Update()
        {
            MoveLayerList();
            CheckDestroyAbleLayer();
            CheckLayer();
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
            // (speed * Time.deltaTime) = 초당 speed의 속도로 뭔가를 하겠다는 표현
            Vector3 dir = GetMoveDirection(direction);
            Vector3 moveVector = dir * (speed * Time.deltaTime);

            // 1. 활성화된 모든 레이어를 moveVector만큼 옮겨준다
            for (int i = 0; i < enableLayerList.Count; ++i)
            {
                enableLayerList[i].transform.Translate(moveVector);
            }
        }

        private void CheckDestroyAbleLayer()
        {
            // 2. 가장 첫번째 Layer(enableLayerList[0])가
            // EndPivot의 경계를 넘어간다면(x값보다 작아진다면)
            // 삭제한다.

            GameObject headLayer = enableLayerList[0];
            // 가장 앞에있는 Layer오브젝트를 가져옵니다

            bool check = false;

            switch (direction)
            {
                case ScrollDirection.Left:
                    // headLayer의 x가 endPivot보다 작다면
                    check = headLayer.transform.position.x <= endPivot.position.x;
                    break;
                case ScrollDirection.Right:
                    // headLayer의 x가 endPivot보다 크다면
                    check = headLayer.transform.position.x >= endPivot.position.x;
                    break;
                case ScrollDirection.Up:
                    // headLayer의 y가 endPivot보다 크다면
                    check = headLayer.transform.position.y >= endPivot.position.y;
                    break;
                case ScrollDirection.Down:
                    // headLayer의 y가 endPivot보다 작다면
                    check = headLayer.transform.position.y <= endPivot.position.y;
                    break;
            }

            // 왼쪽케이스
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
                GameObject layer = SpawnRandomLayer();
            }
        }


        // 랜덤한 Layer를 대기열에 넣어두고, 앞으로 나올 Layer들을 미리 알아둡시다.
        private Queue<GameObject> layerQueue = new Queue<GameObject>();

        // 이 함수는 호출되면 랜덤한 레이어를 생성만 해서 반환합니다.
        private GameObject SpawnRandomLayer()
        {
            int randIdx = Random.Range(0, layerPrefabs.Length);
            GameObject layer = Instantiate(layerPrefabs[randIdx],transform);
            // 기본적으로 .Instantiate()함수에는 여러가지 활용법이 있고,
            // 그 중 하나로 Instantiate(생성할 견본, Transfrom parent); 와 같이
            // 생성하면서 동시에 GameObject의 부모 관계를 설정해주는 활용법이 있습니다.
            layer.transform.position= spawnPivot.position;
            return layer;

        }

        // 이 함수는 대기열의 잔량을 체크해서 일정량 이하가 된다면
        // 랜덤한 레이어를 대기열에 추가합니다.
        private void CheckLayer()
        {
            //1. Layer를 생성해서 대기열에 넣고,
            //2. 기존의 삭제로직이 실행되면
            //3. 대기열에 있는 Layer를 꺼내와서 enableLayerList에 넣어줍니다.
            //4. if(대기열의 갯수가 충분치 않다면)
            //5.    미리 레이어를 생성해서 대기열을 채워줍니다

            // layerQueue에 레이어 담아둠 => 레이어가 지나가면 =>
            // layerQueue에 있는 레이어를 enableLayerList에 옮겨줌

            if (layerQueue.Count < 3)
            {
                for (int i = 0; i < 10; ++i)
                {
                    GameObject randLayer = SpawnRandomLayer();
                    randLayer.SetActive(false);
                    layerQueue.Enqueue(randLayer);
                    Debug.Log($"{randLayer.gameObject.name}이 대기열에 추가되었습니다!");
                }
            }
            if (enableLayerList.Count < 2)
            {
                // 대기열의 가장 앞에 있는 개체를 꺼냅니다.
                GameObject enableLayer = layerQueue.Dequeue();
                enableLayer.SetActive(true);
                enableLayerList.Add(enableLayer);
            } 
        }
    }
}

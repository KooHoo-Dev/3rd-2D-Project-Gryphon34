using UnityEngine;

namespace Study.Mono
{
    // # Life Cycle
    // [오브젝트 생성]
    //    ↓
    // Awake()      (딱 한 번)
    //    ↓
    // OnEnable()    (활성화될 때마다)
    //    ↓
    // Start()      (딱 한 번)
    //    ↓
    // ┌───────── ┐
    // │  (게임 루프 시작) │
    // │      ↓           │
    // │ FixedUpdate()     │   (물리 업데이트)
    // │      ↓           │
    // │   Update()        │   (프레임 업데이트)
    // │      ↓           │
    // │ LateUpdate()      │   (모든 Update 후)
    // │      ↓           │
    // └───────── ┘  (게임이 실행되는 동안 반복)
    //    ↓
    // OnDisable()    (비활성화될 때마다)
    //    ↓
    // OnDestroy()    (오브젝트 파괴 시)


    // MonoBehaviour?
    // - 유니티에서 생성하는 모든 스크립트가 기본적으로 상속받는
    //  베이스 클래스 입니다.
    // - 게임 오브젝트에 컴포넌트 형태로 부착될 수 있게 해주며,
    //  다양한 생명주기(LifeCycle) 이벤트 메시지를 수신할 수 있는
    //  권한을 부여합니다

    public class Study_MonoBehaviour : MonoBehaviour
    {

        // Awake() : 스크립트 인스턴스(객체) 로드 시 1회 호출됨
        // 메모리에 할당될때 호출된다고 생각하면 됩니다.
        // 객체(컴포넌트가) 비 활성 상태여도 호출 됩니다
        private void Awake()
        {
            Debug.Log("Awake()가 호출 되었습니다");
        }

        // Start() : 첫 Update(= 첫 프레임) 직전 1회 호출 됩니다. 
        // 비 활성화 상태일때는 호출되지 않습니다.
        // Scene(장면)의 다른 객체 상태를 참고하여
        // 본인 초기화 로직에 사용하려고 할 때 사용합니다.
        private void Start()
        {
            Debug.Log("Start()가 호출 되었습니다");
        }

        // Update()를 어떻게 활용하는지 살펴봅시다
        // public 접근제한자를 사용하면 인스펙터에 노출이 됩니다.
        public float speed = 2.0f;
        public float countDown = 5;
        public float waitTime = 0.0f;

        public GameObject triangle;

        // Update() : 매 프레임마다 호출되는 이벤트 함수 입니다.
        // 프레임 속도(결국 사양)에 따라 호출 빈도와 주기가 다릅니다.
        // GameObject와 컴포넌트가 모두 활성화 일때 호출 됩니다.
        private void Update()
        {
            Debug.Log("Update()가 호출 되었습니다");

            // Scene에 있는 Triangle이 지정된 시간이 지나면
            // 위로 상승하는 간단한 로직을 구현해 봅시다.

            waitTime += Time.deltaTime;
            if(waitTime > countDown)
            {
                triangle.transform.position =
                    triangle.transform.position + (Vector3.up * speed * Time.deltaTime);
            }
        }

        // FixedUpdate() : 고정된 시간 간격(Fixed Timestep)마다 호출됨
        private void FixedUpdate()
        {
            Debug.Log("FixedUpdate()가 호출 되었습니다");
        }

        // LateUpdate() : 모든 Mono의 Update가 종료된 후에 호출 됨
        private void LateUpdate()
        {
            Debug.Log("LateUpdate()가 호출 되었습니다");
        }

        // OnEnable() : 객체가 활성화 상태로 전환될 때마다 호출됨'
        // Scene이라는 무대에 등장 할 때
        private void OnEnable()
        {
            Debug.Log("OnEnable()가 호출 되었습니다");
        }

        // OnDisable() : 객체가 비활성화 상태로 전환될 때마다 호출됨
        // Scene이라는 무대에서 퇴장 할 때
        private void OnDisable()
        {
            Debug.Log("OnDisable()가 호출 되었습니다");
        }

        // OnDestroy() : 오브젝트가 소멸되기 직전 1회 호출됨.
        // Scene이라는 극장 밖으로 나갈때
        private void OnDestroy()
        {
            Debug.Log("OnDestroy()가 호출 되었습니다");
        }
    }
}

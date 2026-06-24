using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class UFOController_2 : MonoBehaviour
{
    // 위,아래 움직임 기능 넣어보기
    // 기본적으로 하강을 하지만 스페이스를 눌렀을때는 상승

    public float Speed = 1.0f;

    public int CoinCount = 0;

    public GameObject coin;
    public TextMeshPro clearMessage;
    public TextMeshPro countMessage;

    private bool isColliding = false;
    private bool isShown = false;

    private SpriteRenderer spriterenderer;
    

    private void Awake()
    {
        clearMessage.gameObject.SetActive(false);
        countMessage.gameObject.SetActive(false);
        spriterenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //기본적으로 하강
        Vector3 moveVector = new Vector3(0, -1, 0);

        // Space를 누르면 상승한다
        if (Keyboard.current.spaceKey.isPressed)
        {
            moveVector = moveVector * (-1f); // -1 * -1 = +1;
        }

        //속도 * 프레임당 이동량을 구해준다
        moveVector = moveVector * Speed * Time.deltaTime;

        transform.position += moveVector;
        if (CoinCount == 15&&isShown==false)
        {
            isShown = true;
            StartCoroutine(CountMessage());
        }

        if (CoinCount < 15) isShown = false;

        if (CoinCount > 30) clearMessage.gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        bool IsCoinTag = collider.gameObject.CompareTag("Coin");
        if ((isColliding==false)&&IsCoinTag)
        {
            Destroy(collider.gameObject);
            CoinCount += 1;
        }
        else if ((isColliding == false) && (collider.gameObject.CompareTag("Obstacle")))
        {
            isColliding = true;
            StartCoroutine(CoinDestroy());
            StartCoroutine(CollisionEffect());
        }
    }

    // # Coroutine?
    // - 시간의 흐름에 따라 순차적으로 진행되는 로직을 Update 함수 없이 깔끔하게 작성
    // 하게 해주는 유니티의 강력한 기능입니다.
    // - 함수의 실행을 잠시 멈추고, 특정조건 후 멈춘 지점부터 다시 시작할 수 있는
    // 함수입니다.
    // - Coroutine으로 선언된 함수는 그냥 실행할 수 없으며, StartCoroutine()이라는 함수를 통해서만 실행
    
    private IEnumerator CollisionEffect()
    {
        Color origin = spriterenderer.color;
        Color effect = spriterenderer.color;
        effect.a = 0.2f;
        float term = 0.1f;

        for (int i = 0; i < 10; ++i)
        {
            spriterenderer.color = effect;
            yield return new WaitForSeconds(term);
            spriterenderer.color = origin;
            yield return new WaitForSeconds(term);
        }
        isColliding = false;
    }

    private IEnumerator CountMessage()
    {
        countMessage.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        countMessage.gameObject.SetActive(false);
    }

    private IEnumerator CoinDestroy()
    {
        if (CoinCount <= 0)
        {
            CoinCount = 0;
            yield break;
        }
        CoinCount--;
        GameObject minusCoin = Instantiate(coin, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        Destroy(minusCoin.gameObject);
    }

}
using DG.Tweening;
using Study.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study_DoTween
{
    public class B_CoinExample : MonoBehaviour
    {
        private void Update()
        {
            if (SimpleInput.GetKeyDown(Key.Digit1))
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                Sequence seq = DOTween.Sequence()
                    .Append(transform.DOMove(transform.position + Vector3.up, 0.3f))
                    .Join(transform.DOScale(1.4f, 0.3f))
                    .Append(spriteRenderer.DOFade(0f, 0.3f))
                    .AppendCallback(() => Debug.Log("코인 획득! 점수 +10"))
                    .OnComplete(() => Debug.Log("연출 종료(오브젝트 파괴)"))
                    .SetLink(gameObject);
            }
        }
    }
}
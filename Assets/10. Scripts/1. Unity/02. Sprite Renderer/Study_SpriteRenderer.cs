using UnityEngine;
using UnityEngine.InputSystem;

namespace Study.Sprite_Renderer
{
    public class Study_SpriteRenderer : MonoBehaviour
    {
        // SpriteRenderer
        // - 2D 이미지인 "스프라이트(Sprite)"를 씬에 렌더링(그려주는)
        // 역할을 담당. GameObject에 부착ㄷ외어 2D 이미지를 화면에 표시하고,
        // 색상, 정령 순서 등을 제어할 수 있게 해준다.

        // Field 설명
        // 1. Sprite : 렌더링할 실제 Sprite 에셋을 지정합니다. 프로젝트 창에서
        //            Sprite 에셋을 드래그하여 할당할 수 있습니다.
        // 2. Color  : 스프라이트의 색상과 투명도(Alpha)를 조절 합니다. 흰색(기본값)
        //            원본 스프라이트 생상을 유지하며, 다른 생상을 적용하면 스프라이트가
        //            해당 색상으로 틴트(tint)됩니다.
        // 3. Flip   : 스프라이트를 X축 또는 Y축으로 뒤집을 수 있습니다.
        //            (캐릭터가 왼쪽/오른쪽 바라보게 할 때 사용)
        // 4. Material : 스프라이트를 렌더링하는데 사용될 Material을 지정합니다. 일반적으로는
        //              기본을 사용합니다
        // 5. Order in Layer : 같은 Layer 그룹 내에서 스프라이트 렌더링 순서를 지정합니다.
        //                    숫자가 높을수록 더 앞에 그려집니다(나중에 렌더링됩니다).

        public SpriteRenderer spriteRenderer;


        private void Update()
        {
            Flip();
            UpdateColor();
        }

        private void Flip()
        {
            if(Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                spriteRenderer.flipX = true;
            }
            else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                spriteRenderer.flipX = false;
            }
        }

        public Color colorA;
        public Color colorB;
        
        private Color startColor;   // 시작 컬러 값
        private Color goalColor;    // 목표 컬러 값

        public float waitTime = 0.0f;
        public float fadeTime = 3.0f;

        [Range(0, 1)]
        public float progress = 0.0f;

        private void UpdateColor()
        {
            // 위쪽화살표가 눌렸을 때는 goalColor를 colorA로 설정
            // 아래쪽 화살표가 눌렸을 때는 goalColor를 colorB로 설정
            // 매 Update마다 startColor => goalColor로 spriteRenderer의 color값을 변경

            waitTime += Time.deltaTime;

            // waitTime / fadeTime
            spriteRenderer.color = Color.Lerp(startColor, goalColor, waitTime / fadeTime);
            progress = waitTime / fadeTime;

            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                startColor = colorB;
                goalColor = colorA;
                waitTime = 0.0f;
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                startColor = colorA;
                goalColor = colorB;
                waitTime = 0.0f;
            }
        }
    }
}
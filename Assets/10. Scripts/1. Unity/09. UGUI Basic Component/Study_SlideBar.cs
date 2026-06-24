using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Study_SliderBar
{
    public class Study_SlideBar : MonoBehaviour
    {
        private Slider[] Sliders { get; set; }
        // private 프로퍼티는 나만 접근하고 나만 수정할 수 있는 필드입니다

        [field: SerializeField] TMP_Text[] sliderTexts { get; set; }


        private void Awake()
        {
            Sliders = GetComponentsInChildren<Slider>();
        }

        private void Update()
        {
            //UpdateSliderText();
        }

        private void UpdateSliderText()
        {
            // 모든 슬라이더의 value들을 Text에 표현합니다.

            for(int i = 0; i < sliderTexts.Length; ++i)
            {
                // 인덱스에 알맞는 Slider 객체를 가져옴
                Slider targetSlider = Sliders[i];
                TMP_Text targetText = sliderTexts[i];

                // text에 slider의 value를 넣어 줌
                //targetText.text = targetSlider.value.ToString
                string text = $"{targetSlider.value:F2}";
                targetText.SetText(text);
                
                // SetText 함수가 더 효율적
            }
        }

        // 매 순간 바꾸는게 아니라 수치가 변경될때만 Update를 해봅시다.
        public void OnChangedvalue(float value)
        {
            UpdateSliderText();
        }
    }
}


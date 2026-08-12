using System;
using UnityEngine;

namespace Study.MiniFarm
{
    public class Crops : MonoBehaviour
    {
        public Sprite[] CropSprites;
        public SpriteRenderer Crop;
        public int requiredDay;
        private int myDay = 0;

        private int currentCrop = 0;
        // 내 생각은 일단 작물 이미지를 배열로 가져와.
        // 기본이 씨앗이야.
        // 그리고 일수가 변하면 그 다음 작물 이미지를 가져오는거야.

        private void Awake()
        {
            Crop =  GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            Crop.sprite = CropSprites[currentCrop];
            WorldClock.Instance.OnDayStarted += IsDayChanged;
        }
        
        
        private void IsDayChanged(int day)
        {
            myDay++;
            if (currentCrop >= CropSprites.Length - 1) return;
            if (myDay % requiredDay == 0)
            {
                currentCrop++;
                Crop.sprite = CropSprites[currentCrop];
            }
            
        }
    }
}
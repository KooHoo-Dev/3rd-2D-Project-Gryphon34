using System;
using Jay;
using UnityEngine;

namespace Study.MiniDefence
{
    public class EconomyManager : SingletonBase<EconomyManager>
    {
        // 런타임에 조회하는 현재 플레이어의 Mineral양
        public int Minerals { get; private set; }

        // 잔액이 바뀔 때마다 구독자에게 "현재 잔액"을 알려주는 이벤트 변수 
        public event Action<int> MineralsChanged;

        protected override void OnInitialize()
        {
            Minerals = GameSettings.Instance.StartingMinerals;
        }

        // amount만큼 지불할 수 있는가?
        // BuildMenu에서 UI가 확인(구매) 버튼을 켤지말지 조회할때 사용합니다
        public bool CanAfford(int amount)
        {
            return Minerals >= amount;
        }

        // 소비하는 함수
        // 현재는 유닛을 놓을때만 소비한다.
        public bool TrySpend(int amount)
        {
            // 구매 가능한지 체크를 해보고
            if (CanAfford(amount) == false) return false;

            // 차감 로직을 진행하고
            Minerals -= amount;
            // 구독자들에게 잔액을 알려준다
            MineralsChanged?.Invoke(Minerals);
            return true;
        }

        // 벌었다
        // 적개체를 죽일때, 웨이브를 클리어했을때, 유닛을 팔았을때
        public void Earn(int amount)
        {
            if(amount <= 0) return;
            Minerals += amount;
            MineralsChanged?.Invoke(Minerals);
        }
    }
}
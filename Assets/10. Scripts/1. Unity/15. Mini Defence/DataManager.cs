using System;
using Jay;
using UnityEngine;

namespace Study.MiniDefence
{
    //[Facade Patern] 데이터 표들이 들어가는 단일 창구
    // 데이터 종류별 테이블(Enemy, Wave, Unit)을 "보유"하고
    // 로딩 순서를 조율하는 역할입니다.
    // 실제 로드/해석/보관/조회는 각각의 테이블이 책임집니다
    
    // 새 데이터 종류를 추가하고 싶으면
    // 1. Table클래스를 만들고
    // 2. DataManager에 추가 하면 되는 구조 입니다
    public class DataManager : SingletonBase<DataManager>
    {
        public EnemyTable Enemies { get; } = new EnemyTable();
        public WaveTable Waves { get; } = new WaveTable();
        public UnitTable Units { get; } = new UnitTable();
        
        // 혹시 모를 사항을 위해 준비해둔 로딩 완료 체크 플래그
        public bool IsReady { get; private set; } = false;
        public event Action OnReady; // 로딩 완료 통지

        private void Start()
        {
            Load();
        }

        private void Load()
        {
            Enemies.Load();
            Waves.Load(Enemies);
            Units.Load();
            IsReady = true;
            OnReady?.Invoke();
            Debug.Log($"[DataManager] Loaded {Enemies.Count} enemies, {Waves.Count} waves, {Units.Count} units.");
        }
        
        // 편의 조회 함수들
        public EnemyDef GetEnemyDef(int id) => Enemies.Get(id);
        public WaveDef GetWaveDef(int id) => Waves.Get(id);
        public UnitDef GetUnitDef(int id) => Units.Get(id);
    }
    
}

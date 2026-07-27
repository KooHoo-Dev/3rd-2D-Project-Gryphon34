using System;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study.MiniDefence
{
    // Wave를 테스트 하는 객체
    // 개발할때 사용을 하며 실제 릴리즈된 내용에는 없다 
    
    public class WaveTester : MonoBehaviour
    {
        public WaveManager waveManager;
        public MapData map;
        
        // 작업하기 편하도록 인스펙터 수정가능한 데이터를 맹글어 둔다
        [SerializeField] private WaveDef waveDef;
        [SerializeField] private EnemyDef enemyDef;

        private void Update()
        {
            // 개발용 테스트를 위한 코드
            if (SimpleInput.GetKeyDown(Key.Digit1))
            {
                // 테스트를 위해 Enemy 정보를 주입한다.
                waveDef.Enemy = enemyDef;
                waveManager.StartWave(waveDef, map);
            }
        }

        private int PlayerHp = 30;

        private void Start()
        {
            waveManager.EnemyReachedGoal += PlayerHpUpdate;
        }

        private void PlayerHpUpdate(Transform enemyTransform, EnemyDef enemyDef)
        {
            PlayerHp -= enemyDef.LeakDamage;
            if (PlayerHp <= 0)
            {
                waveManager.EnemyReachedGoal -= PlayerHpUpdate;
                Debug.Log("Game Over");
            }
        }
    }
}
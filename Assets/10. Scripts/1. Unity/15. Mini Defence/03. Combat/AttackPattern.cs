using System.Collections.Generic;
using Study.MiniDefence._3._Combat;
using UnityEditor;
using UnityEngine;

namespace Study.MiniDefence
{
    // [Strategy Pattern] 고른 대상을 "어떻게 때릴지"
    // 조준(누구를)과 공격 방식(어떻게)는 서로 다른 축입니다.
    // 그래서 각각 따로 전력패턴을 이용해서 구현하면, "최근접 + 광역"같은 조합을 만들 수도 있습니다
    
    // 피해량와 크리티컬 여부는 공격하는 쪽(Unit)이 미리 설정하여 넘겨주고
    // AttackPattern에 따라서 공격이 적용되는 "그 순간, 그 자리"에 설정에따른
    // 피해가 적용될 수 있도록 처리합니다.
    public interface IAttackPattern
    {
        void Fire(Unit attacker, Enemy target, int damage, bool isCritical);
    }

    // 고른 대상 하나만 때린다.
    public class SingleAttack : IAttackPattern
    {
        public void Fire(Unit attacker, Enemy target, int damage, bool isCritical)
        {
            // 발사기가 없는 유닛은 즉시 명중한다
            if (attacker.Launcher == null)
            {
                target.TakeDamage(damage, isCritical);
                return;
            }
            
            // 델리게이트로 전달되는 매개변수가 만약 람다라면
            // C#의 명명된 매개변수를 사용해서 넘기면 가독성이 올라갑니다.
            
            // 명명된 매개변수 (Named Arguments)?
            // : 함수를 부를 때 순서대신, 매개변수 이름을 직접 적어서 값을
            //  넣을 수 있는 기능입니다. 가독성 향상, 순서 무관 호출 등등의
            //  장점이 있지만 더 기입할게 많아져서 귀찮은 감이 있습니다.
            //  저는 단순한 람다를 매개변수로 전달하는것을 표현할때만 보통 사용합니다.
            attacker.Launcher.Launch(target,
                onArrive: vector3 =>
                {
                    if(target != null && target.IsDead == false)
                        target.TakeDamage(damage, isCritical);
                }
                );
        }
    }

    
    public class SplashAttack : IAttackPattern
    {
        // 스플래쉬 어택당 1개의 buffer를 사용한다(bucket도 가능한 표현)
        private List<Enemy> buffer = new();
        
        public void Fire(Unit attacker, Enemy target, int damage, bool isCritical)
        {
            // 테이블의 내용을 값복사 한다.
            float radius = attacker.Def.SplashRadius;
            int maxTargets = attacker.Def.MaxTargets;

            // 발사기가 없으면 조준한 적의 자리에서 즉시 폭발한다
            if (attacker.Launcher == null)
            {
                SplashAt(target.transform.position, radius, maxTargets, damage, isCritical);
                return;
            }
            
            attacker.Launcher.Launch(target,
                onArrive: position =>
                {
                    SplashAt(position, radius, maxTargets, damage, isCritical);
                });
        }

        // 한 지점(center)를 중심으로 반경 안의 적들을 때린다.
        private void SplashAt(Vector3 center, float radius, 
            int maxTargets, int damage, bool isCritical)
        {
            // EnemyRegistry의 함수를 이용해서 buffer를 조건에 맞게 채운다
            EnemyRegistry.FindInRange(center, radius, ref buffer);
            
            int limit = Mathf.Max(1, maxTargets);
            int count = Mathf.Min(limit, buffer.Count);

            // 버퍼에 10가 있더라고 count만큼 반복하게된다
            for (int i = 0; i < count; i++)
            {
                Enemy enemy = buffer[i];
                enemy.TakeDamage(damage, isCritical);
            }
        }
    }
    
    
    // [Factory] 표에 적힌 UnitKind를 실제 공격 방식으로 바꿔 준다
    public static class AttackPatternFactory
    {
        public static IAttackPattern Create(UnitKind kind)
        {
            return kind switch
            {
                UnitKind.Cannon => new SplashAttack(),
                _ => new SingleAttack(), // 캐논 빼고는 전부다 단일 대상 
            };
        }
    }
    
}
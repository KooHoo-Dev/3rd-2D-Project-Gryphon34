using System.Collections.Generic;
using UnityEngine;

namespace Study_OOP_And_Design_Pattern.Creational
{
    // Builder - 예시 : 캐릭터 빌더
    // 상황 : 옵션이 많은 캐릭터를 단계별로 조립한다.
    // 형태 : 각 Setter(Set함수)가 this를 반환하는 형태로 구성
    
    public class Character
    {
        public string Name { get; set; } = "이름 없음";
        public int Hp { get; set; } = 1;
        public int Atk { get; set; } = 1;
        public List<string> Skills = new List<string>();
    }
    
    public class CharacterBuilder
    {
        private Character character;
        
        // 생성자에서 character를 할당 받으면 기존에 있던 객체도 다시 수정 가능해진다
        // 주의 : 불변성을 가지고 있는 필드들은 생성자에서 받아야 합니다.
        public CharacterBuilder() { character = new Character(); }
        public CharacterBuilder(Character  _character) { character = _character; }
        
        /*
         *public string Name { get; set; } = "이름 없음";
        public int Hp { get; set; } = 1;
        public int Atk { get; set; } = 1;
        public List<string> Skills = new List<string>();
         *
         */

        public CharacterBuilder SetName(string name)
        { character.Name = name; return this; }

        public CharacterBuilder SetHp(int hp)
        { character.Hp = hp; return this; }

        public CharacterBuilder SetAtk(int atk)
        { character.Atk = atk; return this; }

        public CharacterBuilder AddSkill(string skill)
        { character.Skills.Add(skill); return this; }

        public Character Build() { return character; }
    }

    public class CharacterBuilderDemo : MonoBehaviour
    {
        private void Start()
        {
            Character hero = new CharacterBuilder()
                .SetName("Kang")
                .SetAtk(100)
                .SetHp(1000)
                .AddSkill("몰루")
                .Build();

            Character enemy = new CharacterBuilder()
                .SetName("망자")
                .SetAtk(70)
                .SetHp(500)
                .AddSkill("공격")
                .Build();
        }
    }
}
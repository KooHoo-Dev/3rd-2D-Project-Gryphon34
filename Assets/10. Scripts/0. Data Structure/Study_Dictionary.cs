using UnityEngine;
using System.Collections.Generic; // 자료 구조사용할면 해당 네임스페이스 사용

namespace Study_DateStructure
{
    public class Study_Dictionary : MonoBehaviour
    {
        private void Start()
        {
            StudyDictionary();
        }

        private void StudyDictionary()
        {
            // # Dictionary<TKey, TValue>는 '이름표가 붙은 사물함' 또는 '전화번호부'
            // - 키(Key)와 값(Value)의 쌍(Pair) : 각 사물함에는(Value)에는 고유한
            //   이름표(Key)가 붙어있습니다. 이 이름표를 통해서 원하는 사물하믜
            //   내용물을 아주 빨리 찾을 수 있습니다.
            // - 고유한 키 : 모든 이름표는 서로 달라야합니다. 같은 이름표를
            //   가진 사물함은 절대 있을 수 없습니다. 하지만 사물함의 내용물(Value)
            //   은 중복 될 수 있습니다.
            // - 빠른 검색 : 이름표만 알면 원하는 사물함을 거의 즉시 찾을 수 있습니다

            // 1. Dictionary의 생성 : 키와 값의 타입을 알려주기
            // string(이름)을 Key로, int(나이)를 Value로 하는 Dictionary를 만들어 봅시다
            // 이름은 'studentAge'

            Dictionary<string, int> studentAge;
            studentAge = new Dictionary<string, int>();

            studentAge.Add("김철수", 20);
            studentAge.Add("이영희", 21);
            studentAge.Add("박민수", 22);

            // 데이터 접근 : Key를 사용하거나, TryGetValue()
            // 이름표(Key)를 통해 나이(Value)를 찾아봅시다.
            Debug.Log($"3. 데이터 접근 : 특정 학생의 나이 찾아보기");
            Debug.Log($"김철수의 나이 : {studentAge["김철수"]}");
            Debug.Log($"박민수의 나이 : {studentAge["박민수"]}");
            Debug.Log("");

            // 없는 Key로 데이터에 접근하면?
            Debug.Log($"3.1키 존재 여부 확인");
            if(studentAge.TryGetValue("이영희",out int yhAge))
            {
                Debug.Log($"이영희의 나이 : {yhAge}");
            }

            // Contains 키 존재 여부 확인

            // 데이터 수정 : Key를 사용하여 접근 후 수정
            Debug.Log($"4. 데이터 수정 : 김철수");
            studentAge["김철수"] = 30;
            Debug.Log($"수정된 김철수의 나이 : {studentAge["김철수"]}");
        }
    }
}

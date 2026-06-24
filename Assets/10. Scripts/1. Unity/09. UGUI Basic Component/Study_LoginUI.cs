using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

namespace Study_LoginUI
{
    public class Study_LoginUI : MonoBehaviour
    {
        [field: SerializeField]
        private TMP_InputField EmailField { get; set; }

        [field: SerializeField]
        private TMP_InputField PasswordField { get; set; }

        [field: SerializeField]
        private Button LoginButton { get; set; }

        [field: SerializeField]
        private Toggle AutoLoginToggle { get; set; }

        private Selectable[] selectables;
        private int index = 0;

        private List<User> userDB = new List<User>();
        private HashSet<string> domains = new HashSet<string>();

        private void Awake()
        {
            selectables = new Selectable[] { EmailField, PasswordField, LoginButton };
            index = -1;
            CreateUser();
            AutoLoginToggle.isOn = false;
        }

        private void Update()
        {
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                index += 1;
                if (index >= selectables.Length) index = 0;
                selectables[index].Select();
            }
        }

        private string inputEmail;
        private string inputPassword;

        public void OnEndEditEmail(string email)
        {
            index = 0;
            Debug.Log($"입력한 이메일 : {email}");
            inputEmail = email;
        }

        public void OnEndEditPassword(string password)
        {
            index = 1;
            Debug.Log($"입력한 이메일 : {password}");
            inputPassword = password;
        }

        public void OnClickLoginButton()
        {
            index = 2;
            Debug.Log($"로그인 버튼 클릭!");

            if (string.IsNullOrEmpty(inputEmail) || string.IsNullOrEmpty(inputPassword))
            {
                Debug.Log($"이메일과 비밀번호를 입력해 주세요");
                return;
            }
            string domain = inputEmail.Split('@')[1];
            if (domains.Contains(domain) == false)
            {
                Debug.Log("도메인이 다릅니다");
                return;
            }

            if (TryGetUser(inputEmail,out User user) == false)
            {
                Debug.Log($"{inputEmail}은 존재하지 않는 ID입니다");
                return;
            }

            if (user.Password.Equals(inputPassword) == false)
            {
                Debug.Log($"비밀번호가 일치하지 않습니다");
                return;
            }
            
        }


        private bool TryGetUser(string email,out User user)
        {
            user = null;
            for(int i = 0; i < userDB.Count; ++i)
            {
                if(userDB[i].ID.Equals(email))
                {
                    user = userDB[i];
                    return true;
                }
            }
            return false;
        }



        private void CreateUser()
        {
            userDB.Add(new User("koohoo89@gmail.com", "MyP@ssw0rd!"));
            userDB.Add(new User("alex_smith@naver.com", "Al3x2026!!"));
            userDB.Add(new User("happy_min@kakao.com", "H@ppyM1n99"));
            userDB.Add(new User("vr_pioneer@gmail.com", "Virtu@lR3al"));
            userDB.Add(new User("starlight22@naver.com", "St@rL1ght*"));

            userDB.Add(new User("unity_dev@kakao.com", "Un1tyD3v#"));
            userDB.Add(new User("coffee_lover@gmail.com", "C0ff33L0v3"));
            userDB.Add(new User("blue_sky_77@naver.com", "Blu3Sky77&"));
            userDB.Add(new User("mr_creator@kakao.com", "M!xedR3al"));
            userDB.Add(new User("night_owl_88@gmail.com", "N1ght0wl^"));

            userDB.Add(new User("gamer_x@naver.com", "G@m3rX001"));
            userDB.Add(new User("pixel_master@kakao.com", "P1x3lM@st3r"));
            userDB.Add(new User("tech_guru@gmail.com", "T3chGuru$"));
            userDB.Add(new User("sunny_day@naver.com", "SunnY_D@y"));
            userDB.Add(new User("code_ninja@kakao.com", "C0d3N1nj@!"));

            userDB.Add(new User("silver_lining@gmail.com", "S1lv3rL1n"));
            userDB.Add(new User("quiet_river@naver.com", "Qu13tR1v3r"));
            userDB.Add(new User("mountain_climber@kakao.com", "M0unt@1nC!"));
            userDB.Add(new User("city_lights@gmail.com", "C1tyL1ghts$"));
            userDB.Add(new User("fast_runner@naver.com", "F@stRunn3r"));

            for(int i = 0; i < userDB.Count; ++i)
            {
                string userEmail = userDB[i].ID;
                string domain = userEmail.Split('@')[1];
                //"gamer_x@naver.com" 을 @로 split하면
                // {gamer_x,naver.com}
                domains.Add(domain);
                //HashSet에 다 담아보면 모든 유저의 도메인들 종류가 담긴
                // 
            }
        }

        public void OnClickAutoLogin(bool isOn)
        {
            Debug.Log($"OnClickAutoLoginToggle = {isOn}");
        }
    }

    

    public class User
    {
        public string ID { get; private set; }
        public string Password { get; private set; }

        public User(string id, string password)
        {
            ID = id;
            Password = password;
        }

        public override string ToString()
        {
            return $"User : ID = {ID}, PW = {Password}";
        }
    }
}


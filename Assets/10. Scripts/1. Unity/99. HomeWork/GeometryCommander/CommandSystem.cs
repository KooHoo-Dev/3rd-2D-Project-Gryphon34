using Study.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Geometry
{
    public class CommandSystem : MonoBehaviour
    {
        /*
         * 맞다 아니다를 입력할때마다 판단하고 있는데
         * 틀리면 다시 초기화 하고 커맨드 입력
         * 맞으면 카운트를 올려서
         * commandList의 
         * 
         * 
         * 순서는 버튼을 눌러
         * 그러면 커맨드 창 초기화 되서 커맨드 리스트에 저장해
         * 그리고 유저는 입력을 해
         * 입력 할 때 마다 확인해
         * 맞으면 계속 해
         * 틀리면 다시 커맨드 창 초기화 해서 커맨드 리스트에 저장해
         * 그리고 유저는 입력을 해 반복해
         * 
         * 하다가 커맨드 리스트의 길이만큼 맞으면
         * 큐브를 소환해
         */

        enum Arrow
        {
            Up,
            Down,
            Left,
            Right,
        }

        enum SpawnType
        {
            Triangle = 5,
            Circle = 7,
            Square = 9,
            Support = 12
        }

        public enum CommandState
        {
            Idle,
            Commanding,
            Spawning
        }

        private int count = 0;
        private bool IsCorrect = false;
        private CommandState currentState = CommandState.Idle;

        private Arrow[] arrows = new Arrow[4];

        private SpawnType spawnType;
        private List<Arrow> commandList = new List<Arrow>();
        private List<Arrow> InputCommand = new List<Arrow>();
        private List<Image> spawnedArrowImages = new List<Image>();

        private GameObject spawn;
        public GameObject trianglePre;
        public GameObject circlePre;
        public GameObject squarePre;
        public GameObject supportPre;

        public GameObject[] arrowPre;

        public Transform arrowUI;
        private void Awake()
        {
            arrows[0] = Arrow.Up;
            arrows[1] = Arrow.Down;
            arrows[2] = Arrow.Left;
            arrows[3] = Arrow.Right;
        }

        private void Update()
        {
            switch (currentState)
            {
                case CommandState.Idle:
                    break;
                case CommandState.Commanding:
                    PlayerCommand();
                    break;
                case CommandState.Spawning:
                    if (Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        spawnedArrowImages.Clear();
                        SetSpawnPosition();
                    }
                    break;
            }
        }

        public void OnTriangleClick()
        {
            if (currentState != CommandState.Idle) return;
            // 자원과 쿨타임 체크 로직
            // if(currentResource <triangleCost) return;
            // if(isTriangleCooldown) return;
            InitialCommand(SpawnType.Triangle);
            currentState = CommandState.Commanding;
        }

        public void OnCircleClick()
        {
            if (currentState != CommandState.Idle) return;
            // 자원과 쿨타임 체크 로직
            // if(currentResource <circleCost) return;
            // if(iscircleCooldown) return;
            InitialCommand(SpawnType.Circle);
            currentState = CommandState.Commanding;
        }

        public void OnSquareClick()
        {
            if (currentState != CommandState.Idle) return;
            // 자원과 쿨타임 체크 로직
            // if(currentResource <squareCost) return;
            // if(issquareCooldown) return;
            InitialCommand(SpawnType.Square);
            currentState = CommandState.Commanding;
        }

        public void OnSupportClick()
        {
            if (currentState != CommandState.Idle) return;
            // 자원과 쿨타임 체크 로직
            // if(currentResource <supportCost) return;
            // if(issupportCooldown) return;
            InitialCommand(SpawnType.Support);
            currentState = CommandState.Commanding;
        }

        private void PlayerCommand()
        {
            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                InputCommand.Add(Arrow.Up);
                CheckCommand();
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                InputCommand.Add(Arrow.Down);
                CheckCommand();
            }
            else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                InputCommand.Add(Arrow.Left);
                CheckCommand();
            }
            else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                InputCommand.Add(Arrow.Right);
                CheckCommand();
            }
        }

        private void CheckCommand()
        {
            if (commandList[count] == InputCommand[count])
            {
                spawnedArrowImages[count].color = Color.green;
                count++;
                if (count == commandList.Count)
                {
                    Debug.Log("성공");
                    IsCorrect = true;
                    count = 0;
                }
            }
            else
            {
                Debug.Log("커맨드 실패");
                commandList.Clear();
                InputCommand.Clear();
                count = 0;

                foreach (Transform child in arrowUI)
                {
                    Destroy(child.gameObject);
                }
                spawnedArrowImages.Clear();
            }
        }

        private void InitialCommand(SpawnType type)
        {
            commandList.Clear();
            InputCommand.Clear();
            count = 0;
            spawnType = type;

            for (int i = 0; i < (int)type; ++i)
            {
                int randNum = Random.Range(0, 4);
                commandList.Add(arrows[randNum]);
            }
            DrawCommad();
            Debug.Log($"{string.Join(",", commandList)}");
        }

        private void SetSpawnPosition()
        {
            Vector2 mousePosition = SimpleInput.GetMousePosition();

            // 화면 좌표계에서 World(Scene)의 좌표계로 전환합니다.
            // 전환 할 때는 Camera의 기능을 이용합니다.

            Vector3 worldMousePoint = Camera.main.ScreenToWorldPoint(mousePosition);
            worldMousePoint.z = transform.position.z; // z값을 보정
                                                      // 마우스가 클릭한 곳에 소환

            switch (spawnType)
            {
                case SpawnType.Triangle:
                    spawn = trianglePre;
                    break;
                case SpawnType.Circle:
                    spawn = circlePre;
                    break;
                case SpawnType.Square:
                    spawn = squarePre;
                    break;
                case SpawnType.Support:
                    spawn = supportPre;
                    break;
            }
            GameObject.Instantiate(spawn, worldMousePoint, Quaternion.identity);

            commandList.Clear();
            InputCommand.Clear();
        }

        private void DrawCommad()
        {
            foreach (Transform child in arrowUI)
            {
                Destroy(child.gameObject);
            }

            spawnedArrowImages.Clear();

            for (int i = 0; i < commandList.Count; ++i)
            {
                int Index = (int)commandList[i];

                GameObject newArrow = GameObject.Instantiate(arrowPre[Index], arrowUI, false);
                spawnedArrowImages.Add(newArrow.GetComponent<Image>());
            }
        }

    }
}
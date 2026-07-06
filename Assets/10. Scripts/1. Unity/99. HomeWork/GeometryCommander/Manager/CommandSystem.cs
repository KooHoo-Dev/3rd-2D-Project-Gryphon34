using Michsky.UI.Shift;
using Study.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Geometry
{
    public class CommandSystem : MonoBehaviour
    {
        enum Arrow
        {
            Up,
            Down,
            Left,
            Right,
        }

        public enum CommandState
        {
            Idle,
            Commanding,
            Spawning
        }
        

        private int count = 0;
        private bool IsSupport = false;

        private CommandState currentState = CommandState.Idle;
        private Arrow[] arrows = new Arrow[4];

        private PlayerManager.UnitType spawnType;
        private List<Arrow> commandList = new List<Arrow>();
        private List<Arrow> InputCommand = new List<Arrow>();
        private List<Image> spawnedArrowImages = new List<Image>();

        public PlayerManager playerManager;

        private GameObject spawn;
        public GameObject trianglePre;
        public GameObject circlePre;
        public GameObject squarePre;
        public GameObject supportPre;

        public GameObject[] arrowPre;

        public Collider2D spawnArea;

        public SpriteRenderer supportRadius;

        public Transform arrowUI;
        private void Awake()
        {
            arrows[0] = Arrow.Up;
            arrows[1] = Arrow.Down;
            arrows[2] = Arrow.Left;
            arrows[3] = Arrow.Right;

            supportRadius.enabled = false;
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
                    Vector2 mousePosition = SimpleInput.GetMousePosition();
                    Vector3 worldMousePoint = Camera.main.ScreenToWorldPoint(mousePosition);
                    worldMousePoint.z = transform.position.z;

                    if (IsSupport&&supportRadius!=null)
                    {
                        supportRadius.enabled = true;
                        supportRadius.transform.position = worldMousePoint;
                    }

                    if (Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        spawnedArrowImages.Clear();
                        SetSpawnPosition(worldMousePoint);
                        if (currentState == CommandState.Idle)
                        {
                            IsSupport = false;
                            supportRadius.enabled = false;
                        }
                    }
                    break;
            }
        }

        public void OnTriangleClick()
        {
            spawnType = PlayerManager.UnitType.Triangle;

            if (CheckCondition(spawnType) == false) return;
            InitialCommand(spawnType);
            currentState = CommandState.Commanding;
        }

        public void OnCircleClick()
        {
            spawnType = PlayerManager.UnitType.Circle;

            if (CheckCondition(spawnType) == false) return;
            InitialCommand(spawnType);
            currentState = CommandState.Commanding;
        }

        public void OnSquareClick()
        {
            spawnType = PlayerManager.UnitType.Square;

            if (CheckCondition(spawnType) == false) return;

            InitialCommand(spawnType);
            currentState = CommandState.Commanding;
        }

        public void OnSupportClick()
        {
            spawnType = PlayerManager.UnitType.Support;
            IsSupport = true;
            // 자원과 쿨타임 체크 로직
            if (CheckCondition(spawnType) == false) return;

            InitialCommand(spawnType);
            currentState = CommandState.Commanding;
        }

        private bool CheckCondition(PlayerManager.UnitType type)
        {
            if (playerManager.IsOnCooldown(spawnType))
            {
                Debug.LogWarning($"{spawnType}이 쿨다운 중입니다.");
                return false;
            }
            if (playerManager.CanAfford(spawnType) == false)
            {
                Debug.LogWarning($"자원이 부족합니다.");
                return false;
            }
            return true;
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
                    count = 0;
                    currentState = CommandState.Spawning;
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

        private void InitialCommand(PlayerManager.UnitType type)
        {
            commandList.Clear();
            InputCommand.Clear();
            count = 0;

            for (int i = 0; i < (int)type; ++i)
            {
                int randNum = Random.Range(0, 4);
                commandList.Add(arrows[randNum]);
            }
            DrawCommad();
            Debug.Log($"{string.Join(",", commandList)}");
        }

        private void SetSpawnPosition(Vector3 worldMousePoint)
        {
            if (spawnArea != null && spawnArea.OverlapPoint(worldMousePoint) == false)
            {
                return;
            }

            PlayerManager.UnitType unitType;

            switch (spawnType)
            {
                case PlayerManager.UnitType.Triangle:
                    spawn = trianglePre;
                    unitType = PlayerManager.UnitType.Triangle;
                    playerManager.UseCost(unitType);
                    break;
                case PlayerManager.UnitType.Circle:
                    spawn = circlePre;
                    unitType = PlayerManager.UnitType.Circle;
                    playerManager.UseCost(unitType);
                    break;
                case PlayerManager.UnitType.Square:
                    spawn = squarePre;
                    unitType = PlayerManager.UnitType.Square;
                    playerManager.UseCost(unitType);
                    break;
                case PlayerManager.UnitType.Support:
                    spawn = supportPre;
                    unitType = PlayerManager.UnitType.Support;
                    playerManager.UseCost(unitType);
                    break;
            }
            Collider2D[] hitUnits = Physics2D.OverlapBoxAll(spawnArea.bounds.center, spawnArea.bounds.size, 360f);

            foreach(Collider2D collider in hitUnits)
            {
                PlayerUnit targetUnit = collider.GetComponent<PlayerUnit>();

                if(targetUnit !=null && targetUnit.current_Hp > 0)
                {
                    targetUnit.current_Morale += 10.0f;
                }
            }

            GameObject.Instantiate(spawn, worldMousePoint, Quaternion.identity);
            currentState = CommandState.Idle;
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
using Study.Utilities;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private int count = 0;

    private Arrow[] arrows = new Arrow[4];

    private List<SpawnType> spawnType = new List<SpawnType>();
    private List<Arrow> commandList = new List<Arrow>();
    private List<Arrow> InputCommand = new List<Arrow>();

    public GameObject cubePre;

    private void Awake()
    {
        arrows[0] = Arrow.Up;
        arrows[1] = Arrow.Down;
        arrows[2] = Arrow.Left;
        arrows[3] = Arrow.Right;
    }

    private void Update()
    {
        if (commandList.Count>0) 
        {
            PlayerCommand();
            
        }
    }

    public void OnTriangleClick()
    {
        InitialCommand(SpawnType.Triangle);
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
            count++;
            if(count == commandList.Count)
            {
                Debug.Log("성공");
                count = 0;
            }
        }
        else
        {
            Debug.Log("커맨드 실패");
            commandList.Clear();
            InputCommand.Clear();
            count = 0;
        }
    }

    private void InitialCommand(SpawnType type)
    {
        commandList.Clear();
        InputCommand.Clear();
        count = 0;

        for(int i = 0; i < (int)type; ++i)
        {
            int randNum = Random.Range(0, 4);
            commandList.Add(arrows[randNum]);
        }
        Debug.Log($"{string.Join(",", commandList)}");
    }

    private void SetSpawnPosition(Vector3 spawnPos)
    {
        // 마우스가 클릭한 곳에 cubePre 소환
        GameObject.Instantiate(cubePre,spawnPos,Quaternion.identity);

        commandList.Clear();
        InputCommand.Clear();
    }
}
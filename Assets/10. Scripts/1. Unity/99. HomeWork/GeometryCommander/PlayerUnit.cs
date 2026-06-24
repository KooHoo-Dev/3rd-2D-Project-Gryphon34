using UnityEngine;

public abstract class PlayerUnit : MonoBehaviour
{
    // enum : 유닛_타입(동그라미_근접, 네모_근접, 세모_원거리)
    // float 변수: 최대_체력, 현재_체력, 최대_사기, 현재_사기, 기본_공격력, 사거리
    // bool 변수 : 최초_조우_상태
    // GameObject : 현재_타겟

    enum Unit_Type
    {
        Circle,
        Square,
        Triangle,
    }

    [SerializeField]
    private float maxHp;
    [SerializeField]
    private float maxMorale = 100f;
    [SerializeField]
    private float attackPower;
    [SerializeField]
    private float attackRange;
    [SerializeField]
    private Unit_Type myType;


    public float curent_Hp;
    public float current_Morale;
    private GameObject current_Target;
    public bool First_Contact = true;

    protected abstract void FindTarget();

    protected abstract void Attack();

    public void TakeDamage()
    {

    }
}

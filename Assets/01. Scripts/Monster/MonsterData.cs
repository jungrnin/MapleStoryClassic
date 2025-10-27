using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName ="SO/MonsterData", order = int.MaxValue)]

public class MonsterData : ScriptableObject
{
    [Header("Monster Info")]
    public string monsterName;
    public Sprite sprite;
    public RuntimeAnimatorController animatorController;

    [Header("Status")]
    public int maxHp = 0;
    public float moveSpeed = 0f;
    public int damage = 0;

    [Header("PatrolRange")]
    public float leftOffset = 0f;
    public float rightOffset = 0f;
}

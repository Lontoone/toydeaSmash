using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPropertyCode : MonoBehaviour
{
    public const string TEAM_CODE = "TEAMCODE";
    public const string BODY_CODE = "BODYCODE";
    public const string HEAD_CDOE = "HEADCODE";
    public const string ROOM_READY = "ROOMREADY";


    public static readonly Color[] TEAMCOLORS = { Color.red, Color.green, Color.white, Color.blue };
    public static readonly KeyCode[] JumpKeys = { KeyCode.G, KeyCode.Period };
    public static readonly KeyCode[] DashKyes = { KeyCode.LeftShift, KeyCode.Space};
    public static readonly KeyCode[] DuckKyes = { KeyCode.S, KeyCode.DownArrow};
    public static readonly KeyCode[] AttackKyes = { KeyCode.F, KeyCode.Slash };
    public static readonly KeyCode[] DefenseKyes = { KeyCode.H, KeyCode.Comma};
}

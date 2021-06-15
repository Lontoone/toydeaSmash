using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPropertyCode : MonoBehaviour
{
    public const string TEAM_CODE = "TEAMCODE";
    public const string BODY_CODE = "BODYCODE";
    public const string HEAD_CDOE = "HEADCODE";
    public const string ROOM_READY = "ROOMREADY";


    public static readonly Color[] TEAMCOLORS = { new Color(1,1,1,1),new Color(0,97,253,256)/256,
                                                  new Color(146,0,0,256)/256,new Color(255,207,19,256)/256,
                                                  new Color(165,133,255,256)/256,new Color(55,55,55,256)/256,
                                                  new Color(0,94,10,256)/256,new Color(236,76,0,256)/256
    };
    public static readonly KeyCode[] JumpKeys = { KeyCode.G, KeyCode.Period };
    public static readonly KeyCode[] DashKyes = { KeyCode.LeftShift, KeyCode.Space };
    public static readonly KeyCode[] DuckKyes = { KeyCode.S, KeyCode.DownArrow };
    public static readonly KeyCode[] AttackKyes = { KeyCode.F, KeyCode.Slash };
    public static readonly KeyCode[] DefenseKyes = { KeyCode.H, KeyCode.Comma };
}

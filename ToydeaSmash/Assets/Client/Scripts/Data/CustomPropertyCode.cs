using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPropertyCode : MonoBehaviour
{
    public const string TEAM_CODE = "TEAMCODE";
    public const string BODY_CODE = "BODYCODE";
    public const string HEAD_CDOE = "HEADCODE";
    public const string ROOM_READY = "ROOMREADY";


    public static readonly Color32[] TEAMCOLORS = { new Color32(255, 255, 255, 255),new Color32(0,97,253,255),
                                                  new Color32(146,0,0,255),new Color32(255,207,19,255),
                                                  new Color32(165,133,255,255),new Color32(55,55,55,255),
                                                  new Color32(0,94,10,255),new Color32(236,76,0,255)
    };
    public static readonly KeyCode[] JumpKeys = { KeyCode.G, KeyCode.Period };
    public static readonly KeyCode[] DashKyes = { KeyCode.LeftShift, KeyCode.Space };
    public static readonly KeyCode[] DuckKyes = { KeyCode.S, KeyCode.DownArrow };
    public static readonly KeyCode[] AttackKyes = { KeyCode.F, KeyCode.Slash };
    public static readonly KeyCode[] DefenseKyes = { KeyCode.H, KeyCode.Comma };
}

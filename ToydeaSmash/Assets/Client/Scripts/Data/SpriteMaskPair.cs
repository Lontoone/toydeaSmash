using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/sprite_mask_pair")]

public class SpriteMaskPair : ScriptableObject
{
    public SpriteMask[] pairs;

    public Sprite GetSprite(string _target)
    {
        for (int i = 0; i < pairs.Length; i++)
        {
            Debug.Log("get sprite " + _target + " " + pairs[i].spriteSheet.name);
            if (pairs[i].animation_name == _target)
            {
                return pairs[i].spriteSheet;
            }
        }
        return null;
    }

    [System.Serializable]
    public struct SpriteMask
    {
        public string animation_name;
        public Sprite spriteSheet;
    }
}

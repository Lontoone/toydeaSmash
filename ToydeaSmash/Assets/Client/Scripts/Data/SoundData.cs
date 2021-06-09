using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/SoundCollection")]
public class SoundData : ScriptableObject
{
    public static Dictionary<string, AudioClip> nameClipPairsMap = new Dictionary<string, AudioClip>();
    public NameSoundPair[] pairs;
    [System.Serializable]
    public struct NameSoundPair
    {
        public string name;
        public AudioClip clip;
    }
    public void MapInit()
    {
        for (int i = 0; i < pairs.Length; i++)
        {
            if (!nameClipPairsMap.ContainsKey(pairs[i].name))
                nameClipPairsMap.Add(pairs[i].name, pairs[i].clip);
        }
    }
}

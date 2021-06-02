using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    //public string resources_folder;

    public static void CreateImageTrail(Sprite _sp, Vector2 _pos, Vector2 _scale , Color color=default)
    {
        GameObject _trail = GCManager.Instantiate("ImageTrail", position: _pos);
        _trail.GetComponent<SpriteRenderer>().sprite = _sp;
        _trail.GetComponent<SpriteRenderer>().color = color;
        _trail.transform.localScale = _scale;
        _trail.transform.position = _pos;   
    }


}

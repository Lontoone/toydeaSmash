using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerResultPanel : MonoBehaviour
{
    public int playerData_index;

    public Image bg;

    public TextMeshProUGUI place_text;
    public TextMeshProUGUI death_text;
    public TextMeshProUGUI kill_text;
    public TextMeshProUGUI damage_text;
    public TextMeshProUGUI damage_take_text;


    //public GameObject texts_prefab;
    //public GameObject container;


    public void SetUp(int _index)
    {
        playerData_index = _index;

        //place_text.text = LocalRoomManager.instance.players[_index].playerProperty[GameResultManager.DAMAGE].ToString();

        death_text.text = LocalRoomManager.instance.players[_index].GetValue<int>(GameResultManager.DEATH).ToString();
        kill_text.text = LocalRoomManager.instance.players[_index].GetValue<int>(GameResultManager.KILL).ToString();
        damage_text.text = LocalRoomManager.instance.players[_index].GetValue<int>(GameResultManager.DAMAGE).ToString();
        damage_take_text.text = LocalRoomManager.instance.players[_index].GetValue<int>(GameResultManager.DAMAGETAKE).ToString();

        bg.color = CustomPropertyCode.TEAMCOLORS[LocalRoomManager.instance.players[_index].GetValue<int>(CustomPropertyCode.TEAM_CODE)];
    }
}

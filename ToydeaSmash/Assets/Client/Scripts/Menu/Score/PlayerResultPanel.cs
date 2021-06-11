using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        List<SortData> _data = GetDamageSortPlayerIndxe();

        place_text.text = (_data.FindIndex(x => x.index == _index) + 1).ToString();

        death_text.text = LocalRoomManager.instance.players[_index].GetValue<int>(GameResultManager.DEATH).ToString();
        kill_text.text = LocalRoomManager.instance.players[_index].GetValue<int>(GameResultManager.KILL).ToString();
        damage_text.text = LocalRoomManager.instance.players[_index].GetValue<int>(GameResultManager.DAMAGE).ToString();
        damage_take_text.text = LocalRoomManager.instance.players[_index].GetValue<int>(GameResultManager.DAMAGETAKE).ToString();

        bg.color = CustomPropertyCode.TEAMCOLORS[LocalRoomManager.instance.players[_index].GetValue<int>(CustomPropertyCode.TEAM_CODE)];
    }

    List<SortData> GetDamageSortPlayerIndxe()
    {
        List<SortData> _res = new List<SortData>();
        for (int i = 0; i < LocalRoomManager.instance.players.Count; i++)
        {
            SortData _data = new SortData();
            _data.index = i;
            _data.data = LocalRoomManager.instance.players[i].GetValue<int>(GameResultManager.KILL);
            _res.Add(_data);
        }
        _res.Sort((x, y) => y.data.CompareTo(x.data));
        return _res;
    }
    private class SortData
    {
        public int index;
        public int data;
    }
}

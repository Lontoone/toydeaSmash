using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class PlayerLifeStockControl : MonoBehaviour
{
    public TextMeshProUGUI lifeStock_number_text;
    public Image panel;
    public Animator revive_animation;
    public Animator popup_animation;
    public void SetUp(int _playerIndex)
    {
        int _teamCode = 0;
        int _lifeStock = 0;
        if (PhotonNetwork.IsConnected)
        {
            Player _p = LocalRoomManager.instance.players[_playerIndex].GetValue<Player>(CustomPropertyCode.PLAYER);
            _teamCode = (int)_p.CustomProperties[CustomPropertyCode.TEAM_CODE];

            //prevant lifestock data is not uploaded.
            var _lifeStockData = _p.CustomProperties[CustomPropertyCode.LIFESTOCK];
            if (_lifeStockData == null)
            {
                _lifeStock = LocalRoomManager.instance.players[_playerIndex].GetValue<int>(CustomPropertyCode.LIFESTOCK);
            }
            else
            {
                _lifeStock = (int)_lifeStockData;
            }
        }
        else
        {
            //local
            _teamCode = LocalRoomManager.instance.players[_playerIndex].GetValue<int>(CustomPropertyCode.TEAM_CODE);
            _lifeStock = LocalRoomManager.instance.players[_playerIndex].GetValue<int>(CustomPropertyCode.LIFESTOCK);
        }
        Color _c = CustomPropertyCode.TEAMCOLORS[_teamCode];
        panel.color = _c;

        lifeStock_number_text.text = _lifeStock.ToString();
        Debug.Log("set life stock ui  index " + _playerIndex + " " + _c + " " + _lifeStock);
    }

    public void TriggerReviveAnimation()
    {
        revive_animation.SetTrigger("Revive");
        popup_animation.SetTrigger("Popup");
    }

    public void SetUp(Color _color)
    {
        panel.color = _color;
        //lifeStock_number_text.color = _color;
    }
}

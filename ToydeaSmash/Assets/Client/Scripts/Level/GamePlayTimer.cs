using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//public Player time counter:
public class GamePlayTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private int _counter = 0;
    private int _maxTime = 0;
    private static WaitForSeconds _wait = new WaitForSeconds(1);
    private IEnumerator Start()
    {
        _maxTime = LocalRoomManager.instance.gamePlaySetting.GetValue<int>(GameplaySettingControl.MINUTES_OPT) * 60;
        _counter = _maxTime;
        while (_counter > 0)
        {
            _counter--;
            timerText.text = _counter.ToString();
            yield return _wait;
        }

        Debug.Log("Time up");
        //GCManager.Clear();
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Result");
        }
        else if (!PhotonNetwork.IsConnected)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Result");
        }
    }


}

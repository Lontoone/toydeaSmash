using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
public class PauseMenuControl : MonoBehaviour
{
    public GameObject panel;
    public GameObject resumeConuntDownPanel;
    public TextMeshProUGUI countDownText;
    private Coroutine c_resumeCountDown;
    public void Quit()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
    public void Start()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && c_resumeCountDown==null)
        {
            //panel.SetActive(true);
            Pause();
        }
    }

    public void Pause()
    {
        if (c_resumeCountDown != null)
        {
            return;
        }
        Time.timeScale = Mathf.Abs(1 - Time.timeScale);
        if (Time.timeScale == 0)
        {
            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
            c_resumeCountDown = StartCoroutine(ResumeCountDownCoro());
        }
    }

    private IEnumerator ResumeCountDownCoro()
    {
        WaitForSecondsRealtime _wait = new WaitForSecondsRealtime(1);
        Time.timeScale = 0;
        const int _TIME = 5;
        int _counter = 0;
        resumeConuntDownPanel.SetActive(true);
        while (_counter < _TIME)
        {
            countDownText.text = (_TIME - _counter).ToString();
            _counter++;
            resumeConuntDownPanel.transform.DOPunchScale(Vector3.one , 0.5f).SetUpdate(true);
            yield return _wait;
        }
        resumeConuntDownPanel.SetActive(false);
        c_resumeCountDown = null;
        Time.timeScale = 1;
    }
}

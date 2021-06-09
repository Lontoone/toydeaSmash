using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class OptionsSettingMenu : MonoBehaviour
{
    private Resolution[] _resolutions;
    public TMP_Dropdown resolutionDropDown;
    private void Start()
    {
        SetUpResolutionDropDown();
    }
    private void SetUpResolutionDropDown()
    {
        _resolutions = Screen.resolutions;
        resolutionDropDown.ClearOptions();

        List<string> _options = new List<string>();
        int _currentResIndex = 0;
        for (int i = 0; i < _resolutions.Length; i++)
        {
            string __opts = _resolutions[i].width + " x " + _resolutions[i].height;
            _options.Add(__opts);

            if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
            {
                _currentResIndex = i;
            }
        }
        resolutionDropDown.AddOptions(_options);
        resolutionDropDown.value = _currentResIndex;
        resolutionDropDown.RefreshShownValue();
    }

    public AudioMixer audioMixer;
    public void SetMainVolume(float _volume)
    {
        audioMixer.SetFloat("MainVolume", _volume);
    }
    public void SetBGMVolume(float _volume)
    {
        audioMixer.SetFloat("BgmVolume", _volume);
    }
    public void SetSfxVolume(float _volume)
    {
        audioMixer.SetFloat("SfxVolume", _volume);
    }
    public void SetFullScreen(bool _isFullScreen)
    {
        Screen.fullScreen = _isFullScreen;
    }

    public void SetResolution(int _index)
    {
        Screen.SetResolution(_resolutions[_index].width, _resolutions[_index].height, Screen.fullScreen);
    }
}

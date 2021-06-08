using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelectPanelControl : MonoBehaviour
{
    public PlayerSlot playerSlot;
    //public Sprite[] sprites;
    public GameObject teamIconPrefab;
    public GameObject headIconPrefab;
    public GameObject bodyIconPrefab;
    public GameObject teamContainer;
    public GameObject headContainer;
    public GameObject bodyContainer;

    public Sprite[] _headSprites;
    public Sprite[] _bodySprites;

    public Ease easeType;

    private const string HEAD_RESOURCESPATH = "Head icon/";
    private const string BODY_RESOURCESPATH = "Weapon icon/";
    private const int _GAP = 2;

    public void Start()
    {
        //load spite files
        //_headSprites = Resources.LoadAll<Sprite>(HEAD_RESOURCESPATH);
        //_bodySprites = Resources.LoadAll<Sprite>(BODY_RESOURCESPATH);

        //create color item:
        for (int i = 0; i < CustomPropertyCode.TEAMCOLORS.Length; i++)
        {
            GameObject _colorIcon = Instantiate(teamIconPrefab, Vector3.zero, Quaternion.identity, teamContainer.transform);
            _colorIcon.GetComponent<Image>().color = CustomPropertyCode.TEAMCOLORS[i];
            _colorIcon.transform.SetParent(teamContainer.transform);
            _colorIcon.transform.localPosition = new Vector2(i * _GAP, 0);
        }
        //create head item:
        for (int i = 0; i < _headSprites.Length; i++)
        {
            GameObject _icon = Instantiate(headIconPrefab, Vector3.zero, Quaternion.identity, headContainer.transform);
            _icon.GetComponent<Image>().sprite = _headSprites[i];
            _icon.transform.localPosition = new Vector2(i * _GAP, 0);
        }
        //create body item:
        for (int i = 0; i < _bodySprites.Length; i++)
        {
            GameObject _icon = Instantiate(bodyIconPrefab, Vector3.zero, Quaternion.identity, bodyContainer.transform);
            _icon.GetComponent<Image>().sprite = _bodySprites[i];
            _icon.transform.localPosition = new Vector2(i * _GAP, 0);
        }
        playerSlot.OnTeamChanged += ScrollTeamContainer;
        playerSlot.OnHeadChanged += ScrollHeadContainer;
        playerSlot.OnBodyChanged += ScrollBodyContainer;
    }
    public void OnDestroy()
    {
        playerSlot.OnTeamChanged -= ScrollTeamContainer;
        playerSlot.OnHeadChanged -= ScrollHeadContainer;
        playerSlot.OnBodyChanged -= ScrollBodyContainer;
    }

    private void ScrollTeamContainer(int _index)
    {
        Vector2 __goalPos = new Vector2(-_index * _GAP, 0);
        teamContainer.transform.DOLocalMove(__goalPos, 0.5f).SetEase(easeType);
    }
    private void ScrollHeadContainer(int _index)
    {
        Vector2 __goalPos = new Vector2(-_index * _GAP, 0);
        headContainer.transform.DOLocalMove(__goalPos, 0.5f).SetEase(easeType);
    }
    private void ScrollBodyContainer(int _index)
    {
        Vector2 __goalPos = new Vector2(-_index * _GAP, 0);
        bodyContainer.transform.DOLocalMove(__goalPos, 0.5f).SetEase(easeType);
    }

}

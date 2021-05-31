using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HPBarControl : MonoBehaviour
{
    public HitableObj hitable;

    public Image bar;

    public GameObject inner_bar;

    float maxHP;
    float mask_width;

    public const string HPBAR_GC_KEY = "HPBAR_GC_KEY";

    float height_offset = 0;

    private void Awake()
    {
        Debug.Log("register hp bar");
        GCManager.RegisterObject(HPBAR_GC_KEY, gameObject);

        mask_width = GetComponent<RectTransform>().sizeDelta.x;

    }
    public void SetHitable(HitableObj _hitable)
    {
        hitable = _hitable;
        maxHP = hitable.HP;
        hitable.gotHit_event += UpdateHPBar;
        hitable.Die_event += DestorySelfOnDie;

        //height_offset = (float)_hitable.gameObject.GetComponent<SpriteRenderer>()?.bounds.size.y;
        height_offset = 2;
    }

    private void OnEnable()
    {
        if (hitable == null) { return; }

        maxHP = hitable.HP;
        hitable.gotHit_event += UpdateHPBar;
        hitable.Die_event += DestorySelfOnDie;


    }
    private void OnDisable()
    {
        if (hitable == null) { return; }

        hitable.gotHit_event -= UpdateHPBar;
        hitable.Die_event -= DestorySelfOnDie;
    }

    void DestorySelfOnDie()
    {
        GCManager.Destory(HPBAR_GC_KEY, gameObject);
    }

    private void FixedUpdate()
    {
        //follow gameobject

        //TODO: get sprite height offset
        Vector2 _pos = new Vector2(hitable.transform.position.x, hitable.transform.position.y + height_offset);
        transform.position = _pos;

    }

    public void UpdateHPBar()
    {
        bar.fillAmount = hitable.HP / (float)maxHP;

    }
}

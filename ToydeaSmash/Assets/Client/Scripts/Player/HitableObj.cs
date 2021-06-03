using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
//可攻擊物件
public class HitableObj : MonoBehaviour
{
    public float backForce = 0.2f;
    public static event Action<GameObject, float, GameObject> Hit_event;
    public static event Action<GameObject, GameObject> OnKilled; //target , killed by
    //public GameObject hit_numUI_prefab;//跳傷害的UI
    //public static event Action<GameObject> Hit_effect_event;
    public event Action Die_event;
    public event Action<GameObject> KilledBy_event;
    public event Action gotHit_event;
    public event Action gotHeel_event;
    public event Action<GameObject> HitBy_event;
    public float HP = 20;
    [HideInInspector]
    public float maxHP;
    public bool isDead = false;
    public bool isHitable = true;

    public float hit_combo = 0;


    [Range(0, 1)]
    public float damage_taking_rate = 1; // 0%~100%

    HPBarControl hpbar;
    private IEnumerator Start()
    {
        //wait for hp bar to finish regiester:
        yield return new WaitForEndOfFrame();
        //Create HP Bar
        hpbar = GCManager.Instantiate(HPBarControl.HPBAR_GC_KEY).GetComponent<HPBarControl>();
        hpbar.SetHitable(this);

        maxHP = HP;
        Hit_event += Hit;
    }
    private void OnDestroy()
    {
        Hit_event -= Hit;
    }

    public static void Hit_event_c(GameObject t, float d, GameObject attackSource)
    {
        if (Hit_event != null)
        {


            Hit_event(t, d, attackSource); //被攻擊的,傷害,攻擊者
        }
    }

    public void Heal(float amount)
    {
        HP = Mathf.Clamp(HP + amount, 0, maxHP);
        if (gotHeel_event != null)
            gotHeel_event();
    }


    void Hit(GameObject target, float damage, GameObject sources)
    {
        if (target == gameObject)
        {
            Debug.Log(gameObject.name + " 受到 " + damage);

            if (isHitable)
            {
                //Turn to hit sources
                if (Vector2.Dot(target.transform.right, sources.transform.right) > 0)
                {
                    target.transform.eulerAngles = new Vector3(0, target.transform.eulerAngles.y + 180, 0);
                }

                //HP -= damage;
                HP = Mathf.Clamp(HP - damage * damage_taking_rate, 0, maxHP);
                //特效:
                Hit_effect();
                hit_combo++;

                //傷害文字
                //Effecter.PopupTextUI(target, damage.ToString(), 1);

            }

            //判斷死亡
            if (HP <= 0)
            {

                if (!isDead)
                {
                    if (Die_event != null)
                        Die_event();
                    if (KilledBy_event != null)
                        KilledBy_event(sources);

                    if (OnKilled != null)
                    {
                        OnKilled(target, sources);
                    }
                }
                isDead = true;
            }
            else
            {
                if (gotHit_event != null)
                {
                    Debug.Log("<color=green>HURT</color>");
                    gotHit_event();
                }
                if (HitBy_event != null)
                    HitBy_event(sources);
            }

        }
    }

    void Hit_effect()
    {
        //晃鏡頭
        //GameObject.FindObjectOfType<CameraFollow>().CameraShake(0.25f,0.1f,2.5f);
        //CameraFollow.CameraShake_c(0.25f, 0.1f, 2.5f);

        //Effecter.BreakParticleEffect(gameObject.GetComponent<SpriteRenderer>(), Effecter.BLAST_LINE_SMALL, 2);
        //Effecter.BreakParticleEffect(gameObject.GetComponent<SpriteRenderer>(), Effecter.BLAST_Particle_SMALL, 5);

    }
}

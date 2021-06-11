using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Versus Game mode
public class VersusGamePlay : MonoBehaviour
{
    public GameObject lifeStockUIContainer;
    public PlayerLifeStockControl lifeStockItem_prefab;

    public Dictionary<int, int> playerLifeStock = new Dictionary<int, int>();
    Dictionary<int, PlayerLifeStockControl> lifeStockUI = new Dictionary<int, PlayerLifeStockControl>();

    public void Awake()
    {
        PlayerControl.OnCreate += RegisterPlayer;
        PlayerControl.OnDestory += CheckPlayerRevive;

        HitableObj.OnKilled += CheckTeamWinLose;
    }
    public void OnDestroy()
    {
        PlayerControl.OnCreate -= RegisterPlayer;
        PlayerControl.OnDestory -= CheckPlayerRevive;

        HitableObj.OnKilled -= CheckTeamWinLose;
    }

    public const string ISLOSE = "ISLOSE";

    //players first join the game
    public void RegisterPlayer(int _index)
    {
        if (!playerLifeStock.ContainsKey(_index))
        {
            playerLifeStock.Add(_index, (int)LocalRoomManager.instance.gamePlaySetting.playerProperty[GameplaySettingControl.LIFESTOCK_OPT]);
            LocalRoomManager.instance.players[_index].SetProperty(ISLOSE, false);
            //PlayerControl.OnCreate -= RegisterPlayer;
            //PlayerControl.OnCreate += MinusLifeStock;

            Debug.Log("player " + _index + " has lifes " + playerLifeStock[_index]);

            //generate life stock ui item for each player
            PlayerLifeStockControl _ui = Instantiate(lifeStockItem_prefab, Vector3.zero, Quaternion.identity, lifeStockUIContainer.transform);
            lifeStockUI.Add(_index, _ui);
            _ui.SetUp(CustomPropertyCode.TEAMCOLORS[LocalRoomManager.instance.players[_index].GetValue<int>(CustomPropertyCode.TEAM_CODE)]);
            _ui.lifeStock_number_text.text = playerLifeStock[_index].ToString();
        }
    }
    public void MinusLifeStock(int _index)
    {
        //update ui and data
        playerLifeStock[_index]--;
        lifeStockUI[_index].lifeStock_number_text.text = playerLifeStock[_index].ToString();
    }

    public void CheckPlayerRevive(int _i)
    {
        MinusLifeStock(_i);

        if (playerLifeStock[_i] <= 0)
        {
            //player i lose!
            Debug.Log("player " + _i + " lose !");
            LocalRoomManager.instance.players[_i].SetProperty(ISLOSE, true);
        }
        else
        {
            //create player:
            LocalRoomManager.instance.Revive(_i);
        }
    }

    public void CheckTeamWinLose(GameObject target, GameObject killer)
    {
        PlayerControl kpc = killer.GetComponent<PlayerControl>();
        if (kpc == null)
            return;

        int killer_teamCode = (int)LocalRoomManager.instance.players[kpc.dataIndex].playerProperty[CustomPropertyCode.TEAM_CODE];

        int[] other_teams = LocalRoomManager.instance.Get_Index_In_Different_Team(killer_teamCode);

        bool is_team_win = true;
        for (int i = 0; i < other_teams.Length; i++)
        {
            if (playerLifeStock[other_teams[i]] > 0)
            {
                is_team_win = false;
            }
        }

        if (is_team_win)
        {
            //WIN
            Debug.Log("Winner is team " + killer_teamCode);
            //TODO: win hint change to result scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("Result");
        }


    }
}

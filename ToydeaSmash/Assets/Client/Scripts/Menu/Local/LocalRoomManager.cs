using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class LocalRoomManager : MonoBehaviour
{
    public static LocalRoomManager instance;
    public List<LocalPlayerProperty> players = new List<LocalPlayerProperty>();

    public event Action<LocalPlayerProperty> OnLocalPlayerAdded;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
        DontDestroyOnLoad(this);
    }

    public IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        AddLocalPlayer();  //Add first

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    public void AddLocalPlayer()
    {
        LocalPlayerProperty _local = new LocalPlayerProperty();
        players.Add(_local);

        //add slot 
        if (OnLocalPlayerAdded != null)
        {
            OnLocalPlayerAdded.Invoke(_local);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadScene)
    {
        //if in game play
        if (scene.name=="GamePlay") {
            //generate player
            Debug.Log(" Create player!");
            //for each playerData=>

            //  Load Player from resources

            //

        }
    }


}

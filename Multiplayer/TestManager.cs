using Assets.Multiplayer;
using System;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    [SerializeField]
    private UnityNetworkAdapter NetworkAdapter;
    private NetworkManager NetworkManager { get; set; }

    [SerializeField]
    private int _behaviours;

    void Start()
    {
        NetworkManager = new NetworkManager();

        NetworkAdapter = FindObjectOfType<UnityNetworkAdapter>();

        //NetworkManager = new NetworkManager();
    }

    [ContextMenu("Update network's")]
    public void UpdateNetwork()
    {
        var bh = FindObjectsOfType<NetworkBehavior>(true);

        _behaviours = bh.Length;//
    }
}

#if UNITY_EDITOR

[UnityEditor.InitializeOnLoad]
static class EditorSceneManagerSceneSavedExample
{
    static EditorSceneManagerSceneSavedExample()
    {
        UnityEditor.SceneManagement.EditorSceneManager.sceneSaved += OnSceneSaved;
    }
 
    static void OnSceneSaved(UnityEngine.SceneManagement.Scene scene)
    {
        //var mg = GameObject.FindObjectOfType<TestManager>();
        //mg.UpdateNetwork();
    }
}

#endif

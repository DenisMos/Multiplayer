using Assets.Multiplayer;
using Assets.Multiplayer.Scripts.Framework;
using UnityEngine;

namespace Assets.Module.Multiplayer.Spawn
{
    /// <summary>Система сокетов.</summary>
    internal class SpawnSocket : MonoBehaviour
    {
        [SerializeField]
        private NetworkBehavior[] _players;

        [SerializeField]
        private UnityNetworkAdapter _unityNetworkAdapter;

        private void Start()
        {
            Init();
            //SetPlayer(1);
            var scheduler = _unityNetworkAdapter.NetworkScheduler;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.O))
            {
                Init();
                FuncHandleApi.Post("GetUserSocket", _players[0].Id.ToString());
                SetPlayer(0);
            }

            if(Input.GetKeyDown(KeyCode.P))
            {
                Init();
                FuncHandleApi.Post("GetUserSocket", _players[1].Id.ToString());
                SetPlayer(1);
            }
        }

        private void Init()
        {
            var cameries = FindObjectsOfType<Camera>();
            var moves    = FindObjectsOfType<Movement>();

            foreach(var item in cameries)
            {
                item.enabled = false;
            }

            foreach(var item in moves)
            {
                item.enabled = false;
            }
        }

        public void SetPlayer(int id)
        {
            _players[id].gameObject.SetActive(true);
            _players[id].GetComponentInChildren<Camera>().enabled = true;
            _players[id].GetComponentInChildren<Movement>().enabled = true;
        }
    }
}

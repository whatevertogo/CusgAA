using UnityEngine;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;

        public static AudioManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindFirstObjectByType<AudioManager>();
                if (_instance != null) return _instance;
                GameObject go = new GameObject(nameof(AudioManager));
                _instance = go.AddComponent<AudioManager>();
                DontDestroyOnLoad(go);

                return _instance;
            }
        }
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
            
        }
        
    }
    
}


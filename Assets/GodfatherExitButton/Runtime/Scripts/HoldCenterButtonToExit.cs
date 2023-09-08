using UnityEngine;

namespace GodfatherExitButton
{
    public class HoldCenterButtonToExit : MonoBehaviour
    {
        private static HoldCenterButtonToExit _instance = null;

        [Header("Hold Duration")]
        [SerializeField] private float _holdDuration = 3f;
        private float _holdTimer = 0f;

        [Header("Key Code")]
        [SerializeField] private KeyCode _keyToExit = KeyCode.Return;

        private void Awake()
        {
            if (_instance != null) {
                Destroy(gameObject);
            } else {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Update()
        {
            if (Input.GetKey(_keyToExit)) {
                _holdTimer += Time.unscaledDeltaTime;
                if (_holdTimer >= _holdDuration) {
                    Exit();
                }
            } else {
                _holdTimer = 0f;
            }
        }

        public void Exit()
        {
#if UNITY_EDITOR
            Debug.Log("Force Exit !");
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}
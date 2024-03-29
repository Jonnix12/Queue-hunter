using System;
using System.Collections;
using Queue.ConfigFiles.ToolTipConfig;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Queue.Tools.LoadingScreen
{
    public class LoadingScreenHandler : MonoBehaviour
    { 
        [SerializeField,TabGroup("LoadingScreen")] private float _fadeInDuration = 1;
        [SerializeField,TabGroup("LoadingScreen")] private GameObject _viewPort;
        [SerializeField,TabGroup("LoadingScreen")] private GameObject _uiScreen;
        [SerializeField,TabGroup("LoadingScreen")] private Image _backGround;

        [SerializeField,TabGroup("ToolTip")] private TMP_Text _toolTipText;
        [SerializeField,TabGroup("ToolTip")] private ToolTipConfig _toolTip;
        [SerializeField,MinMaxSlider(2,7),TabGroup("ToolTip")] private Vector2 _toolTipDelay;
        
        private float _toolTipDelayTimer;

        private bool _useToolTip = true;
        
        public bool IsFadeIn { get; private set; } = true;

        private void Start()
        {
            if (_toolTip is null || _toolTipText is null)
                _useToolTip = false;
        }

        private void Update()
        {
            if (!IsFadeIn)
                return;
            
            _toolTipDelayTimer  -= Time.deltaTime;

            if (!_useToolTip)
                return;
            
            if (0 > _toolTipDelayTimer)
            {
                _toolTipDelayTimer = UnityEngine.Random.Range(_toolTipDelay.x, _toolTipDelay.y);
                _toolTipText.text = _toolTip.GetToolTip();
            }
        }

        public IEnumerator FadeIn()
        {
            _viewPort.SetActive(true);
            float fade = 0;

            while (fade < 1)
            {
                fade += Time.deltaTime / _fadeInDuration;
                Color color = new Color(255, 255, 255, fade);
                yield return null;
                _backGround.color = color;
            }

            if (_useToolTip)
            {
                _toolTipDelayTimer = UnityEngine.Random.Range(_toolTipDelay.x, _toolTipDelay.y);
                _toolTipText.text = _toolTip.GetToolTip();   
            }
            
            _uiScreen.SetActive(true);
            IsFadeIn = true;
        }

        public IEnumerator FadeOut()
        {
            _uiScreen.SetActive(false);
            
            float fade = 1;

            while (fade > 0)
            {
                fade -= Time.deltaTime / _fadeInDuration;
                Color color = new Color(255, 255, 255, fade);
                yield return null;
                _backGround.color = color;
            }

            _viewPort.SetActive(false);
            IsFadeIn = false;
        }
    }
}

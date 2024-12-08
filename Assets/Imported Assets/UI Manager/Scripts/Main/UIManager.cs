using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG.UI.Main
{
    public enum UIState { Start, Process, Win, Fail }

    public class UIManager : MonoBehaviour
    {
        #region Singleton
        private static UIManager _default;
        public static UIManager Default => _default;
        #endregion

        [SerializeField] private Panel _startPanel;
        [SerializeField] private Panel _processPanel;
        [SerializeField] private Panel _winPanel;
        [SerializeField] private Panel _failPanel;

        private Dictionary<UIState, Panel> _stateToPanel;
        private UIState _curentState;
        public Action<UIState, UIState> OnStateChanged;
        public UIState CurentState
        {
            get => _curentState;
            set
            {
                if (_curentState != value)
                {
                    _stateToPanel[value].ShowPanel();
                    _stateToPanel[_curentState].HidePanel();
                    OnStateChanged?.Invoke(_curentState, value);
                    _curentState = value;
                }
            }
        }
        public Panel this[UIState state]
        {
            get => _stateToPanel[state];
        }
        private void Awake()
        {
            _default = this;

            _stateToPanel = new Dictionary<UIState, Panel>();
            _stateToPanel.Add(UIState.Start, _startPanel);
            _stateToPanel.Add(UIState.Process, _processPanel);
            _stateToPanel.Add(UIState.Win, _winPanel);
            _stateToPanel.Add(UIState.Fail, _failPanel);
        }
    }
}
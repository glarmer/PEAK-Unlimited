using System.Collections.Generic;
using BepInEx.Configuration;
using PEAKUnlimited.Patches;
using UnityEngine;

namespace PEAKUnlimited;

public class ModConfigurationUI : MonoBehaviour
{
        private List<Option> _options;
        private bool _visible;
        private int _selectedIndex;

        private bool _prevCursorVisible;
        private CursorLockMode _prevCursorLock;

        private Texture2D _whiteTex;
        private GUIStyle _titleStyle;
        private GUIStyle _rowStyle;
        private GUIStyle _hintStyle;

        private const int PanelWidth = 360;
        private const int RowHeight = 28;
        private const int Pad = 12;
        private const int TitleHeight = 28;

        public void Init(List<Option> options)
        {
            _options = options ?? new List<Option>();
            _selectedIndex = 0;
            EnsureStyles();
        }

        private void EnsureStyles()
        {
            if (_whiteTex == null)
            {
                _whiteTex = new Texture2D(1, 1);
                _whiteTex.SetPixel(0, 0, Color.white);
                _whiteTex.Apply();
            }

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold
            };

            _rowStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 14,
                padding = new RectOffset(10, 10, 4, 4)
            };

            _hintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true
            };
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                _visible = !_visible;
                if (_visible) OnOpened();
                else OnClosed();
            }

            if (!_visible || _options.Count == 0) return;

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                bool reverse = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                CycleSelection(reverse ? -1 : 1);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow)) CycleSelection(-1);
            if (Input.GetKeyDown(KeyCode.DownArrow)) CycleSelection(1);

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                ToggleSelected();
            
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                AdjustInt(-1);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                AdjustInt(1);
        }

        private void AdjustInt(int delta)
        {
            var opt = _options[_selectedIndex];
            if (opt.Type == Option.OptionType.Int)
            {
                int newVal = Mathf.Clamp(opt.IntEntry.Value + (delta * opt.Step), opt.MinInt, opt.MaxInt);
                opt.IntEntry.Value = newVal;
            }
        }

        private void ToggleSelected()
        {
            var opt = _options[_selectedIndex];
            if (opt.Type == Option.OptionType.Bool)
            {
                opt.BoolEntry.Value = !opt.BoolEntry.Value;
            }
            else if (opt.Type == Option.OptionType.Int)
            {
                int next = opt.IntEntry.Value + opt.Step;
                if (next > opt.MaxInt) next = opt.MinInt;
                opt.IntEntry.Value = next;
            }
        }

        private void OnOpened()
        {
            _prevCursorVisible = Cursor.visible;
            _prevCursorLock = Cursor.lockState;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnClosed()
        {
            Cursor.visible = _prevCursorVisible;
            Cursor.lockState = _prevCursorLock;
        }

        private void CycleSelection(int delta)
        {
            _selectedIndex = (_selectedIndex + delta) % _options.Count;
            if (_selectedIndex < 0) _selectedIndex += _options.Count;
        }

        private void OnGUI()
        {
            if (!_visible) return;
            EnsureStyles();

            int panelHeight = Pad + TitleHeight + 8 + (_options.Count * (RowHeight + 4)) + Pad + 34;
            Rect panelRect = new Rect(20, 20, PanelWidth, panelHeight);

            GUI.color = new Color(0f, 0f, 0f, 0.75f);
            GUI.DrawTexture(panelRect, _whiteTex);
            GUI.color = Color.white;

            var titleRect = new Rect(panelRect.x + Pad, panelRect.y + Pad, panelRect.width - Pad * 2, TitleHeight);
            GUI.Label(titleRect, "PEAK Unlimited Mod Options", _titleStyle);

            float y = titleRect.yMax + 8;
            for (int i = 0; i < _options.Count; i++)
            {
                var rowRect = new Rect(panelRect.x + Pad, y, panelRect.width - Pad * 2, RowHeight);
                bool hover = rowRect.Contains(Event.current.mousePosition);
                if (hover) _selectedIndex = i;

                if (i == _selectedIndex)
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.12f);
                    GUI.DrawTexture(rowRect, _whiteTex);
                    GUI.color = Color.white;
                }

                var opt = _options[i];
                string valueStr = opt.Type == Option.OptionType.Bool
                    ? (opt.BoolEntry.Value ? "ON" : "OFF")
                    : opt.IntEntry.Value.ToString();

                if (GUI.Button(rowRect, $"{opt.Label}: {valueStr}", _rowStyle))
                {
                    ToggleSelected();
                }

                y += RowHeight + 4;
            }

            var hintRect = new Rect(panelRect.x + Pad, panelRect.yMax - Pad - 30, panelRect.width - Pad * 2, 30);
            GUI.Label(
                hintRect,
                "F2: Open/Close • Tab/Shift+Tab ↑/↓: Move • Enter/Click: Change • ←/→: Adjust Int",
                _hintStyle
            );
        }

        private void OnDestroy()
        {
            if (_whiteTex != null)
            {
                Destroy(_whiteTex);
                _whiteTex = null;
            }
        }
    }

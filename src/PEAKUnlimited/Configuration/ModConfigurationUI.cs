using System.Collections.Generic;
using PEAKUnlimited.Patches;
using UnityEngine;

namespace PEAKUnlimited.Configuration;

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
        
        private string titleText = "PEAK Unlimited Settings";
        private string hintText = "F2: Open/Close • Tab or ↑/↓:  Move • Enter/Click: Change • Scroll Wheel or ←/→ Arrows: Adjust Numerical Values";

        private int PanelWidth = 460;
        private int RowHeight = 32;
        private int Pad = 12;

        private int TitleFontSize = 22;
        private int OptionFontSize = 16;
        private int HintFontSize = 14;

        private void CalculatePanelWidth()
        {
            float maxWidth = _titleStyle.CalcSize(new GUIContent(titleText)).x;
            foreach (var opt in _options)
            {
                string valueStr = opt.Type == Option.OptionType.Bool
                    ? (opt.BoolEntry.Value ? "ON" : "OFF")
                    : opt.IntEntry.Value.ToString();
                float w = _rowStyle.CalcSize(new GUIContent($"{opt.Label}: {valueStr}")).x;
                if (w > maxWidth) maxWidth = w;
            }
            
            PanelWidth = Mathf.Clamp((int)maxWidth + Pad * 2, 460, Screen.width - Pad * 2);
        }

        private void Scale(int scale)
        {
            if (scale < 0 && HintFontSize < 2)
            {
                return;
            } 
            TitleFontSize += scale * 2;
            OptionFontSize += scale * 2;
            HintFontSize += scale * 2;
            
            RowHeight = OptionFontSize + 16;

            CalculatePanelWidth();
        }

        public void Init(List<Option> options)
        {
            _options = options ?? new List<Option>();
            _selectedIndex = 0;
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
                fontSize = TitleFontSize,
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold
            };

            _rowStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = OptionFontSize,
                padding = new RectOffset(10, 10, 4, 4)
            };

            _hintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = HintFontSize,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true
            };
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2) && (PlayerConnectionLogAwakePatch.isHost || GameHandler.GetService<RichPresenceService>().m_currentState == RichPresenceState.Status_MainMenu))
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

            if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus))
            {
                this.Scale(1);
            }

            if (Input.GetKeyDown(KeyCode.Minus))
            {
                this.Scale(-1);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow)) CycleSelection(-1);
            if (Input.GetKeyDown(KeyCode.DownArrow)) CycleSelection(1);

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                ToggleSelected();
            
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                AdjustInt(-1);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                AdjustInt(1);
            
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)       
                AdjustInt(1);
            else if (scroll < 0f) 
                AdjustInt(-1);
        }

        private void ToggleSelected()
        {
            var opt = _options[_selectedIndex];
            if (opt.IsDisabled()) return; 

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

        private void AdjustInt(int delta)
        {
            var opt = _options[_selectedIndex];
            if (opt.IsDisabled()) return;

            if (opt.Type == Option.OptionType.Int)
            {
                int newVal = Mathf.Clamp(opt.IntEntry.Value + (delta * opt.Step), opt.MinInt, opt.MaxInt);
                opt.IntEntry.Value = newVal;
            }
        }

        private void OnOpened()
        {
            _prevCursorVisible = Cursor.visible;
            _prevCursorLock = Cursor.lockState;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            if (_options.Count > 0 && _options[_selectedIndex].IsDisabled())
                CycleSelection(1);
        }

        private void OnClosed()
        {
            Cursor.visible = _prevCursorVisible;
            Cursor.lockState = _prevCursorLock;
        }

        private void CycleSelection(int delta)
        {
            if (_options.Count == 0)
                return;

            int startIndex = _selectedIndex;
            do
            {
                _selectedIndex = (_selectedIndex + delta) % _options.Count;
                if (_selectedIndex < 0)
                    _selectedIndex += _options.Count;
                
                if (!_options[_selectedIndex].IsDisabled())
                    break;

            } while (_selectedIndex != startIndex);
        }

        private void OnGUI()
        {
            if (!_visible) return;
            EnsureStyles();
            CalculatePanelWidth();
            
            float titleHeight = _titleStyle.CalcHeight(new GUIContent(titleText), PanelWidth - Pad * 2);

            int panelHeight = Pad + (int) titleHeight + 8 + (_options.Count * (RowHeight + 4)) + Pad + 34;
            Rect panelRect = new Rect(20, 20, PanelWidth, panelHeight);

            GUI.color = new Color(0f, 0f, 0f, 0.75f);
            GUI.DrawTexture(panelRect, _whiteTex);
            GUI.color = Color.white;

            var titleRect = new Rect(panelRect.x + Pad, panelRect.y + Pad, panelRect.width - Pad * 2, titleHeight);
            GUI.Label(titleRect, titleText, _titleStyle);

            float y = titleRect.yMax + 8;
            for (int i = 0; i < _options.Count; i++)
            {
                var rowRect = new Rect(panelRect.x + Pad, y, panelRect.width - Pad * 2, RowHeight);
                var opt = _options[i];

                bool hover = rowRect.Contains(Event.current.mousePosition);
                if (hover && !opt.IsDisabled())
                    _selectedIndex = i;

                string valueStr = opt.Type == Option.OptionType.Bool
                    ? (opt.BoolEntry.Value ? "ON" : "OFF")
                    : opt.IntEntry.Value.ToString();

                // Highlight if selected
                if (i == _selectedIndex && !opt.IsDisabled())
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.24f);
                    GUI.DrawTexture(rowRect, _whiteTex);
                    GUI.color = Color.white;
                }

                // Grey out if disabled
                GUI.enabled = !opt.IsDisabled();

                if (GUI.Button(rowRect, $"{opt.Label}: {valueStr}", _rowStyle))
                {
                    if (!opt.IsDisabled()) 
                        ToggleSelected();
                }
                
                GUI.enabled = true; // Reset for next option

                y += RowHeight + 4;
            }

            float hintHeight = _hintStyle.CalcHeight(new GUIContent(hintText), PanelWidth - Pad * 2);
            var hintRect = new Rect(panelRect.x + Pad, panelRect.yMax - Pad - hintHeight, panelRect.width - Pad * 2, hintHeight);
            GUI.Label(hintRect, hintText, _hintStyle);
            GUI.Label(
                hintRect,
                hintText,
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

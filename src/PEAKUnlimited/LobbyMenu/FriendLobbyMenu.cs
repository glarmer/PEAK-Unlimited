using System;
using System.Collections.Generic;
using Peak.Network;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PEAKUnlimited.LobbyMenu;

public class FriendLobbyMenu : MonoBehaviour
{
    private Dictionary<CSteamID, SteamFriend> _friends = new Dictionary<CSteamID, SteamFriend>();
    private GameObject _originalUI;
    private GameObject _panel;
    private GameObject _panelDrop;
    private GameObject _text;
    private GameObject _timer;

    public void Awake()
    {
        GetSteamFriends();
        InitialiseMenu();
    }

    public void Start()
    {
        InitialiseMenu();
    }

    public void GetSteamFriends()
    {
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
        for (int i = 0; i < friendCount; i++)
        {
            CSteamID steamID = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            SteamFriend friend = new SteamFriend(steamID);
            _friends.Add(steamID, friend);
        }
    }

    public void InitialiseMenu()
    {
        _panel = null;
        _panelDrop = null;
        _text = null;
        _timer = null;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject currentObject = gameObject.transform.GetChild(i).gameObject;
            switch (currentObject.name)
            {
                case "Panel":
                    _panel = currentObject.gameObject;
                    break;
                case "PanelDrop":
                    _panelDrop = currentObject.gameObject;
                    break;
                case "Text (TMP)":
                    _text = currentObject.gameObject;
                    break;
                case "Timer":
                    _timer = currentObject.gameObject;
                    break;
                default:
                    break;
            }
        }

        if (_timer)
        {
            Destroy(_timer);
        }

        if (_panel)
        {
            RectTransform rectTransform = _panel.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0f, 0.5f);
            rectTransform.anchorMax = new Vector2(1f, 0.5f);
            rectTransform.offsetMin = new Vector2(-162f, -5000f);
            rectTransform.offsetMax = new Vector2(0f, 5000f);
            rectTransform.sizeDelta = new Vector2(162f, 10000f);
        }

        if (_panelDrop)
        {
            RectTransform rectTransform = _panelDrop.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0f, 0.5f);
            rectTransform.anchorMax = new Vector2(1f, 0.5f);
            rectTransform.offsetMin = new Vector2(-170f, -5000f);
            rectTransform.offsetMax = new Vector2(0f, 5000f);
            rectTransform.sizeDelta = new Vector2(170f, 10000f);
        }
        
        if (_text)
        {
            GameObject originalButton = GameObject.Find("Button_PlayWithFriends");

            int i = 0;
            foreach (KeyValuePair<CSteamID, SteamFriend> idFriend in _friends)
            {
                CSteamID lobbyId = idFriend.Value.GameInfo.m_steamIDLobby; //todo fix
                string steamString = idFriend.Value.GameStatus;
                if (string.IsNullOrWhiteSpace(steamString))
                {
                    continue;
                }
                GameObject playerText = Instantiate(_text, _text.transform.parent);
                TextMeshProUGUI textMeshPro = playerText.GetComponent<TextMeshProUGUI>();
                textMeshPro.SetText(steamString);
                textMeshPro.rectTransform.localPosition.Set(textMeshPro.rectTransform.localPosition.x,
                    textMeshPro.rectTransform.localPosition.y - i * 100, textMeshPro.rectTransform.localPosition.z);
                Plugin.Logger.LogInfo($"Initialising, {lobbyId.m_SteamID} , {lobbyId.IsValid()}");

                if (!lobbyId.IsLobby())
                {
                    i++;
                    continue;
                }

                GameObject playerButton = Instantiate(originalButton, textMeshPro.transform.parent);
                CopyTransforms(playerText.transform, playerButton.transform);
                RectTransform buttonTransform = playerButton.GetComponent<RectTransform>();
                buttonTransform.anchoredPosition = textMeshPro.rectTransform.anchoredPosition +
                                                   new Vector2(0f,
                                                       textMeshPro.rectTransform.anchoredPosition.y - i * 100 + 30);
                buttonTransform.anchorMax = textMeshPro.rectTransform.anchorMax +
                                            new Vector2(0f,
                                                textMeshPro.rectTransform.anchoredPosition.y - i * 100 + 30);
                buttonTransform.anchorMin = textMeshPro.rectTransform.anchorMin +
                                            new Vector2(0f,
                                                textMeshPro.rectTransform.anchoredPosition.y - i * 100 + 30);

                playerButton.transform.localPosition = new Vector3(-170, 0, playerButton.transform.localPosition.z);
                playerButton.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                Button button = playerButton.GetComponent<Button>();

                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener(() => { SteamLobbyAPI.LobbyHandler.JoinLobby(lobbyId); });

                i++;
            }

            Destroy(_text);
        }
    }

    public void CopyTransforms(Transform sourceTransform, Transform targetTransform)
    {
        targetTransform.position = sourceTransform.position;
        targetTransform.rotation = sourceTransform.rotation;
        targetTransform.localScale = sourceTransform.localScale;

        foreach (Transform sourceChild in sourceTransform)
        {
            Transform targetChild = targetTransform.Find(sourceChild.name);
            if (targetChild)
            {
                CopyTransforms(sourceChild, targetChild);
            }
        }
    }
}
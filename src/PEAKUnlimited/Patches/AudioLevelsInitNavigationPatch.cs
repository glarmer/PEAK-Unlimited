using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using PEAKUnlimited.Util.Debugging;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PEAKUnlimited.Patches;

public class AudioLevelsInitNavigationPatch
{
    [HarmonyPatch(typeof(AudioLevels), nameof(AudioLevels.InitNavigation))]
    [HarmonyPostfix]
    static void Postfix(AudioLevels __instance)
    {
        if (!__instance.mainPage)
            return;

        int max = Plugin.ConfigurationHandler.MaxPlayers;
        int existing = __instance.sliders.Count;

        if (existing >= max)
            return;

        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.AudioSliderLogic, $"Adding sliders: {existing} -> {max}");
        
        var template = __instance.sliders.FirstOrDefault()?.gameObject;
        if (template == null)
            return;

        AddSliders(__instance, template, existing, max);
        SetupAudioLevelsScroll();

        __instance._dirty = true;
    }
    
    private static void AddSliders(AudioLevels instance, GameObject template, int existing, int max)
    {
        int toAdd = max - existing;

        for (int i = 0; i < toAdd; i++)
        {
            var go = Object.Instantiate(template, template.transform.parent);
            go.name = $"AudioLevelSlider ({existing + i})";

            var slider = go.GetComponent<AudioLevelSlider>();
            instance.sliders.Add(slider);

            slider.Init(PhotonNetwork.LocalPlayer);
        }
    }
    
    private static void SetupAudioLevelsScroll()
    {
        var audioLevels = GameObject.Find("AudioLevels");
        if (audioLevels == null) return;

        if (audioLevels.transform.parent?.name == "AudioLevelsScroll")
            return;

        var scrollGameObject = CreateScrollRoot(audioLevels);
        var viewport = CreateViewport(scrollGameObject);
        var contentRectTransform = MoveContent(audioLevels, viewport);
        var scroll = ConfigureScroll(scrollGameObject, viewport, contentRectTransform);

        CreateScrollbar(scrollGameObject, scroll);
        ConfigureLayout(audioLevels);

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);
        Canvas.ForceUpdateCanvases();

        ForwardScrollEvents(audioLevels, scroll);

        scrollGameObject.transform.SetSiblingIndex(audioLevels.transform.GetSiblingIndex());
        scroll.verticalNormalizedPosition = 1f;
    }
    
    private static GameObject CreateScrollRoot(GameObject audioLevels)
    {
        var audioScrollBarGameObject = new GameObject("AudioLevelsScroll", typeof(RectTransform), typeof(ScrollRect));
        audioScrollBarGameObject.transform.SetParent(audioLevels.transform.parent, false);

        var scrollRectTransform = audioScrollBarGameObject.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = new Vector2(1, 0);
        scrollRectTransform.anchorMax = new Vector2(1, 1);
        scrollRectTransform.pivot = new Vector2(1, 1);
        scrollRectTransform.anchoredPosition = Vector2.zero;
        scrollRectTransform.sizeDelta = new Vector2(300, 0);

        return audioScrollBarGameObject;
    }

    private static GameObject CreateViewport(GameObject scrollGameObject)
    {
        var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
        viewport.transform.SetParent(scrollGameObject.transform, false);

        var viewportRectTransform = viewport.GetComponent<RectTransform>();
        viewportRectTransform.anchorMin = Vector2.zero;
        viewportRectTransform.anchorMax = Vector2.one;
        viewportRectTransform.offsetMin = Vector2.zero;
        viewportRectTransform.offsetMax = new Vector2(-20, 0);

        viewport.GetComponent<Image>().enabled = false;
        viewport.GetComponent<Mask>().showMaskGraphic = false;

        return viewport;
    }

    private static RectTransform MoveContent(GameObject audioLevels, GameObject viewport)
    {
        audioLevels.transform.SetParent(viewport.transform, false);

        var audioLevelsRectTransform = audioLevels.GetComponent<RectTransform>();
        audioLevelsRectTransform.anchorMin = new Vector2(0, 1);
        audioLevelsRectTransform.anchorMax = new Vector2(1, 1);
        audioLevelsRectTransform.pivot = new Vector2(0.5f, 1);
        audioLevelsRectTransform.anchoredPosition = Vector2.zero;
        audioLevelsRectTransform.offsetMin = new Vector2(0, audioLevelsRectTransform.offsetMin.y);
        audioLevelsRectTransform.offsetMax = new Vector2(0, audioLevelsRectTransform.offsetMax.y);

        return audioLevelsRectTransform;
    }

    private static ScrollRect ConfigureScroll(GameObject scrollGameObject, GameObject viewport, RectTransform rectTransform)
    {
        var scroll = scrollGameObject.GetComponent<ScrollRect>();
        scroll.viewport = viewport.GetComponent<RectTransform>();
        scroll.content = rectTransform;
        scroll.horizontal = false;
        scroll.vertical = true;

        scroll.inertia = true;
        scroll.decelerationRate = 0.08f;
        scroll.scrollSensitivity = 25f;
        scroll.movementType = ScrollRect.MovementType.Elastic;

        return scroll;
    }

    private static void CreateScrollbar(GameObject scrollGameObject, ScrollRect scrollRect)
    {
        var scrollBarGameObject = new GameObject("Scrollbar", typeof(RectTransform), typeof(Image), typeof(Scrollbar));
        scrollBarGameObject.transform.SetParent(scrollGameObject.transform, false);

        var rt = scrollBarGameObject.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.sizeDelta = new Vector2(20, 0);

        scrollBarGameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.3f);

        var handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
        handle.transform.SetParent(scrollBarGameObject.transform, false);

        var handleRT = handle.GetComponent<RectTransform>();
        handleRT.anchorMin = Vector2.zero;
        handleRT.anchorMax = Vector2.one;
        handleRT.offsetMin = new Vector2(4, 4);
        handleRT.offsetMax = new Vector2(-4, -4);

        handle.GetComponent<Image>().color = new Color(1, 1, 1, 0.55f);

        var scrollbar = scrollBarGameObject.GetComponent<Scrollbar>();
        scrollbar.direction = Scrollbar.Direction.BottomToTop;
        scrollbar.handleRect = handleRT;
        scrollbar.targetGraphic = handle.GetComponent<Image>();

        scrollRect.verticalScrollbar = scrollbar;
        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
    }

    private static void ConfigureLayout(GameObject audioLevels)
    {
        var layout = audioLevels.GetComponent<VerticalLayoutGroup>();
        if (layout != null)
        {
            layout.padding.right = 50;
            layout.enabled = true;
        }

        var fitter = audioLevels.GetComponent<ContentSizeFitter>() ??
                     audioLevels.AddComponent<ContentSizeFitter>();

        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private static void ForwardScrollEvents(GameObject audioLevels, ScrollRect scroll)
    {
        foreach (var t in audioLevels.GetComponentsInChildren<Transform>(true))
        {
            var f = t.GetComponent<AudioLevelSliderScrollForwarder>() ??
                    t.gameObject.AddComponent<AudioLevelSliderScrollForwarder>();

            f.SetScroll(scroll);
        }
    }
    
    //try to help scroll when above sliders, idk, doesn't seem to work
    public class AudioLevelSliderScrollForwarder : MonoBehaviour, IScrollHandler
    {
        private ScrollRect _scroll;

        public void SetScroll(ScrollRect scroll) => _scroll = scroll;

        public void OnScroll(PointerEventData e)
        {
            _scroll?.OnScroll(e);
        }
    }
}
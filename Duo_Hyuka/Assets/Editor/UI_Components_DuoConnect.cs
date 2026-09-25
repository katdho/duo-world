using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class DuoConnectUIComponents {
    public static GameObject FindChild(Transform parent, string objectName) {
        Transform[] children =parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children) {
            if (child.name == objectName) {
                return child.gameObject;
            }
        }
        return null;
    }

    public static GameObject GetOrCreateObject(string name, Transform parent) {
        GameObject existing = FindChild(parent, name);
        if (existing != null) {
            existing.transform.SetParent(parent, false);
            return existing;
        }
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        return obj;
    }

    public static GameObject GetOrCreatePanel(string name, Transform parent, Color color) {
        GameObject panel =GetOrCreateObject(name, parent);
        RectTransform rect = panel.GetComponent<RectTransform>();
        if (rect == null) {
            rect = panel.AddComponent<RectTransform>();
        }

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = panel.GetComponent<Image>();
        if (image == null) {
            image = panel.AddComponent<Image>();
        }
        image.color = color;
        return panel;
    }

    public static TMP_Text GetOrCreateText(string name, Transform parent, string text, float fontSize,
                                           Color color, Vector2 position,Vector2 size) {
        GameObject obj = GetOrCreateObject(name, parent);
        RectTransform rect = obj.GetComponent<RectTransform>();
        if (rect == null) {
            rect = obj.AddComponent<RectTransform>();
        }
        SetRect(rect, position, size);

        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
        if (tmp == null) {
            tmp = obj.AddComponent<TextMeshProUGUI>();
        }
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.textWrappingMode = TextWrappingModes.Normal;
        return tmp;
    }

    public static Button GetOrCreateButton(string name, Transform parent, string label,
                                           Vector2 position, Vector2 size, Color backgroundColor, 
                                           Color textColor, float fontSize = 34f) {
        GameObject obj = GetOrCreateObject(name, parent);
        RectTransform rect = obj.GetComponent<RectTransform>();
        if (rect == null) {
            rect = obj.AddComponent<RectTransform>();
        }
        SetRect(rect, position, size);

        Image image = obj.GetComponent<Image>();
        if (image == null) {
            image = obj.AddComponent<Image>();
        }
        image.color = backgroundColor;
        
        Button button = obj.GetComponent<Button>();
        if (button == null) {button = obj.AddComponent<Button>();
        }

        GameObject textObject = FindChild(obj.transform, "Text");
        if (textObject == null) {
            textObject = new GameObject("Text");
            textObject.transform.SetParent(obj.transform, false);
        }

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        if (textRect == null) {
            textRect = textObject.AddComponent<RectTransform>();
        }
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(20, 10);
        textRect.offsetMax = new Vector2(-20, -10);

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        if (text == null) {
            text = textObject.AddComponent<TextMeshProUGUI>();
        }
        text.text = label;
        text.fontSize = fontSize;
        text.color = textColor;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.Normal;

        DuoConnectUIStyles.ConfigureButton(
            button,
            label,
            backgroundColor,
            textColor,
            fontSize
        );
        return button;
    }

    public static void MoveToParent(GameObject obj, Transform parent) {
        obj.transform.SetParent(parent, false);
    }

    public static void SetRect(RectTransform rect, Vector2 position, Vector2 size) {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    public static void AddClick(Button button, UnityEngine.Events.UnityAction action) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
}

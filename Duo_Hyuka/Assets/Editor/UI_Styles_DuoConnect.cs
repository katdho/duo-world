using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class DuoConnectUIStyles {
    public static readonly Color Green = new Color(0.345f, 0.800f, 0.008f, 1f);
    public static readonly Color DarkGreen = new Color(0.250f, 0.620f, 0.000f, 1f);
    public static readonly Color White = new Color(1f, 1f, 1f, 1f);
    public static readonly Color LightGray = new Color(0.940f, 0.950f, 0.950f, 1f);
    public static readonly Color Gray = new Color(0.450f, 0.470f, 0.470f, 1f);
    public static readonly Color DarkText = new Color(0.120f, 0.160f, 0.160f, 1f);

    public static void ConfigureCanvas(Canvas canvas) {
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null) {
            scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        }

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null) {
            canvas.gameObject.AddComponent<GraphicRaycaster>();
        }
    }

    public static void ConfigureButton(Button button, string label, Color backgroundColor,
                                       Color textColor, float fontSize = 34f) {
        Image image = button.GetComponent<Image>();
        if (image != null) {
            image.color = backgroundColor;
        }

        ColorBlock colors = button.colors;
        colors.normalColor = backgroundColor;
        colors.highlightedColor = SlightlyLighter(backgroundColor);
        colors.pressedColor = SlightlyDarker(backgroundColor);
        colors.selectedColor = backgroundColor;
        colors.disabledColor = new Color(
            backgroundColor.r,
            backgroundColor.g,
            backgroundColor.b,
            0.5f
        );
        button.colors = colors;

        TMP_Text text = button.GetComponentInChildren<TMP_Text>();
        if (text != null) {
            text.text = label;
            text.fontSize = fontSize;
            text.color = textColor;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.textWrappingMode = TextWrappingModes.Normal;
        }
    }

    public static void ConfigureInputField(TMP_InputField input) {
        Image image = input.GetComponent<Image>();
        if (image != null) {
            image.color = White;
        }

        input.contentType = TMP_InputField.ContentType.Alphanumeric;
        input.characterLimit = 6;
        input.lineType = TMP_InputField.LineType.SingleLine;

        TMP_Text text = input.textComponent;
        if (text != null) {
            text.fontSize = 42;
            text.color = DarkText;
            text.alignment = TextAlignmentOptions.Center;
        }

        TMP_Text placeholder = input.placeholder as TMP_Text;
        if (placeholder != null) {
            placeholder.text = "ENTER CODE";
            placeholder.fontSize = 38;
            placeholder.color = Gray;
            placeholder.alignment = TextAlignmentOptions.Center;
        }
    }

    public static void StylePanel(GameObject panel, Color backgroundColor) {
        Image image = panel.GetComponent<Image>();
        if (image != null) {
            image.color = backgroundColor;
        }
    }

    private static Color SlightlyLighter(Color color) {
        return Color.Lerp(color, White, 0.12f);
    }

    private static Color SlightlyDarker(Color color) {
        return Color.Lerp(color, Color.black, 0.12f);
    }
}

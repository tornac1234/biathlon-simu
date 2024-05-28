using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Main;

    public Text TopLeftText;
    public Text TopRightText;
    public Text TimeScaleText;
    public Slider TimeScaleSlider;
    public Toggle TimePassingToggle;

    public void Start()
    {
        Main = this;

        TimeScaleSlider.onValueChanged.AddListener(TimeScaleChanged);
    }

    public static void SetTLText(string text)
    {
        Main.TopLeftText.text = text;
    }

    public static void SetTRText(string text)
    {
        Main.TopRightText.text = text;
    }

    public void TimeScaleChanged(float timeScale)
    {
        TimeScaleText.text = $"Time Scale: 1/{timeScale}";
    }

    public static float GetTimeScale()
    {
        return 1 / Main.TimeScaleSlider.value;
    }

    // TODO: Graphe, https://www.youtube.com/watch?v=CmU5-v-v1Qo
}

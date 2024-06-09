using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Main;

    public Text ShotsLeftText;
    public Text SummaryText;

    public TargetsHolder TargetsHolder;
    public Camera SummaryCamera;

    private void Start()
    {
        Main = this;
    }

    public static void UpdateSummary(TargetsHolder.ShootType shootType)
    {
        List<Target> targets = Main.TargetsHolder.GetTargets(shootType);
        string touched = string.Join(", ", targets.Select(t => t.Touched ? "O": "X"));
        string distances = string.Join(", ", targets.Select(t => t.Touched ? Math.Round((decimal)t.TouchedDistance(), 10).ToString() : "X"));
        string summary = $"Temps passé à chaque cible:\r\n4.34s, 2.84s, 2.97s, 2.1s, 1.9s\r\nCibles touchées (touché = O):\r\n{touched}\r\nDistance au centre de la cible:\r\n{distances}";

        Main.SummaryText.text = summary;
    }

    public static void SetSummaryVisible(bool visible)
    {
        Main.ShotsLeftText.gameObject.SetActive(!visible);
        Main.SummaryText.gameObject.SetActive(visible);
        Main.SummaryCamera.gameObject.SetActive(visible);
        Camera.main.gameObject.SetActive(!visible);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private PlayerController player;

    [Header("Reloading UI")]
    [SerializeField]
    private CanvasRenderer reloadingPanel;
    [SerializeField]
    private CanvasRenderer reloadingProgressBar;

    [Header("Stamina Meter")]
    [SerializeField]
    private CanvasRenderer staminaMeter;

    private void Update()
    {
        if (player.isReloading)
        {
            reloadingPanel.gameObject.SetActive(true);
            reloadingProgressBar.gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(player.GetPercentDoneReloading(), 1f);
        }
        else 
        {
            reloadingPanel.gameObject.SetActive(false);
        }

        staminaMeter.gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1f, player.GetPercentStaminaRemaining());
    }
}

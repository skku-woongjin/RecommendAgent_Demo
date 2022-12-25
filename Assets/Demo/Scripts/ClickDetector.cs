using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using System;

public class ClickDetector : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.GetComponent<TMP_Text>() != null)
        {
            int idx = Convert.ToInt32(eventData.pointerCurrentRaycast.gameObject.GetComponent<TMP_Text>().text);
            GameManager.Gmr.trailGenerator.goTo(idx, true);
            GameManager.Gmr.Invoke("randOwner", .5f);

        }
    }
}
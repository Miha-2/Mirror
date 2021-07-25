using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverPop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float popFactor = 1.1f;
    
    public void OnPointerEnter(PointerEventData eventData) => transform.DOScale(transform.localScale * popFactor, .15f).SetEase(Ease.OutSine);

    public void OnPointerExit(PointerEventData eventData) => transform.DOScale(transform.localScale / popFactor, .15f).SetEase(Ease.OutSine);
}

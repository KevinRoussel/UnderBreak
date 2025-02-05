﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayAnimOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Animation animToPlay;
    [SerializeField] AnimationClip _empty;
    [SerializeField] AnimationClip _mouvement;
    [SerializeField] AudioSource _onOver;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _onOver.Play();
        animToPlay.Play(_mouvement.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _onOver.Stop();
        animToPlay.Play(_empty.name);
    }

    public void ReplayEmpty() => animToPlay.Play(_empty.name);

}

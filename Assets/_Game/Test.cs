using _Game.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public SelectionHeroUI HeroesUI;
    private void Start()
    {
        HeroesUI = FindObjectOfType<SelectionHeroUI>();
    }
}

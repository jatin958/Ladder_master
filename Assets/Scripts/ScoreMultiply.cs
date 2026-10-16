using System;
using DG.Tweening;
using UnityEngine;

public class ScoreMultiply : MonoBehaviour
{
   [SerializeField] private int multiply;
   [SerializeField] private Renderer targetRenderer;
   [SerializeField] private Color selectedColor;
   [SerializeField] private float duration = 0.5f;

   private void Start()
   {
       selectedColor = targetRenderer.material.color;
   }

   public int getMultiplayier()
    {
        return multiply;
    }
    public void ChangeToSelected()
    {
        // Pehle white
        targetRenderer.material.color = Color.white;

        // White -> Selected Color
        targetRenderer.material
            .DOColor(selectedColor, duration)
            .SetEase(Ease.InOutSine);
    }
}

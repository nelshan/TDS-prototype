using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    public void HoverSound()
    {
        AudioManager.Instance.PlaySFX("ui button hover");
    }

    public void ClickSound()
    {
        AudioManager.Instance.PlaySFX("ui buttonclick sfx");
    }
}

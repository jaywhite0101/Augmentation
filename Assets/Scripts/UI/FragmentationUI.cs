using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FragmentationUI : MonoBehaviour
{
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (ItemStorage.Fragmentation != null)
        {
            image.sprite = ItemStorage.Fragmentation.GetComponent<ItemType>().Image;
            image.enabled = true;
        }
        else
        {
            image.enabled = false;
            image.sprite = null;
        }
    }
}

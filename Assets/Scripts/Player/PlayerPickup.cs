// Copyright (c) TigardHighGDC
// SPDX-License SPDX-License-Identifier: Apache-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRange = 4.0f;
    public GameObject UIParent;
    public GameObject ItemUI;

    private GameObject currentItem;
    private float itemDistance;

    private void Update()
    {
        itemDistance = 99999.0f;
        currentItem = null;

        foreach (GameObject item in GameObject.FindGameObjectsWithTag("Pickupable"))
        {
            float distance = Vector3.Distance(transform.position, item.transform.position);

            if (distance < itemDistance)
            {
                itemDistance = distance;
                currentItem = item;
            }
        }

        if (currentItem != null && itemDistance <= pickupRange && Input.GetKeyDown(KeyCode.F))
        {
            Pickup(currentItem.GetComponent<PickupableItem>());
            Destroy(currentItem);
        }
    }

    private void Pickup(PickupableItem pickup)
    {
        if (pickup.Weapon != null)
        {
            gameObject.GetComponent<WeaponInventory>().AddWeapon(pickup.Weapon, pickup.WeaponEffect);
        }
        else if (pickup.Item != null)
        {
            GameObject itemUI = Instantiate(ItemUI, UIParent.transform);
            itemUI.GetComponent<RectTransform>().anchoredPosition += ItemStorage.ItemUIPosition();
            itemUI.GetComponent<Image>().sprite = pickup.Item.GetComponent<ItemType>().Image;
            ItemStorage.ItemList.Add(ItemStorage.ReplaceItem(pickup.Item));
        }
    }
}

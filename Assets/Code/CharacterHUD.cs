using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHUD : MonoBehaviour
{
    public void UpdateView()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        var character = GetComponentInParent<Character>();
        var card = CharacterCard.GetInstance();
        foreach (var effect in character.AppliedAbilities)
        {
            var iconPrefab = card.AbilityIconPrefab;
            var iconInstance = Instantiate(iconPrefab, this.transform);
            iconInstance.GetComponent<SpriteRenderer>().sprite = card.GetIcon(effect);
        }
    }
}

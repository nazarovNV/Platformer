using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemComponent : MonoBehaviour,IobjectDestroyer
{
    [SerializeField] private ItemType type;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    private Item item;

    public Item Item
    {
        get { return item; }
    }

    public void StartDestroy()
    {
        animator.SetTrigger("StartDestroy");
    }

    public void EndDestroy()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        item = GameManager.Instance.itemBase.GetItemOfID((int)type);
        spriteRenderer.sprite = item.Icon;
        GameManager.Instance.itemsContainer.Add(gameObject, this);
    }

    void IobjectDestroyer.Destroy(GameObject gameObject)
    {
        MonoBehaviour.Destroy(gameObject);
    }
}

public enum ItemType
{
    DamagePotion=1, ArmorPotion=2,ForcePotion=3
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEmitor : MonoBehaviour
{
    [SerializeField] private Buff buff;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (GameManager.Instance.buffRecieverContainer.ContainsKey(col.gameObject))
        {
            var reciever = GameManager.Instance.buffRecieverContainer[col.gameObject];
            reciever.AddBuff(buff);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        var reciever = GameManager.Instance.buffRecieverContainer[col.gameObject];
        reciever.RemoveBuff(buff);
    }
}
[System.Serializable]
public class Buff
{
    public BuffType type;
    public float additiveBonus;
    public float multipleBonus;
}

public enum BuffType : byte
{
    Damage, Force, Armor
}

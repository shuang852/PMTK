using System.Collections.Generic;
using UnityEngine;

public class PostcardController : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private int maxCards;
    [SerializeField] private Vector2 positionVariance;
    [SerializeField] private float rotationVariance;

    [SerializeField, HideInInspector] private List<Postcard> cards = new List<Postcard>();
    
    private Transform CardParent => cardParent ? cardParent : cardParent = transform;

    [ContextMenu("Spawn")]
    public void Spawn()
    {
        Rasterise();
        Create();
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        foreach (Postcard card in cards)
        {
            if (card)
                card.Destroy();
        }
        
        cards.Clear();
    }
    
    private void Create()
    {
        if (cards.Count >= maxCards)
            ClearLast();
        
        foreach (Postcard previous in cards)
            previous.transform.Translate(Vector3.forward, Space.Self);
        
        Vector3 position = new Vector3(positionVariance.x * ((Random.value * 2f) - 1f),
            positionVariance.y * ((Random.value * 2f) - 1f), 0f);
        Quaternion rotation = Quaternion.AngleAxis((Random.value * 2f - 1f) * rotationVariance, Vector3.back);
        
        Postcard card = Instantiate(cardPrefab, CardParent).GetComponent<Postcard>();
        card.transform.localPosition = position;
        card.transform.localRotation = rotation;
        
        cards.Add(card);
    }

    private void ClearLast()
    {
        if (cards.Count == 0)
            return;
        
        Destroy(cards[0].gameObject);
        cards.RemoveAt(0);
    }
    
    private void Rasterise()
    {
        if (cards.Count == 0 || cards[0] is null)
            return;
        
        cards[cards.Count - 1].Rasterise();
    }
}

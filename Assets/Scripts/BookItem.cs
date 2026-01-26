using UnityEngine;

public class BookItem : MonoBehaviour
{
    public BookColor color;

    [HideInInspector] public bool isPlaced; // para evitar recontar
}

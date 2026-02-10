using UnityEngine;

public class PuzzleBooksManager : MonoBehaviour
{
    [SerializeField] private int requiredBooks = 3;
    [SerializeField] private DoorController doorToOpen;

    private int _placedCount;
    private bool _opened;

    public void OnBookPlaced()
    {
        if (_opened) return;

        _placedCount++;

        if (_placedCount >= requiredBooks)
        {
            _opened = true;
            if (doorToOpen != null) doorToOpen.Open();
        }
    }
}

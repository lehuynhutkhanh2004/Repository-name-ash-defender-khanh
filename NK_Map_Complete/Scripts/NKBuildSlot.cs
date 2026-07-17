using UnityEngine;

public class NKBuildSlot : MonoBehaviour
{
    public bool isOccupied;
    public Transform placedHero;

    public bool CanPlaceHero()
    {
        return !isOccupied && placedHero == null;
    }

    public void PlaceHero(Transform hero)
    {
        if (hero == null) return;
        placedHero = hero;
        isOccupied = true;
        hero.position = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isOccupied ? Color.red : Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.9f, 0.9f, 0.05f));
    }
}

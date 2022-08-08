using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesUnit : MonoBehaviour
{
    // Set the parameters for the speed of the unit.
    public float Speed;

    private Queue<TilesType> _path;

    public void SetUnitPath(Queue<TilesType> path)
    {
        _path = path;

        // Resets current unit movement.
        StopAllCoroutines();
        StartCoroutine(MoveUnitAlongPath(path));
    }

    // Sets up the movement of the unit.
    private IEnumerator MoveUnitAlongPath(Queue<TilesType> path)
    {
        yield return new WaitForSeconds(1);
        Vector3 lastPosition = transform.position;
        // Only moves if the tiles in the path is greater than 0.
        while (path.Count > 0)
        {
            TilesType nextTile = path.Dequeue();
            float lerpVal = 0;
            transform.LookAt(nextTile.transform, Vector3.up);

            while (lerpVal < 1)
            {
                lerpVal += Time.deltaTime * Speed;
                // Moves unit to the next tile in the queue.
                transform.position = Vector3.Lerp(lastPosition, nextTile.transform.position, lerpVal);
                yield return new WaitForEndOfFrame();
            }
            // Time it should wait before going to the next tile in the queue.
            yield return new WaitForSeconds(0.5f / Speed);
            lastPosition = nextTile.transform.position;
        }
    }
}

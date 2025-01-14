using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointControl : MonoBehaviour
{
    public List<GameObject> Exits;

    public int NextMarker;
    public GameObject Next;
    public bool Red;
    public GameObject Car;
    public bool ActivatePanic;

    public void Lane()
    {
        if (Exits.Count == 0) return;

        NextMarker = Random.Range(0, Exits.Count);

        Next = Exits[NextMarker];
        if (Red == true)
        {

        }

        else if (ActivatePanic == true)
        {
            Car.GetComponent<TrafficBrain>().panic = true;
            ActivatePanic = false;
        }

        else
        {
            Car.GetComponent<TrafficBrain>().goal = Next.transform;
        }

        

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawSphere(transform.position, .5f);

        foreach (GameObject exit in Exits)
        {
            if (exit != null)
            {
                // Draws a blue line from this transform to the target
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, exit.transform.position);
            }
        }
    }
}

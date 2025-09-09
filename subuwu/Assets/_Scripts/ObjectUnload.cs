using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectUnload : MonoBehaviour
{
    public GameObject Canvas_1;
    public GameObject Canvas0;
    public GameObject Canvas1;

    public Dictionary<GameObject, int> Canvas = new Dictionary<GameObject, int>();
    // Track pending deactivate routines per canvas
    private Dictionary<GameObject, Coroutine> pending = new Dictionary<GameObject, Coroutine>();

    void Start()
    {
        Canvas.Add(Canvas_1, -1);
        Canvas.Add(Canvas0, 0);
        Canvas.Add(Canvas1, 1);
    }
    
    void Update()
    {
        if (!CamMovement.moved) return;

        int s  = CamMovement.screennum;
        int ps = CamMovement.prevscreennum;

        foreach (var kvp in Canvas)
        {
            GameObject canv = kvp.Key;
            int id = kvp.Value;

            // If this is the current screen: ensure active and cancel any pending deactivate
            if (id == s)
            {
                // cancel stale deactivate if any (fixes 0→1→0)
                if (pending.TryGetValue(canv, out var co) && co != null)
                {
                    StopCoroutine(co);
                    pending.Remove(canv);
                }

                canv.SetActive(true);
            }

            // If this is the previous screen and we actually changed screens: schedule deactivate once
            if (id == ps && s != ps && !pending.ContainsKey(canv))
            {
                pending[canv] = StartCoroutine(timer(canv, id));
            }
        }

        // Handle this transition once; prevents starting multiple coroutines each frame
        CamMovement.moved = false;
    }

    private IEnumerator timer(GameObject canv, int screenId)
    {
        yield return new WaitForSeconds(0.2f);

        // Final guard: only deactivate if this canvas is STILL not the current one
        if (CamMovement.screennum != screenId)
        {
            canv.SetActive(false);
        }

        // Clean up handle
        pending.Remove(canv);
    }
}

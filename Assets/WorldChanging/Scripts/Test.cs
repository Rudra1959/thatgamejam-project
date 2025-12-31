using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public ShaderPosition sp;
    
    private bool Expanding=false;
    public float Speed = 200f;
    public float Maxradius = 500f;
    float MinRadius = 1f;
    private Coroutine radiusCoroutine;
    // Start is called before the first frame update
    private void Update()
    {
        RadiusControll();
      
    }
    void RadiusControll()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {

            Expanding = !Expanding;

            if(radiusCoroutine!=null)
                StopCoroutine(radiusCoroutine);

            radiusCoroutine = StartCoroutine(UpdateRadius(Expanding ? Maxradius : MinRadius));
        }
       
    }

    private IEnumerator UpdateRadius(float target)
    {
        if (sp == null) yield break;

        while(!Mathf.Approximately(sp.radius,target))
        {
            sp.radius=Mathf.MoveTowards(sp.radius, target, Speed*Time.deltaTime);
            yield return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableParticle : MonoBehaviour
{

    private void Start()
    {
        Invoke("DestroyParticle", 3);
    }
    private void DestroyParticle()
    {   
        Destroy(this.gameObject);
    }
}

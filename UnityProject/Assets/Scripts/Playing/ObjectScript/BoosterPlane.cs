using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterPlane : MonoBehaviour
{

   public float force = 100000f;
   
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Kart") || other.CompareTag("AI"))
      {
            Rigidbody _rb;
            if(other.TryGetComponent<Rigidbody>(out _rb))
            {
                _rb.AddForce(transform.forward * force, ForceMode.Impulse);
            }

        }
   }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
   void InitializeHealth();
   void OnDamageTaken(float damage);
   
   void OnDamageTakenRPC(float damage, Vector3 position);
}

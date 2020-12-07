using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowZone : MonoBehaviour
{
void OnTriggerStay2D(Collider2D other)
{
   RubyController controller = other.GetComponent<RubyController>();
   
   if (controller != null)
   {
      controller.speed = controller.slowspeed; 
   }     
}
void OnTriggerExit2D(Collider2D other)
{
   RubyController controller = other.GetComponent<RubyController>();
   
   if (controller != null)
   {
      controller.speed = controller.normalspeed; 
   } 
}

}

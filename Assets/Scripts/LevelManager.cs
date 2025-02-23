using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject objects;
    [SerializeField] private TargetBehavior[] targets; 
   public bool LevelComplete()
   {
       foreach (var t in targets)
       { 
           if (!t.Completed) return false;
       }
       
       return true;
   }
   
   public void MoveObjects(Vector3 direction)
   {
       objects.transform.position += direction;
   }
}

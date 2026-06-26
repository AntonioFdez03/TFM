using UnityEngine;

class Floor : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {   
        print("Entra al collision Enter");

        if (transform.parent.gameObject.TryGetComponent(out PlaceableBehaviour pb))
        {   
            if(collision.gameObject.TryGetComponent(out PlaceableBehaviour collisionPB))
            {
                print("Item añadido desde floor: " + collisionPB.gameObject.name);
                pb.AddTouchingItem(collisionPB);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (transform.parent.gameObject.TryGetComponent(out PlaceableBehaviour pb))
        {   
            if(collision.gameObject.TryGetComponent(out PlaceableBehaviour collisionPB))
            {
                print("Item retirado desde floor: " + collisionPB.gameObject.name);
                pb.RemoveTouchingItem(collisionPB);
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        print("Colisionando con: " + collision);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collisions : MonoBehaviour
{
    [SerializeField] GameObject Dot;
    [SerializeField] GameObject Dotgray;
  
    private bool freezeScheibe;
    //private bool mouseover = false;
    public Ray ray;
    // private RaycastHit hit;
    public GameObject particle;
     
    // Start is called before the first frame update
    void Start()
    {
        freezeScheibe = false;
        
    }


    void Update(){
        
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (collision.name == "RechteWand")
        {
            //rb.velocity = transform.TransformDirection(rb.velocity.x * -1f, rb.velocity.y, 0f);
            //print("collistion");
        }
        if (collision.name == "DeckenWand")
        {
            //rb.velocity = transform.TransformDirection(rb.velocity.x , rb.velocity.y* -1f, 0f);
        }
        if (collision.name == "LinkeWand")
        {
            //rb.velocity = transform.TransformDirection(rb.velocity.x * -1f, rb.velocity.y, 0f);
        }
        if (collision.name == "BodenWand")
        {
            //rb.velocity = transform.TransformDirection(rb.velocity.x, rb.velocity.y*-1f, 0f);
        }
        //print("Gameobject x collided with " + collision.name);

        //rb.velocity = Quaternion.AngleAxis(90.0f, Vector3.up) * rb.velocity;// velocity * -1f;
        // rb.MoveRotation(90f);
    }

    public bool GetFreeze()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb.velocity.x ==0 && rb.velocity.y == 0){
            freezeScheibe = true;
        }
        return freezeScheibe;
    }

}

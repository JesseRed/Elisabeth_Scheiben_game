using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    [SerializeField] GameObject Dot;
    [SerializeField] GameObject Dotgray;
    private bool mouseover = false;
    public Ray ray;
    public bool mouse_pressed = false;
    // private RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {

    }


    void Update(){
        //Converting Mouse Pos to 2D (vector2) World Pos
        if (Input.GetMouseButtonDown(0) && !mouse_pressed) {
            mouse_pressed = true;
            Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit=Physics2D.Raycast(rayPos, Vector2.zero, 0f);
            //RaycastHit2D hit=Physics2D.GetRayIntersection(rayPos, Vector2.zero, 0f);
            if (hit){
                Rigidbody2D rb = hit.collider.gameObject.GetComponent<Rigidbody2D>();
                //hit.collider.gameObject.freezeScheibe = true;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                
                GameObject dot = Instantiate(Dot);
                dot.transform.position = rayPos;
                    
                Destroy(hit.collider.gameObject,1f);
                Destroy(dot, 1f);
            }
            else
            {
                GameObject dot = Instantiate(Dotgray);
                dot.transform.position = rayPos;
                Destroy(dot, 1f);
            }

        }
        if (Input.GetMouseButtonUp(0)  && mouse_pressed){
            mouse_pressed = false;
        }
    }
}
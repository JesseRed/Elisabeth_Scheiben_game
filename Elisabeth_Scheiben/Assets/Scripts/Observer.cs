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
    private GameSession gameSession;
    private int isHit;
    private ItemSpawner itemSpawner;

    // private RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        itemSpawner = FindObjectOfType<ItemSpawner>();
    }


    void Update()
    {
        isHit =0;
        //Converting Mouse Pos to 2D (vector2) World Pos
        if (Input.GetMouseButtonDown(0) && !mouse_pressed)
        {
            isHit = 1;
            mouse_pressed = true;
            Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);
            //RaycastHit2D hit=Physics2D.GetRayIntersection(rayPos, Vector2.zero, 0f);
            if (hit)
            {
                Rigidbody2D rb = hit.collider.gameObject.GetComponent<Rigidbody2D>();
                //hit.collider.gameObject.freezeScheibe = true;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;

                GameObject dot = Instantiate(Dot);
                dot.transform.position = rayPos;

                Destroy(hit.collider.gameObject, 1f);
                Destroy(dot, 1f);

            }
            else
            {
                isHit = 0;
                GameObject dot = Instantiate(Dotgray);
                dot.transform.position = rayPos;
                Destroy(dot, 1f);
            }
            float vel;
            int numScheibenPresent;
            itemSpawner.get_nearest_Scheiben_data(Camera.main.ScreenToViewportPoint(Input.mousePosition).x, Camera.main.ScreenToViewportPoint(Input.mousePosition).y, out vel, out numScheibenPresent);
            gameSession.playerData.AddData(
                    itemSpawner.get_blockIdx(),
                    (float)(Time.time - itemSpawner.get_timeBlockStart()),
                    "mouse",
                    isHit, // Hit
                    itemSpawner.get_scheibeIdxInBlock(), // Number der Scheibe im aktuellen Block
                    Camera.main.ScreenToViewportPoint(Input.mousePosition).x, // Mouse position
                    Camera.main.ScreenToViewportPoint(Input.mousePosition).y, // Mouse position
                    1f,//Camera.main.ScreenToViewportPoint(rb.transform.position).x, // Scheiben Position
                    1f, //Camera.main.ScreenToViewportPoint(rb.transform.position).y, // Scheiben Position
                    1f, //(float)Mathf.Sqrt(Mathf.Pow(rb.velocity.x,2)+Mathf.Pow(rb.velocity.y,2)),
                    (float)0, // Scheiben Diameter
                    (float)(Time.time - 0f), //timeLokaleScheibeInstatiate), //  int existenceTime, 
                    (float)0f,//durationOfScheibe, // maxExistenceTime
                    numScheibenPresent);
            

        }
        if (Input.GetMouseButtonUp(0) && mouse_pressed)
        {
            mouse_pressed = false;
        }
    }
}
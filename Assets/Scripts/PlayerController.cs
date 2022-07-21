using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    private Rigidbody2D rb;
    const float speed = 10;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
            return;

        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        //Debug.Log("Chk" + rb.velocity);
        //rb.velocity = (new Vector3(hAxis, vAxis, 0) * speed;
        rb.velocity = new Vector3(hAxis, vAxis, 0);
        transform.Translate((rb.velocity * speed) * Time.deltaTime);
    }
}

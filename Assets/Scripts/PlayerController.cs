using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public PhotonView photonView;
    private Rigidbody2D rb;
    const float speed = 1000;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Move();
        /*
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        rb.velocity = new Vector3(hAxis, vAxis, 0);
        transform.Translate((rb.velocity * speed) * Time.deltaTime);
        */
    }

    private void Move()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");
        Vector2 targetVelocity = new Vector2(hAxis, vAxis);
        rb.velocity = ((targetVelocity * speed) * Time.deltaTime);
    }
}

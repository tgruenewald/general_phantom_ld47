using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    // Start is called before the first frame update
    public bool active;
    public bool tempturn;
    public bool turn;
    public bool shoot;
    public bool move;
    public bool aimedShot;
    public int shot;
    public float turnDeg;
    public float speed;
    public GameObject target;
    public GameObject foe;
    public GameObject next;
    private Rigidbody rComp;
    Vector3 targetEularRot;
    CharacterController controller;

    Vector3 downVector;
    Target me;
    void Start()
    {
        downVector = new Vector3();
        controller = target.GetComponent<CharacterController>();
        // rComp = target.GetComponent<Rigidbody>();
        
        me = target.GetComponent<Target>();
        Debug.Log("Getting target " + me.amIDead());
        targetEularRot = new Vector3(0, turnDeg, 0);
        GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        me = target.GetComponent<Target>();
        me.StayGrounded(downVector);
        if (me.amIDead() || me.isAimingMode()) {
            stop();
        }
        if(active)
        {
            
            if(move && !me.amIDead() && !me.isAimingMode())
            {
                target.transform.LookAt(transform);
                // me.look(transform.position);
                MoveTowardsTarget(transform.position);
                // Debug.Log(target.transform.name + " is a walking");
                // target.transform.LookAt(this.transform);
                // rComp.velocity = target.transform.forward * speed;
            }
            
            
            
        }
    }

    void stop() {
        controller.Move(new Vector3(0, 0, 0));
    }

     void MoveTowardsTarget(Vector3 flag) {

      var offset = flag - target.transform.position;
      //Get the difference.
      if(offset.magnitude > .1f) {
      //If we're further away than .1 unit, move towards the target.
      //The minimum allowable tolerance varies with the speed of the object and the framerate. 
      // 2 * tolerance must be >= moveSpeed / framerate or the object will jump right over the stop.
           offset = offset.normalized * speed;
           
           if (me.gameObject.activeSelf) {
                controller.Move(offset * Time.deltaTime);
           }

           //actually move the character.
      }
 }

    IEnumerator Shooting() 
    {
        Debug.Log("Starting shooting");
        stop();
        // me.shoot();
        if (foe.activeSelf) {
            target.transform.LookAt(foe.transform);
            if (me != null) {
                me.shoot();
            }
            Target enemy = foe.transform.GetComponent<Target>();
            if (enemy != null) {
                enemy.TakeDamage(1);
            }
        }
        else {
            Debug.Log("shoot the player");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            target.transform.LookAt(player.transform);
            me.shoot();
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null) {
                playerMovement.killHost();
            }
        }

        Fire(shot);
        yield return new WaitForSeconds(2f);
        me.stopShooting();
    }
    void OnTriggerEnter(Collider other)
    {
        if (active) {
            move = false;
            stop();
            Debug.Log("Something entered");
            if(tempturn)
            {
                Turn(turnDeg);
                Debug.Log("turned");
                if(shoot)
                {
                    Fire(shot);
                }
                Turn(turnDeg);
                Debug.Log("Turned back");
                tempturn = !tempturn;
            }
            if(turn)
            {
                Turn(turnDeg);
            }
            if(shoot)
            {
                Fire(shot);
            }
            if(aimedShot && foe != null)
            {
                Debug.Log("Taking shot at " + foe.transform.name);
                move = false;
                StartCoroutine("Shooting");

            }
            if(next != null)
            {
                Debug.Log("Activate next flag");
                active = false;
                next.GetComponent<Flag>().SetON(true);
                next.GetComponent<Flag>().SetMove(true);
            }
            if (next == null) {
                Debug.Log(target.transform.name + " is stopping");
                stop();
                me.idle();

            }
        }

    }
    void Fire(int bullet)
    {
        for(int i = bullet; i > 0; i--)
        {
            Debug.Log(i);
        }
        shoot = false;
    }
    public void SetON(bool state)
    {
        active = state;
    }
    public void SetMove(bool state)
    {
        move = state;
    }
    public void Turn(float turn)
    {
        me.rotateMe(turn);
    }
}

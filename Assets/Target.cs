using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    Animator anim;
    bool isDead = false;
    bool isAiming = false;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;
    public CharacterController controller;

    GameObject highlightIcon;
    GameObject deathIcon;

    void Start()
    {
        anim = GetComponent<Animator>();
        highlightIcon = transform.Find("highlightIcon").gameObject;
        highlightIcon.SetActive(false);

        deathIcon = transform.Find("deathIcon").gameObject;

    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Return) && GetComponent<DialogueTrigger>() != null && GetComponent<DialogueTrigger>().hasNext()) {
            GetComponent<DialogueTrigger>().next();
        }
    }

    public void SelectUnit() {
        highlightIcon.SetActive(true);
    }

    public void UnselectUnit() {
        highlightIcon.SetActive(false);
    }
    public float health = 1f;
    public void TakeDamage(float amount) {
        health -= amount;
        if (health <= 0f) {
            Die();
        }
    }

    public void StayGrounded(Vector3 downVector) {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        downVector.y += -9.8f * Time.deltaTime;
           //normalize it and account for movement speed.
        if (gameObject.activeSelf)
            controller.Move(downVector);
        if (isGrounded && downVector.y < 0) {
            downVector.y = -2f; 
        }
    }

    public bool amIDead() {
        return isDead;
    }

    public void idle() {
        anim.SetBool("isStopped", true);
    }

    void Die() {
        Debug.Log(transform.name + " has died");
        if (GetComponent<DialogueTrigger>() != null)
            GetComponent<DialogueTrigger>().TriggerDialogue();
        anim.SetBool("isDead", true);
        highlightIcon.SetActive(false);
        deathIcon.GetComponent<Renderer>().enabled = true;

        if (gameObject.transform.tag == "blue_soldier") {
            GameObject.Find("FirstPersonPlayer").GetComponent<PlayerMovement>().blue--;
            if (GameObject.Find("FirstPersonPlayer").GetComponent<PlayerMovement>().blue < 0) {
                GameObject.Find("FirstPersonPlayer").GetComponent<PlayerMovement>().blue = 0;
            }
        }
        else {
            GameObject.Find("FirstPersonPlayer").GetComponent<PlayerMovement>().red--;
            if (GameObject.Find("FirstPersonPlayer").GetComponent<PlayerMovement>().red < 0) {
                GameObject.Find("FirstPersonPlayer").GetComponent<PlayerMovement>().red = 0;
            }
        }

        isDead = true;
        // Destroy(gameObject, 2f);
    }

    // public void shootPlayer() {
    //      RaycastHit hit;

    //     if (Physics.Raycast(transform.position, transform.forward, out hit, 100f)) {
    //         Debug.Log(hit.transform.name);
    //         PlayerMovement player = hit.transform.GetComponent<PlayerMovement>();
    //         if (player != null) {
    //             player.deposses();
    //         }

    //     }
    // }

    public void shoot() {
        Debug.Log("Going into isShooting mode");
        isAiming = true;
        // anim.speed = 0f;
        anim.SetBool("isShooting", true);
    }

    public bool isAimingMode() {
        return isAiming;
    }

    public void stopShooting() {
        Debug.Log("Leaving isShooting mode");
        isAiming = false;
        // anim.speed = 1f;
        anim.SetBool("isShooting", false);
    }

     IEnumerator RotateMe(Vector3 byAngles, float inTime) 
     {    var fromAngle = transform.rotation;
         var toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);
         for(var t = 0f; t < 1; t += Time.deltaTime/inTime) {
             transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
             yield return null;
         }
     }

    // IEnumerator RotateToFlag(Vector3 flag, float inTime) 
    //  {    var fromAngle = transform.rotation;
    //      var toAngle = Quaternion.Euler(transform.eulerAngles + );
    //      for(var t = 0f; t < 1; t += Time.deltaTime/inTime) {
    //          transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
    //          yield return null;
    //      }
    //  }

     public void look(Vector3 flag) {
         transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Quaternion.Euler(0, -90, 0) *flag), 70f * Time.deltaTime); 
     }

    public void rotateMe(float byAngles) {
        StartCoroutine(RotateMe(Vector3.forward * byAngles, 0.8f));

    }
}

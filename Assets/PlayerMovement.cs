using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    // Start is called before the first frame update
    public float speed = 12f;

    public float gravity = -9.81f;

    public float jumpHeight = 3f; 

    public float astralProjectionHeight = 10f;
    public float astralProjectionSpeed = 20f;

    public bool astralProjectMode = false;
    public int astralCount = 0;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    GameObject[] soldiers = null;
    GameObject selectedSoldier = null;
    GameObject possesedSoldier = null;

    public string repeatScene;
    public string nextScene;

    public int red = 0;
    public int blue = 0;



    Vector3 velocity;
    bool isGrounded;
    // Update is called once per frame
    IEnumerator astralProjecting() 
    {
        Debug.Log("Turning off gravity");
        gravity = 3f;
        yield return new WaitForSeconds(.5f);
        Debug.Log("Turning on gravity");
        gravity = -9.81f;

    }

    IEnumerator astralDeposses() 
    {
        Debug.Log("Turning off gravity");
        yield return new WaitForSeconds(.1f);
        gravity = 3f;

        yield return new WaitForSeconds(1f);
        Debug.Log("Turning on gravity");
        gravity = -9.81f;
        if (possesedSoldier != null) {
            // release the current solider
            possesedSoldier.transform.position = new Vector3(transform.position.x, transform.position.y - 2f, transform.position.z);
            possesedSoldier.transform.rotation = transform.rotation;
            possesedSoldier.SetActive(true);
            possesedSoldier = null;

        }     

    }

        	    IEnumerator swapLevels(string nextScene) {

                yield return new WaitForSeconds(5f);
                SceneManager.LoadScene(nextScene, LoadSceneMode.Single);


            }

    	    IEnumerator winclock() {
            while (true) {


                yield return new WaitForSeconds(2f);
                calc_win();
            }
            }



    IEnumerator KillingHost() 
    {
        Debug.Log("Turning off gravity");
        yield return new WaitForSeconds(.1f);
        gravity = 7f;

        yield return new WaitForSeconds(1f);
        Debug.Log("Turning on gravity");
        gravity = -9.81f;
        if (possesedSoldier != null) {
            // release the current solider
            possesedSoldier.transform.position = new Vector3(transform.position.x, transform.position.y - 2f, transform.position.z);
            possesedSoldier.transform.rotation = transform.rotation;
            possesedSoldier.SetActive(true);
        }     
        Target target = possesedSoldier.GetComponent<Target>();
        if (target != null) {
            target.TakeDamage(1f);
        }
        possesedSoldier = null;

    }

    public void killHost() {
        Debug.Log("host is dying:  " + possesedSoldier.transform.name);
        gun_visable(false);
        StartCoroutine("KillingHost");
        velocity.y = Mathf.Sqrt(astralProjectionHeight * -2f * gravity );
        controller.Move(velocity * Time.deltaTime);
    }

    public void deposses() {
        Debug.Log("I am depossoing your body");
        gun_visable(false);

        StartCoroutine("astralDeposses");
        velocity.y = Mathf.Sqrt(astralProjectionHeight * -2f * gravity );
        controller.Move(velocity * Time.deltaTime);
    }

    IEnumerator showhelp() 
    {
        yield return new WaitForSeconds(.5f);
        if (GetComponent<DialogueTrigger>() != null)
            GetComponent<DialogueTrigger>().TriggerDialogue();

    }

    void Start() {
        gun_visable(false);
        StartCoroutine("showhelp");
        StartCoroutine("winclock");

        GameObject[] teamBlue = GameObject.FindGameObjectsWithTag("blue_soldier");
        GameObject[] teamRed = GameObject.FindGameObjectsWithTag("red_soldier");

        foreach(GameObject soldier in teamBlue) {
            blue++;
        }        

        foreach(GameObject soldier in teamRed) {
            red++;
        } 
                
    }

    void gun_visable(bool isVisible) {
        GameObject gun = GameObject.Find("mygun1").gameObject;
        gun.GetComponent<Renderer>().enabled = isVisible;
        gun = GameObject.Find("mygun2").gameObject;
        gun.GetComponent<Renderer>().enabled = isVisible;
    }

    void calc_win() {
        GameObject progressBar = GameObject.Find("progressbar");
        if (progressBar.GetComponent<Image>().fillAmount >= 1f) {
            StartCoroutine(swapLevels(repeatScene));

        }
        Debug.Log("Red:  " + red + ", blue: " + blue);    

        if (red == 0 && blue > 0) {
            // yay
            // SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
            Debug.Log("Yay we win");
            StartCoroutine(swapLevels(nextScene));
        }   
        else {
            if (blue == 0) {
                // SceneManager.LoadScene(repeatScene, LoadSceneMode.Single);
                Debug.Log("Lose, must repeat the day");
                StartCoroutine(swapLevels(repeatScene));
            }
        }


    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return) && GetComponent<DialogueTrigger>().hasNext()) {
            GetComponent<DialogueTrigger>().next();
        }

            
        if (astralProjectMode && selectedSoldier != null) {
            
            // transform.LookAt(selectedSoldier.transform);
            // transform.rotation *= Quaternion.FromToRotation(Vector3.left, Vector3.forward);
            // if (selectedSoldier.transform.position != Vector3.zero) {
            //     Debug.Log("rotating");
            //     transform.rotation = Quaternion.Slerp(
            //         transform.rotation,
            //         Quaternion.LookRotation(selectedSoldier.transform.position),
            //         Time.deltaTime * 30f
            //     );
            // }
            astralCount--;
            var offset = selectedSoldier.transform.position - transform.position;
             float singleStep = 5f * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, offset, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);

            //  transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);

            //Get the difference.
            if(offset.magnitude > 3f) {
                //If we're further away than .1 unit, move towards the target.
                //The minimum allowable tolerance varies with the speed of the object and the framerate. 
                // 2 * tolerance must be >= moveSpeed / framerate or the object will jump right over the stop.
                offset = offset.normalized * astralProjectionSpeed;

                Debug.Log("Moving to target: " + offset.magnitude);
                controller.Move(offset * Time.deltaTime);
                //actually move the character.
                // transform.rotation = selectedSoldier.transform.rotation;
            }
            else {
                astralCount = 0;
                gravity = -9.81f;
                if (astralCount <= 0) {
                        astralCount = 0;
                        Debug.Log("Possesion complete");
                        gun_visable(true);
                        astralProjectMode = false;
                        transform.position = new Vector3(selectedSoldier.transform.position.x, selectedSoldier.transform.position.y + 5f, selectedSoldier.transform.position.z);
                        transform.rotation = selectedSoldier.transform.rotation;
                        selectedSoldier.SetActive(false);
                    
                        possesedSoldier = selectedSoldier;  
                        selectedSoldier = null;   
                }
            }






        }


        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f; 
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);


        if (Input.GetButtonDown("Jump") && isGrounded) {
            Debug.Log("jumping");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity );
        }
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Q)) {
            Debug.Log("Possesing solider");

            if (selectedSoldier != null && !astralProjectMode) {
                gun_visable(false);
                Target target = selectedSoldier.GetComponent<Target>();
                if (target != null && !target.amIDead()) {
                        if (possesedSoldier != null) {
                            // release the current solider
                            possesedSoldier.transform.position = new Vector3(transform.position.x, transform.position.y + 5f, transform.position.z);
                            possesedSoldier.transform.rotation = transform.rotation;
                            possesedSoldier.SetActive(true);
                        }      

                        // Now jump out of his body
                        velocity.y = Mathf.Sqrt(astralProjectionHeight * -2f * gravity );
                        controller.Move(velocity * Time.deltaTime);
                        astralProjectMode = true;
                        astralCount = 20;
                        target.UnselectUnit();
                        
                        // transform.LookAt(selectedSoldier.transform);
                        // transform.rotation = selectedSoldier.transform.rotation;
                        StartCoroutine("astralProjecting");

                }                

            }
            else {
                if (selectedSoldier == null) {
                    Debug.Log("Depossesing");
                    deposses();
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.E)) {
            Debug.Log("Selecting soldier");
            soldiers = GameObject.FindGameObjectsWithTag("blue_soldier");

            List<GameObject> alive = new List<GameObject>();
            foreach(GameObject soldier in soldiers) {
                Target target = soldier.GetComponent<Target>();
                if (target != null) {
                    if (!target.amIDead()) {
                        alive.Add(soldier);
                    }
                    else {
                        Debug.Log(soldier.transform.name + " is dead");
                    }
                }
            }
            // remove dead soldiers

            bool foundSelection = false;
            if (selectedSoldier != null) {
                Target target = selectedSoldier.GetComponent<Target>();
                if (target != null && !target.amIDead()) {
                       target.UnselectUnit();
                }
            }
            foreach(GameObject soldier in alive) {
                Debug.Log("Found soldier: " + soldier.transform.name);
                if (foundSelection) {
                    selectedSoldier = soldier;
                    break;
                }

                if (soldier == selectedSoldier) {
                    // then get the next one
                    foundSelection = true;
                    selectedSoldier = null;
                }
            }

            if (selectedSoldier == null && alive.Count > 0) {
                // if at the end of the list then select the first one
                selectedSoldier = alive[0];
            }
            if (selectedSoldier != null) {
                Target target = selectedSoldier.GetComponent<Target>();
                if (target != null && !target.amIDead()) {
                        target.SelectUnit();
                }
            }


        }
    }
}
 
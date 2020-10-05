
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float impactForce = 10f;
    public float fireRate = 15f;

    public Camera fpsCam;

    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    private float nextTimeToFire = 0f;

    // Update is called once per frame
    GameObject gun;

    void Start() {
        gun = GameObject.Find("mygun1").gameObject;
    }
    void Update()
    {
        if (canShoot() && Input.GetButton("Fire1") && Time.time >= nextTimeToFire) {
            nextTimeToFire = Time.time + 1f/fireRate;
            shoot();
        }
        
    }

    bool canShoot() {
        return gun.GetComponent<Renderer>().enabled;
    }

    void shoot() {
        RaycastHit hit;
        muzzleFlash.Play();
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range)) {
            Debug.Log(hit.transform.name);
            Target target = hit.transform.GetComponent<Target>();
            if (target != null) {
                target.TakeDamage(damage);
            }

            if (hit.rigidbody != null) {
                Debug.Log("adding force");
                hit.rigidbody.AddForce(-hit.normal * impactForce, ForceMode.Impulse );
            }

            GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal)); 
            Destroy(impact, 0.5f);
        }



    }
}

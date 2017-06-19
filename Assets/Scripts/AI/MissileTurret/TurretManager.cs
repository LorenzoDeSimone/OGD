using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TurretManager : NetworkBehaviour
{
    public float rotationSpeed = 1;
    public float shootRating = 5;

    private List<GameObject> players;
    private static float radius;
    private Quaternion startRotation;
    private float nextShoot;
    private GameObject playerMissile;

    // Use this for initialization
    void Start()
    {
        players = new List<GameObject>();
        radius = GetComponent<CircleCollider2D>().radius;
        startRotation = transform.rotation;
        nextShoot = Time.time;
    }

    private float minorDistance, distance;
    private Vector3 target, direction;
    Quaternion finalRotation;

    void Update()
    {
        if (players.Count != 0)
        {
            minorDistance = radius;
            foreach (GameObject go in players)
            {
                distance = Vector3.Distance(transform.position, go.transform.position);
                if (distance < minorDistance)
                {
                    target = go.transform.position;
                    minorDistance = distance;
                }
            }
            direction = (transform.position - target).normalized;
            finalRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180);
            transform.rotation = Quaternion.Lerp(transform.rotation, finalRotation, Time.deltaTime * rotationSpeed);
            if (players.Count > 0 && Mathf.Abs(transform.rotation.eulerAngles.z - finalRotation.eulerAngles.z) < 10 && Time.time > nextShoot)
                GetComponent<Animator>().Play("Shoot");
        }
    }

    public void Shoot()
    {
        if (isServer)
        {
            playerMissile = (GameObject)Instantiate(Resources.Load("Prefabs/NPCs/PlayerMissile"));
            playerMissile.transform.position = transform.position;
            playerMissile.transform.right = transform.right;
            playerMissile.GetComponent<PlayerMissile>().SetFirstHitWithAGravityField(true);
            playerMissile.GetComponent<PlayerMissile>().SetStartDirection(true);
            playerMissile.gameObject.SetActive(true);
            NetworkServer.Spawn(playerMissile);
        }
        nextShoot = Time.time + shootRating;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<PlayerDataHolder>())
            players.Add(collider.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<PlayerDataHolder>())
            players.Remove(collider.gameObject);
    }
}

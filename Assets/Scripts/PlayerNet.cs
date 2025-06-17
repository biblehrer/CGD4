using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNet : NetworkBehaviour
{
    private ulong playerID;

    public GameObject pillar;

    public NetworkVariable<int> score = new NetworkVariable<int>
    (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        // animator.SetBool("walking", false);

        playerID = OwnerClientId;
        Debug.Log("" + playerID);
        if (IsOwner)
        {
            GameManager.id = playerID;
        }
        
    }


    void Update()
    {


        if (!IsOwner) { return; }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(x, 0, z);
        if (movement.magnitude > 0)
        {
            animator.SetBool("walking", true);
            MovingServerRPC(movement);
        }
        else
        {
            animator.SetBool("walking", false);
        }

        score.Value += 1;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateObjectServerRPC();
        }      
        
    }

    // [ServerRpc (RequireOwnership = false)] 
    [ServerRpc] 
    public void CreateObjectServerRPC()
    {
        GameObject obj = Instantiate(pillar);
        obj.GetComponent<NetworkObject>().Spawn(true);
    }

    [ServerRpc]
    void MovingServerRPC(Vector3 movement)
    {
        transform.position += movement * Time.deltaTime * 3;
        MovingClientRPC(transform.position);
    }

    [ClientRpc]
    void MovingClientRPC(Vector3 position)
    {
        transform.position = position;
    }
}

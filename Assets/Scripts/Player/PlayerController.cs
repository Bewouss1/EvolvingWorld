using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    CustomActions input;

    NavMeshAgent agent;

    [Header("Movement")]
    [SerializeField] ParticleSystem clickEffect;
    [SerializeField] LayerMask clickableLayers;

    float lookRotationSpeed = 8f;
    
    void Awake() 
    {
        agent = GetComponent<NavMeshAgent>();

        input = new CustomActions();
        AssignInputs();
    }

    void AssignInputs()
    {
        input.Main.Move.performed += ctx => ClickToMove();
    }

    void ClickToMove()
    {
        RaycastHit hit;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePos), out hit, 100, clickableLayers))
        {
            agent.destination = hit.point;
            if(clickEffect != null)
            { 
                GameObject effect = Instantiate(clickEffect, hit.point + new Vector3(0, 0.1f, 0), clickEffect.transform.rotation).gameObject; 
                Destroy(effect, clickEffect.main.duration);
            }
        }
    }

    void OnEnable() 
    { input.Enable(); }

    void OnDisable() 
    { input.Disable();}

    void Update() 
    {
        FaceMovementDirection();
    }

    void FaceMovementDirection()
	{
        if (agent.velocity.sqrMagnitude < 0.1f) return;
        Vector3 flatDirection = new Vector3(agent.velocity.x, 0, agent.velocity.z);
        Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
	}
}

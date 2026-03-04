using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private MyPlayerInput m_playerInput;
    [SerializeField]
    private AgentMover m_agentMover;

    [SerializeField]
    private InteractionDetector m_interactionDetector;
    [SerializeField]
    private Animator m_animator;
    [SerializeField]
    private AnimationEvents m_animationEvents;

    private bool m_isInteracting;

    private void OnEnable()
    {
        m_playerInput.OnPickUpPressed += HandlePickUpPressed;
    }

    private void HandlePickUpPressed()
    {
        if (m_isInteracting)
            return;
        if(m_interactionDetector.ClosestInteractable == null)
            return;
        m_animator.SetBool("Interact", true);
        m_isInteracting = true;
    }

    private void OnDisable()
    {
        m_playerInput.OnPickUpPressed -= HandlePickUpPressed;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        m_interactionDetector.Initialize(IsOwner);
        if (IsOwner)
        {
            m_animationEvents.OnInteract += HandleInteractAction;
            m_animationEvents.OnAnimationDone += HandleAnimationDone;
        }
    }

    private void HandleAnimationDone()
    {
        m_isInteracting = false;
    }

    private void HandleInteractAction()
    {
        if(m_interactionDetector.ClosestInteractable is PickableBase)
        {
            RequestPickUpServerRpc(
                m_interactionDetector.ClosestInteractable.NetworkObject.NetworkObjectId);
        }
    }

    [Rpc(SendTo.Server)]
    private void RequestPickUpServerRpc(ulong networkObjectId)
    {
        if(!NetworkManager.SpawnManager.SpawnedObjects
            .TryGetValue(networkObjectId, out NetworkObject target))
        {
            return;
        }
        if(!target.TryGetComponent(out PickableBase pickableItem))
        {
            return;
        }
        if(!pickableItem.CanBePickedUp)
        {
            return;
        }

        pickableItem.PickUp();
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            m_animationEvents.OnInteract -= HandleInteractAction;
            m_animationEvents.OnAnimationDone -= HandleAnimationDone;
        }
        base.OnNetworkDespawn();
    }
    private void Update()
    {
        if(IsOwner == false)
        {
            return;
        }
        Vector2 movementInput = m_playerInput.MovementInput;
        m_agentMover.Move(movementInput);
    }
}

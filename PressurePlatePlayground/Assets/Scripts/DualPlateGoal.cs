using UnityEngine;
using UnityEngine.VFX;

public sealed class DualPlateGoal : MonoBehaviour
{
    [SerializeField] private Transform gate;
    [SerializeField] private GameObject fireworksRoot;
    
    private bool blueActive;
    private bool orangeActive;
    private Vector3 gateClosedPosition;


    private void Awake()
    {
        gateClosedPosition = gate.position;
        fireworksRoot.SetActive(false);
    }


    public void ActivateBlue()
    {
        blueActive = true;
    }
    public void DeactivateBlue()
    {
        blueActive = false;
    }


    public void ActivateOrange()
    {
        orangeActive = true;
    }
    public void DeactivateOrange()
    {
        orangeActive = false;
    }


    private void Update()
    {
        Vector3 target = blueActive && orangeActive
            ? gateClosedPosition + Vector3.up * 4f
            : gateClosedPosition;
        gate.position = Vector3.MoveTowards(gate.position, target, 3f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMover playerMover = other.GetComponent<PlayerMover>();
        playerMover.enabled = false;
        
        gate.gameObject.SetActive(false);
        
        fireworksRoot.SetActive(true);
        foreach (VisualEffect effect in fireworksRoot.GetComponentsInChildren<VisualEffect>())
        {
            effect.Reinit();
            effect.Play();
        }
    }
}

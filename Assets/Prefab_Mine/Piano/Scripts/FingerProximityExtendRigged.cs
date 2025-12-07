using UnityEngine;

public class FingerProximityExtendRigged : MonoBehaviour
{
    public Animator handAnimator;      // Animator de la mano
    public Transform pianoTransform;   // Piano en la escena
    public float maxDistance = 0.5f;   // distancia donde empieza a levantar el dedo
    public float minDistance = 0.05f;  // distancia donde el dedo está completamente levantado
    public float smoothTime = 0.05f;

    private float currentLift = 0f;
    private float velocity = 0f;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, pianoTransform.position);
        float targetLift = Mathf.Clamp01((maxDistance - distance) / (maxDistance - minDistance));
        currentLift = Mathf.SmoothDamp(currentLift, targetLift, ref velocity, smoothTime);

        // Aplica el valor al parámetro del Animator
        handAnimator.SetFloat("IndexLiftAmount", currentLift);

        // Además ajusta el peso de la capa (por ejemplo, capa 1 = índice)
        handAnimator.SetLayerWeight(1, currentLift);
    }
}


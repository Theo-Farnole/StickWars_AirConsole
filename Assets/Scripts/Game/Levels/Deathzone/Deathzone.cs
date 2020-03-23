using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathzone : MonoBehaviour
{
    private const string LAYER_NAME_IGNORECOLLISION = "Ignore Collision";
    private readonly Quaternion PARTICLE_SYSTEM_ROTATION = Quaternion.Euler(-90, 0, 0);

    [SerializeField] private GameObject _prefabOnKillFX;

    void Start()
    {
#if UNITY_EDITOR
        CheckIfColliderIsSettedCorrectly();
#endif
        ForceLayer();
    }

    void OnValidate()
    {
        ForceLayer();
    }

    private void OnTriggerEnter2D(Collider2D hitCollider)
    {
        Entity hitEntity = hitCollider.GetComponent<Entity>();

        if (hitEntity)
        {
            PlayKillFX(hitEntity.transform);
            hitEntity.Kill(null);
        }
    }

    private void PlayKillFX(Transform hitTransform)
    {
        // find FX play position
        Vector3 position = hitTransform.position;
        Vector3 direction = transform.position - hitTransform.position;

        // try to raycast to get contact point to play FX,
        // else, if no contact point found, play FX at hitTransform pivot point
        Vector3 fxPlayPosition = Physics.Raycast(position, direction, out RaycastHit hit)
            ? hit.point : hitTransform.position;

        // instantiate FX
        var particleSystem = Instantiate(_prefabOnKillFX, fxPlayPosition, PARTICLE_SYSTEM_ROTATION);

        // ==== 
        // set FX color to color of  killed CharController
        var hitCharController = hitTransform.GetComponent<CharController>();
        if (hitCharController != null)
        {
            Color characterColor = hitCharController.charId.GetSpriteColor();

            // darken & lighten
            float deltaColor = 0.1f;
            Color minColor = characterColor * (1 - deltaColor); // darken
            Color maxColor = characterColor * (1 + deltaColor); // lighten

            // correct alpha
            minColor.a = 1;
            maxColor.a = 1;

            // update FX components' start color
            var fxComponents = particleSystem.GetComponentsInChildren<ParticleSystem>();
            foreach (var fx in fxComponents)
            {
                var fxMain = fx.main;

                bool isFXRoot = fx.gameObject.GetInstanceID() == particleSystem.gameObject.GetInstanceID();
                fxMain.startColor = isFXRoot ? minColor : maxColor;

                Debug.LogFormat("Set color {1} to FX {0} ", fx.name, isFXRoot ? minColor : maxColor);
            }
        }
    }

    #region Debugging
    void CheckIfColliderIsSettedCorrectly()
    {
        var collider = GetComponent<Collider2D>();

        if (collider == null)
        {
            Debug.LogErrorFormat("Deathzone \"{0}\" need a trigger to work properly.");
        }
        else
        {
            if (collider.isTrigger == false)
            {
                Debug.LogErrorFormat("Deathzone \"{0}\"'s collider need to be set as trigger to work properly.");
            }
        }
    }

    private void ForceLayer()
    {
        if (LayerMask.LayerToName(gameObject.layer) != LAYER_NAME_IGNORECOLLISION)
        {
            Debug.LogFormat("{0}'s have been set to {1} to avoid collisions errors.", name, LAYER_NAME_IGNORECOLLISION);
            gameObject.layer = LayerMask.NameToLayer(LAYER_NAME_IGNORECOLLISION);
        }
    }
    #endregion
}

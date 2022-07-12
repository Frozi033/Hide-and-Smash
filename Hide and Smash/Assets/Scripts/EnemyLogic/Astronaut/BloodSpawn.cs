using UnityEngine;
using Random = UnityEngine.Random;

public class BloodSpawn : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleBlood;

    [SerializeField] private int _layerId = 6;
    
    [SerializeField] private float _force = 0.3f;
    [SerializeField] private float _randomValue = 0.25f;
    
    private Rigidbody _myRigidbody;

    private void Start()
    {
        _myRigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == _layerId && _myRigidbody.velocity.magnitude > _force && Random.value > _randomValue)
        {
            var contact = collision.contacts[0];

            Instantiate(_particleBlood, contact.point, Quaternion.identity);
        }
    }
}

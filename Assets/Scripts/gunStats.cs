using UnityEngine;

[CreateAssetMenu]
public class gunStats : ScriptableObject
{
    public GameObject gunModel;

    [Range(1, 10)] public int shootDamage;
    [Range(0.1f, 3)] public float shootRate;
    [Range(5, 500)] public int shootDist;
    public int ammoCur; 
    [Range(5, 50)] public int ammoMax;

    public ParticleSystem hitEffect;
    //multiple shoot sounds to keep from getting annoying
    public AudioClip[] shootSound;
    [Range(0, 1)] public float shootSoundVol; 
}

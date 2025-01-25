using UnityEngine;

public class CollisionSounds : MonoBehaviour
{
    //Je ne detruit plus les Builds pour l'instant, sorry, 
    // + j'ai mis les sons direct dans le state OnPlayModeState
    //bisous -> Thomas


    //Build build;

    //private void Start()
    //{
    //    build = GetComponent<Build>();
    //    //actualMaterial = GetComponent<Build.StateController>().Data.materialData.material;
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.relativeVelocity.magnitude > 1)
    //    {
    //        GameMusics.Instance.CollideObjectSounds(build.Data.materialData.material);
    //    }
    //}

    //Je ne detruit plus les Buils pour l'instant, sorry, j'ai mis les sons direct dans le state OnPlayModeState
    //private void OnDestroy() 
    //{
    //    if (build.CheckCurrentState<BuildState.OnPlayModeState>())
    //    {
    //        GameMusics.Instance?.DestroyObjectSounds(build.Data.materialData.material);
    //    }
    //}
}
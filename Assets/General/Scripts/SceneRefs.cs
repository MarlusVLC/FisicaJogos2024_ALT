using UnityEngine;

namespace GeneralHelpers
{
    public class SceneRefs : Singleton<SceneRefs>
    {
        [field: SerializeField] public AudioListener AudioListener { get; private set; }
    }
}
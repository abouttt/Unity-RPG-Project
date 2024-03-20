using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(menuName = "Settings/Scene Settings", fileName = "SceneSettings")]
public class SceneSettings : SingletonScriptableObject<SceneSettings>
{
    [field: SerializeField, SerializedDictionary("Scene", "Addressable Labels"), Space(10)]
    public SerializedDictionary<SceneType, AddressableLabel[]> LoadResourceLabels { get; private set; }

    [field: SerializeField, SerializedDictionary("Scene", "Background"), Space(10)]
    public SerializedDictionary<SceneType, Sprite> BackgroundImages { get; private set; }
}

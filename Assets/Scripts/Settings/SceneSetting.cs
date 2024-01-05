using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(menuName = "Settings/Load Setting", fileName = "LoadSetting")]
public class SceneSetting : SingletonScriptableObject<SceneSetting>
{
    [field: SerializeField, Space(10)]
    public GameObject[] DefaultPrefabs { get; private set; }

    [field: SerializeField, SerializedDictionary("Scene", "Addressable Labels"), Space(10)]
    public SerializedDictionary<SceneType, AddressableLabel[]> LoadResourceLabels { get; private set; }

    [field: SerializeField, SerializedDictionary("Scene", "Background"), Space(10)]
    public SerializedDictionary<SceneType, Sprite> Background { get; private set; }
}

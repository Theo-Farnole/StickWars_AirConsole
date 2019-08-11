using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfilePictureManager : Singleton<ProfilePictureManager>
{
    #region Fields
    private static Dictionary<int, Sprite> _profilesPictures = new Dictionary<int, Sprite>();
    private static Dictionary<int, Coroutine> _profilesCoroutine = new Dictionary<int, Coroutine>();
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        AirConsole.instance.onConnect += OnConnect;
    }

    void OnDestroy()
    {
        AirConsole.instance.onConnect -= OnConnect;
    }
    #endregion

    #region AirConsole Callbacks
    void OnConnect(int deviceId)
    {
        _profilesCoroutine[deviceId] = StartCoroutine(LoadImage(deviceId));
    }
    #endregion

    public void SetProfilePicture(int deviceId, Image destination)
    {
        // if image is loaded
        if (_profilesPictures.ContainsKey(deviceId))
        {
            destination.sprite = _profilesPictures[deviceId];
        }
        // if image is LOADING
        else
        {
            if (_profilesCoroutine.ContainsKey(deviceId))
            {
                StopCoroutine(_profilesCoroutine[deviceId]);
            }

            _profilesCoroutine[deviceId] = StartCoroutine(LoadImage(deviceId, destination));
        }
    }

    IEnumerator LoadImage(int deviceId, Image destination = null)
    {
        if (_profilesPictures.ContainsKey(deviceId) == true)
            yield break;

        string url = AirConsole.instance.GetProfilePicture(deviceId, 256);

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite sprite = texture.ToSprite();

            _profilesPictures[deviceId] = sprite;

            if (destination)
            {
                destination.sprite = sprite;
            }
        }
    }
    #endregion
}

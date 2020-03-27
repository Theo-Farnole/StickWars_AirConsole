using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SpecialPlayerWrapper : MonoBehaviour
{
    [Header("ABOUT PLAYER")]
    [SerializeField] private Image _profilePicture;
    [SerializeField] private Image _outlineProfilePicture;
    [SerializeField] private TextMeshProUGUI _nickname;
    [Header("SPECIAL TITLE")]
    [SerializeField] private TextMeshProUGUI _specialTitle;
    [SerializeField] private TextMeshProUGUI _specialTitleDescription;

    public TextMeshProUGUI SpecialTitle { get => _specialTitle; }
    public TextMeshProUGUI SpecialTitleDescription { get => _specialTitleDescription; }

    /// <summary>
    /// Update profil picture, outline & nickname in fonction of charid passed in args.
    /// </summary>
    public void UpdateCharIdContent(CharId charId)
    {
        Color playerColor = charId.GetUIColor();

        // update nickname
        _nickname.text = CharIdAllocator.GetNickname(charId);
        //_nickname.color = playerColor;

        // update profile picture
        _outlineProfilePicture.color = playerColor;
        ProfilePictureManager.Instance.SetProfilePicture(charId, _profilePicture);
    }
}

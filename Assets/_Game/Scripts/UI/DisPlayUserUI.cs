using _Game.Scripts.Manager;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

namespace _Game.Scripts.UI
{
    public class DisPlayUserUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _userNameTxt, _userIDTxt;

        [SerializeField]
        private SlotAvatarUserUI _slotAvatarUserUI;

        [SerializeField]
        private Transform _container;

        [SerializeField]
        private Image _avatarUser;

        [SerializeField]
        private GameObject _panelAvatarPopup;

        private HashSet<string> displayedAvatars = new HashSet<string>();
        private bool isAvatarInitialized = false;
        private void Awake()
        {
            Invoke(nameof(LoadUserAvatar), 1f);
        }
        private void LoadUserAvatar()
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
                result =>
                {
                    if (result.Data != null && result.Data.ContainsKey("AvatarPath"))
                    {
                        string avatarPath = result.Data["AvatarPath"].Value;
                        if (!string.IsNullOrEmpty(avatarPath))
                        {
                            string fullPath = "Portrait/" + avatarPath;
                            Sprite loadedAvatar = Resources.Load<Sprite>(fullPath);

                            if (loadedAvatar != null)
                            {
                                _avatarUser.sprite = loadedAvatar;
                                UserManagerUI.Instance.AvatarUser.sprite = loadedAvatar;
                                _avatarUser.enabled = true;
                                UserManagerUI.Instance.AvatarUser.enabled = true;

                                isAvatarInitialized = true;
                                Debug.Log("Avatar loaded successfully from PlayFab: " + fullPath);
                            }
                            else
                            {
                                Debug.LogWarning("Avatar not found at path: " + fullPath);
                            }
                        }
                    }
                    DisPlayUserAvatar();
                },
                error =>
                {
                    Debug.LogError("Failed to load avatar path from PlayFab: " + error.GenerateErrorReport());
                    DisPlayUserAvatar();
                });
        }


        public void DisPlayUserAvatar()
        {
            bool hasHeroes = HeroManager.Instance.HeroesAvailable.Count > 0 && HeroManager.Instance.HeroesAvailable[0].heroes.Count > 0;
            if (!hasHeroes)
            {
                UserManagerUI.Instance.AvatarUser.enabled = false;
                _avatarUser.enabled = false;
                return;
            }
            UserManagerUI.Instance.AvatarUser.enabled = true;
            _avatarUser.enabled = true;

            RefreshAvatars();
            _userNameTxt.text = "Name: " + RankingManager.Instance.UserInformation.UserName;
            _userIDTxt.text = "ID: " + RankingManager.Instance.UserInformation.MasterPlayerID;
        }

        private void RefreshAvatars()
        {
            foreach (var heroList in HeroManager.Instance.HeroesAvailable)
            {
                foreach (var hero in heroList.heroes)
                {
                    string avatarPath = hero.IconAvatarPath;

                    if (!displayedAvatars.Contains(avatarPath) && !string.IsNullOrEmpty(avatarPath))
                    {
                        SlotAvatarUserUI slotAvatar = Instantiate(_slotAvatarUserUI, _container);
                        slotAvatar.SetHeroUI(avatarPath);

                        displayedAvatars.Add(avatarPath);

                        slotAvatar.OnAvatarSelected = UpdateUserAvatar;
                    }
                }
            }
        }

        private void UpdateUserAvatar(Sprite selectedAvatar)
        {
            if (selectedAvatar != null)
            {
                UserManagerUI.Instance.AvatarUser.enabled = true;
                _avatarUser.enabled = true;

                _avatarUser.sprite = selectedAvatar;
                UserManagerUI.Instance.AvatarUser.sprite = selectedAvatar;
                _panelAvatarPopup.SetActive(false);
                SaveUserAvatar(selectedAvatar.name);
            }
        }

        private void SaveUserAvatar(string avatarPath)
        {
            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    { "AvatarPath", avatarPath }
                }
            };

            PlayFabClientAPI.UpdateUserData(request,
                result => Debug.Log("Avatar path saved successfully."),
                error => Debug.LogError("Failed to save avatar path: " + error.GenerateErrorReport()));
        }
    }
}

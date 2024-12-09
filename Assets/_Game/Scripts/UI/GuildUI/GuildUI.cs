using _Game.Scripts.Manager;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.GroupsModels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public class GuildUI : MonoBehaviour
    {
        public GuildInformation GuildInformation;

        public List<MemberGuild> MemberList = new List<MemberGuild>();

        [SerializeField]
        private SlotMemberUI _slotMemberUIPrefab;

        [SerializeField]
        private Transform _memberListContainer;

        [SerializeField]
        private TMP_InputField _nameGuildInputFiel;

        [SerializeField]
        private Button _creatGuildBtn;

        [SerializeField]
        private GameObject _panelCreatGuildPopup;

        [SerializeField]
        private GameObject _panelMainGuildPopup;

        #region Stats Guild

        [SerializeField]
        private TextMeshProUGUI _nameGuildTxt;

        [SerializeField]
        private TextMeshProUGUI _idGuildTxt;

        [SerializeField]
        private TextMeshProUGUI _amountMemberTxt;
        #endregion

        private const int MaxGuildMembers = 10;

        private void Start()
        {
            _creatGuildBtn.onClick.AddListener(OnCreateGuildButtonClicked);
            _nameGuildInputFiel.onValueChanged.AddListener(ValidateGuildName);

            CheckPlayerGuildStatus();
        }

        private void ValidateGuildName(string guildName)
        {
            bool isValid = guildName.Length >= 6 && guildName.Length <= 8;
            _creatGuildBtn.interactable = isValid;
        }

        private void OnCreateGuildButtonClicked()
        {
            string guildName = _nameGuildInputFiel.text;
            string masterPlayerId = RankingManager.Instance.UserInformation.MasterPlayerID;

            if (string.IsNullOrEmpty(masterPlayerId))
            {
                Debug.LogError("MasterPlayerID không hợp lệ.");
                return;
            }

            CreateGuildOnPlayFab(masterPlayerId, guildName);
        }

        private void CreateGuildOnPlayFab(string masterPlayerId, string guildName)
        {
            var createGroupRequest = new CreateGroupRequest
            {
                GroupName = guildName
            };

            PlayFabGroupsAPI.CreateGroup(createGroupRequest,
                result =>
                {
                    Debug.Log($"Guild '{guildName}' được tạo thành công với GroupID: {result.Group.Id}");
                    GuildInformation.GuildID = result.Group.Id;
                    GuildInformation.GuildName = guildName;

                    AddMemberToGuild(result.Group.Id, masterPlayerId, "Leader");

                    SaveGuildData(result.Group.Id, guildName);
                    UpdateGuildUI();
                },
                error =>
                {
                    Debug.LogError($"Lỗi khi tạo Guild: {error.ErrorMessage}");
                });
        }


        private void AddMemberToGuild(string groupId, string playerId, string role)
        {
            if (GuildInformation.AmountMember >= MaxGuildMembers)
            {
                Debug.LogError("Guild đã đạt số lượng thành viên tối đa.");
                return;
            }

            var addMemberRequest = new AddMembersRequest
            {
                Group = new PlayFab.GroupsModels.EntityKey
                {
                    Id = groupId,
                    Type = "group"
                },
                Members = new List<PlayFab.GroupsModels.EntityKey>
        {
            new PlayFab.GroupsModels.EntityKey
            {
                Id = playerId,
                Type = "title_player_account"
            }
        }
            };

            PlayFabGroupsAPI.AddMembers(addMemberRequest,
                result =>
                {
                    Debug.Log($"Người chơi {playerId} đã được thêm vào Guild với vai trò {role}.");
                    GuildInformation.AmountMember++;

                    if (role == "Leader")
                    {
                        GuildInformation.MasterPlayerID = playerId;
                    }

                    UpdateGuildUI();
                },
                error =>
                {
                    Debug.LogError($"Lỗi khi thêm thành viên vào Guild: {error.ErrorMessage}");
                });
        }


        private void CheckPlayerGuildStatus()
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey("GuildID"))
                {
                    string guildId = result.Data["GuildID"].Value;
                    string guildName = result.Data["GuildName"].Value;

                    GuildInformation.GuildID = guildId;
                    GuildInformation.GuildName = guildName;

                    _panelCreatGuildPopup.SetActive(false);
                    _panelMainGuildPopup.SetActive(true);
                }
                else
                {
                    _panelCreatGuildPopup.SetActive(true);
                    _panelMainGuildPopup.SetActive(false);
                }
            },
            error =>
            {
                Debug.LogError($"Lỗi khi kiểm tra trạng thái Guild: {error.ErrorMessage}");
            });
        }
        private void SaveGuildData(string groupId, string guildName)
        {
            var playerData = new Dictionary<string, string>
    {
        { "GuildID", groupId },
        { "GuildName", guildName }
    };

            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                Data = playerData
            },
            result =>
            {
                Debug.Log("Dữ liệu Guild được lưu thành công.");
            },
            error =>
            {
                Debug.LogError($"Lỗi khi lưu dữ liệu Guild: {error.ErrorMessage}");
            });
        }

        private void UpdateGuildUI()
        {
            _nameGuildTxt.text = $"Name Guild: {GuildInformation.GuildName}";
            _idGuildTxt.text = $"ID: {GuildInformation.GuildID}";
            _amountMemberTxt.text = $"Members: {GuildInformation.AmountMember}/{MaxGuildMembers}";

           // DisplayMemberList();
        }

        private void DisplayGuildMaster(string masterPlayerId)
        {
            PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest
            {
                PlayFabId = masterPlayerId
            }, result =>
            {
                string masterPlayerName = result.PlayerProfile.DisplayName;
                _nameGuildTxt.text += $"\nMaster: {masterPlayerName} (ID: {masterPlayerId})";
            }, error =>
            {
                Debug.LogError($"Lỗi khi lấy thông tin người tạo Guild: {error.ErrorMessage}");
            });
        }

       /* private void DisplayMemberList()
        {
            foreach (Transform child in _memberListContainer)
            {
                Destroy(child.gameObject);
            }

            PlayFabGroupsAPI.ListGroupMembers(new ListGroupMembersRequest
            {
                Group = new PlayFab.GroupsModels.EntityKey
                {
                    Id = GuildInformation.GuildID,
                    Type = "group"
                }
            },
            result =>
            {
                foreach (var member in result.Members)
                {
                    string memberId = member.Entity.Id;
                    string memberRole = member.RoleId;

                    PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest
                    {
                         PlayFabId = memberId
                    },
                    profileResult =>
                    {
                        string displayName = profileResult.PlayerProfile?.DisplayName ?? "Unknown";

                        var slotMemberUI = Instantiate(_slotMemberUIPrefab, _memberListContainer);
                        slotMemberUI.SetData(
                            position: memberRole,
                            name: $"{displayName} ({memberId})",
                            avatarSprite: null
                        );
                    },
                    error =>
                    {

                    });
                }
            },
            error =>
            {
                Debug.LogError($"Lỗi khi lấy danh sách thành viên: {error.ErrorMessage}");
            });
        }*/
    }

    [System.Serializable]
    public class GuildInformation
    {
        public string GuildName;
        public string GuildID;
        public string MasterPlayerID;
        public int AmountMember = 0;
    }

    [System.Serializable]
    public class MemberGuild
    {
        public string MemberName;
        public string MemberID;
        public bool Caption;
        public bool ViceCaptain;
        public bool Member;
    }
}

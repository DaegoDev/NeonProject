﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLayoutGroup : MonoBehaviour {

	[SerializeField]
	private GameObject _playerListingPrefab;
	private GameObject PlayerListingPrefab {
		get { return _playerListingPrefab; }
	}

	private List<PlayerListing> _playerListings = new List<PlayerListing>();
	private List<PlayerListing> PlayerListings {
		get { return _playerListings; }
	}

	private void OnMasterClientSwitched(PhotonPlayer newMasterClient) {
		PhotonNetwork.LeaveRoom();
	}

	private void OnJoinedRoom() {
		foreach (Transform child in transform) {
			Destroy(child.gameObject);
		}

		MainCanvasManager.Instance.RoomCanvas.transform.SetAsLastSibling();

		PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;

		foreach (PhotonPlayer photonPlayer in photonPlayers) {
			PlayerJoinRoom(photonPlayer);
		}
	}

	private void OnPhotonPlayerConnected(PhotonPlayer photonPlayer) {
		PlayerJoinRoom(photonPlayer);
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer photonPlayer) {
		PlayerLeftRoom(photonPlayer);
	}

	private void PlayerJoinRoom(PhotonPlayer photonPlayer) {
		if (photonPlayer == null) return;

		PlayerLeftRoom(photonPlayer);

		GameObject playerListingObj = Instantiate(PlayerListingPrefab);
		playerListingObj.transform.SetParent(transform, false);

		PlayerListing playerListing = playerListingObj.GetComponent<PlayerListing>();
		playerListing.ApplyPhotonPlayer(photonPlayer);

		PlayerListings.Add(playerListing);
	}

	private void PlayerLeftRoom(PhotonPlayer photonPlayer) {
		int index = PlayerListings.FindIndex(x => x.PhotonPlayer == photonPlayer);

		if (index != -1) {
			Destroy(PlayerListings[index].gameObject);
			PlayerListings.RemoveAt(index);
		}
	}

	public void OnClickRoomState() {
		if (!PhotonNetwork.isMasterClient) return;

		PhotonNetwork.room.IsOpen = !PhotonNetwork.room.IsOpen;
		PhotonNetwork.room.IsVisible = PhotonNetwork.room.IsOpen;
	}

	public void OnClickLeaveRoom() {
		PhotonNetwork.LeaveRoom();
	}

}

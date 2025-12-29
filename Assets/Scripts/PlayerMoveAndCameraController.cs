using MyGame.Interaction;
using SojaExiles;
using UnityEngine;

public static class PlayerMoveAndCameraController
{
	public static PlayerReference player => PlayerReference.Instance;

	public static void SetPlayerMovement(bool enabled)
	{
		if (player == null) return;

		var move = player.GetComponent<PlayerMovement>();
		if (move != null)
			move.enabled = enabled;

		var interaction = player.GetComponent<PlayerInteraction>();
		if (interaction != null)
			interaction.enabled = enabled;

		if (enabled == false)
		{
			var rb = player.GetComponent<Rigidbody>();
			if (rb != null)
				rb.velocity = Vector3.zero;
		}
	}

	public static void SetPlayerCamera(bool enabled)
	{
		if (player == null) return;

		var cam = player.GetComponentInChildren<Camera>();
		if (cam != null)
			cam.enabled = enabled;

		SetPlayerCameraMove(enabled);
    }

	public static void SetPlayerUI(bool enabled)
	{
		var ui = PlayerReference.Instance.playerUI;
		if (ui != null)
			ui.SetActive(enabled);
	}

	public static void DisableAll()
	{
		SetPlayerMovement(false);
		SetPlayerCamera(false);
		SetPlayerUI(false);
	}

	public static void EnableAll()
	{
		SetPlayerMovement(true);
		SetPlayerCamera(true);
		SetPlayerUI(true);
	}

    public static void SetPlayerCameraMove(bool enabled)
    {
        if (player == null) return;

        var cam = player.GetComponentInChildren<Camera>();

        var mouseLook = cam.GetComponentInChildren<MouseLook>();
        if (mouseLook != null)
            mouseLook.enabled = enabled;

        if (enabled == false)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

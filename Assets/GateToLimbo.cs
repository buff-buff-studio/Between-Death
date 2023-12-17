using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Refactor;
using Refactor.Entities;
using Refactor.Interface;
using UnityEngine;

public class GateToLimbo : MonoBehaviour
{
    public GameObject player;
    public MeshRenderer[] swords;
    public Animator playerAnim;
    public Transform playerPos;
    public Animator doorAnim;
    public IngameGameInput input;

    public void OpenDoor()
    {
        player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<Entity>().enabled = false;
        StartCoroutine(OpenDoorLerp());
    }

    public IEnumerator OpenDoorLerp()
    {
        foreach (var s in swords) s.material.DOFloat(1, "_Dissolve_Amount", .25f);
        player.transform.DOMove(new Vector3(playerPos.position.x, player.transform.position.y, playerPos.position.z), .5f);
        playerAnim.transform.DORotate(playerPos.rotation.eulerAngles, .5f);
        yield return new WaitForSeconds(.5f);
        player.GetComponent<CharacterController>().enabled = true;
        playerAnim.Play("OpenDoor");
        doorAnim.Play("OpenDoor");

        yield return new WaitForSeconds(.25f);
        while (!(doorAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1))
        { yield return null; }
        yield return new WaitForSeconds(.1f);
        LoadingScreen.LoadScene("BossArea");
    }
}

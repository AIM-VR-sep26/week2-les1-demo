using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public sealed class PressurePlatePlayModeTests
{
    [UnityTest]
    public IEnumerator PlayerAndHeavyCubeHoldBothPlates()
    {
        SceneManager.LoadScene("PressurePlatePlayground");
        yield return null;

        PlayerMover player = Object.FindFirstObjectByType<PlayerMover>();
        PressurePlate blue = GameObject.Find("Blue PressurePlate").GetComponent<PressurePlate>();
        PressurePlate orange = GameObject.Find("Orange PressurePlate").GetComponent<PressurePlate>();
        Rigidbody heavy = GameObject.Find("Heavy Push Cube").GetComponent<Rigidbody>();
        Rigidbody lightCube = GameObject.Find("Light Push Cube").GetComponent<Rigidbody>();
        GameObject gate = GameObject.Find("Exit Gate");
        Light[] lights = Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Light blueLight = System.Array.Find(lights, light => light.name == "Blue Lock Light");
        Light orangeLight = System.Array.Find(lights, light => light.name == "Orange Lock Light");
        Transform blueLock = GameObject.Find("Blue Door Lock").transform;
        Transform orangeLock = GameObject.Find("Orange Door Lock").transform;
        Vector3 blueOutwardPosition = blueLock.position;
        Vector3 orangeOutwardPosition = orangeLock.position;
        Vector3 orangeOutwardLocalPosition = orangeLock.localPosition;
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        GameObject fireworks = System.Array.Find(allTransforms, item => item.name == "Goal Fireworks").gameObject;

        Assert.That(heavy.mass, Is.EqualTo(1f));
        Assert.That(heavy.transform.localScale, Is.EqualTo(Vector3.one));
        Assert.That(lightCube.mass, Is.EqualTo(0.5f));
        Assert.That(lightCube.transform.localScale, Is.EqualTo(Vector3.one));
        Assert.That(blueLight.gameObject.activeSelf, Is.False);
        Assert.That(orangeLight.gameObject.activeSelf, Is.False);
        Assert.That(fireworks.activeSelf, Is.False);

        lightCube.position = blue.transform.position + Vector3.up;
        Physics.SyncTransforms();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        Assert.That(blueLight.gameObject.activeSelf, Is.False, "The 0.5-mass cube must not activate a plate requiring mass 1.0.");

        lightCube.position = new Vector3(6f, 1f, -8f);
        heavy.position = blue.transform.position + Vector3.up;
        Rigidbody playerBody = player.GetComponent<Rigidbody>();
        playerBody.position = orange.transform.position + Vector3.up;
        Physics.SyncTransforms();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        float closedGateY = gate.transform.position.y;
        yield return new WaitForSeconds(0.5f);
        float openGateY = gate.transform.position.y;
        Assert.That(openGateY, Is.GreaterThan(closedGateY + 0.5f));
        Assert.That(blueLight.gameObject.activeSelf, Is.True);
        Assert.That(orangeLight.gameObject.activeSelf, Is.True);
        Assert.That(blueLock.position.x, Is.GreaterThan(blueOutwardPosition.x + 0.2f));
        Assert.That(orangeLock.position.x, Is.LessThan(orangeOutwardPosition.x - 0.2f));
        float gateRise = openGateY - closedGateY;
        Assert.That(blueLock.position.y, Is.EqualTo(blueOutwardPosition.y + gateRise).Within(0.1f));
        Assert.That(orangeLock.position.y, Is.EqualTo(orangeOutwardPosition.y + gateRise).Within(0.1f));

        playerBody.position = new Vector3(0f, 1f, -8f);
        Physics.SyncTransforms();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.5f);
        Assert.That(blueLight.gameObject.activeSelf, Is.True);
        Assert.That(orangeLight.gameObject.activeSelf, Is.False);
        Assert.That(orangeLock.localPosition.x, Is.EqualTo(orangeOutwardLocalPosition.x).Within(0.1f));

        playerBody.position = orange.transform.position + Vector3.up;
        Physics.SyncTransforms();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.5f);
        playerBody.linearVelocity = new Vector3(0f, 0f, 6f);
        playerBody.position = new Vector3(0f, 1f, 8f);
        Physics.SyncTransforms();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        Assert.That(gate.activeSelf, Is.False);
        Assert.That(fireworks.activeSelf, Is.True);
        Assert.That(fireworks.transform.childCount, Is.EqualTo(3));
        VisualEffect[] fireworkBursts = fireworks.GetComponentsInChildren<VisualEffect>();
        Assert.That(fireworkBursts, Has.Length.EqualTo(3));
        foreach (VisualEffect burst in fireworkBursts)
            Assert.That(burst.visualEffectAsset.name, Does.Contain("Firework"));

        yield return new WaitForSeconds(1f);
        Vector2 stoppedVelocity = new Vector2(playerBody.linearVelocity.x, playerBody.linearVelocity.z);
        Assert.That(stoppedVelocity.magnitude, Is.LessThan(0.1f));

        Keyboard keyboard = Keyboard.current ?? InputSystem.AddDevice<Keyboard>();
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.W));
        InputSystem.Update();
        for (int frame = 0; frame < 4; frame++)
            yield return new WaitForFixedUpdate();
        Assert.That(playerBody.linearVelocity.z, Is.GreaterThan(0.2f),
            "Movement input should work again after the celebration halt completes.");
        InputSystem.QueueStateEvent(keyboard, new KeyboardState());
        InputSystem.Update();


    }
}

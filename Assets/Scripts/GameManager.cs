using UnityEngine;
using Unity.Netcode;
using Unity.Services.Core;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay.Models;
using TMPro;
using System.Collections;


public class GameManager : MonoBehaviour
{
    public string joinCode = "";
    public TMP_Text codeDisplay;
    public static ulong id;

    public bool isClient;
    // Start is called before the first frame update

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        Connect();
    }

    public async void Connect()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        if (!isClient)
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(50);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "udp"));
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            codeDisplay.text = "Code: " + joinCode;
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "udp"));
            NetworkManager.Singleton.StartClient();
        }
        return;

    }  



}

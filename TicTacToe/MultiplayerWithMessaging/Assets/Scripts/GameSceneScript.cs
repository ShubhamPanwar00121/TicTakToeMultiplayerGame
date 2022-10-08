using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneScript : MonoBehaviour
{
    [SerializeField] private Image redPlayer, bluePlayer;
    [SerializeField] private Text winnerText;
    [SerializeField] private GameObject board;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private InputField messageInput;
    [SerializeField] private Text opponentMsg;
 
    private void Start()
    {
        UiManager.UIinstance.SetVariables(redPlayer,bluePlayer,winnerText,messageInput,opponentMsg);
        GameManager.Instance.SetVariables(board,tilePrefab);
        GameManager.Instance.SetGameBoard();
        UiManager.UIinstance.SetUi();
    }
    public void SendMessageToOpponent()
    {
        UiManager.UIinstance.OnMessageSend();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    #region referances
    [SerializeField] private Button createRoom, joinRoom;
    [SerializeField] private Text roomId;
    [SerializeField] private InputField inputRoomId;
    [SerializeField] private Text logMessage;
    [SerializeField] public Image internetError;
    private InputField messageInput;
    private Text opponentMessage;
    private Image redPlayer, bluePlayer;
    private Text winnerText;
    public static UiManager UIinstance { get; private set; }
    #endregion

    private void Awake()
    {
        if(UIinstance != null && UIinstance != this)
        {
            Destroy(this);
        }
        else
        {
            UIinstance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    private void Update()
    {
       if(internetError) internetError.gameObject.SetActive(Application.internetReachability == NetworkReachability.NotReachable);
    }
    public void SetVariables(Image redPlayer, Image bluePlayer, Text winnerText, InputField messageInput, Text opponentMessage)
    {
        this.redPlayer = redPlayer;
        this.bluePlayer = bluePlayer;
        this.winnerText = winnerText;
        this.messageInput = messageInput;
        this.opponentMessage = opponentMessage;
    }
    public void SetUi()
    {
        SetUiForPlayerTurn(GameManager.Instance.playerTurn);
    }
    private void SetUiForPlayerTurn(PLayerColor playerTurn)
    {
        if(playerTurn == PLayerColor.RED)
        {
            redPlayer.gameObject.SetActive(true);
            bluePlayer.gameObject.SetActive(false);
        }
        else
        {
            redPlayer.gameObject.SetActive(false);
            bluePlayer.gameObject.SetActive(true);
        }
    }
    public void OnBtnPress(GameObject Btn, bool CalledByFirebase = false)
    {
        if (GameManager.Instance.playerTurn == PLayerColor.BLUE && !CalledByFirebase) return;

        Tile btn;

        if(Btn.TryGetComponent<Tile>(out Tile tileComponent))
        {
            btn = tileComponent;

            //updating database
            if (firebaseManager.instance.playerNumber == 1) firebaseManager.instance.SetP1Values(btn.GetButtonIndexJSON());
            else if (firebaseManager.instance.playerNumber == 2) firebaseManager.instance.SetP2Values(btn.GetButtonIndexJSON());
            //-----------------

            if (btn.playerColor != PLayerColor.NONE) return; //check if button is alreadey pressed
            btn.SetPlayerColor(GameManager.Instance.playerTurn);
            Btn.GetComponent<Image>().color = (GameManager.Instance.playerTurn == PLayerColor.RED) ? Color.red : Color.blue;
            if(GameManager.Instance.CheckBoard(GameManager.Instance.playerTurn)) //checking if anyone wins
            {
                string winner = GameManager.Instance.playerTurn == PLayerColor.RED ? "Red" : "Blue";
                winnerText.gameObject.SetActive(true);
                winnerText.text = string.Format("{0} Is The Winner", winner);
                Time.timeScale = 0;
            }
            else
            {
                if (GameManager.Instance.allTilesFilled) //check if all buttons are pressed
                {
                    winnerText.text = "Match Tie";
                    winnerText.gameObject.SetActive(true);
                    Time.timeScale = 0;
                }
                   
                else
                {
                    GameManager.Instance.changeTurns();
                    SetUiForPlayerTurn(GameManager.Instance.playerTurn);
                }
            }
        }
        else
        {
            SetLogMessage("Btn did not have Tile.cs");
        }
    }
    public void RoomCreated()
    {
        roomId.text = firebaseManager.instance.roomId;
        SetLogMessage("Room Created Waiting For Other To Join");
    }
    public void JoinRoom()
    {
        if (!inputRoomId.gameObject.activeInHierarchy)
        {
            inputRoomId.gameObject.SetActive(true);
            createRoom.interactable = false;
            joinRoom.transform.GetChild(0).GetComponent<Text>().text = "Join";
        }
        else
        {
            if(!string.IsNullOrEmpty(inputRoomId.text))
            {
                joinRoom.interactable = false;
                inputRoomId.text.ToUpper();
                firebaseManager.instance.JoinRoom(inputRoomId.text);
            }
        }
    }
    public void RoomCreateClicked()
    {
        createRoom.interactable = false;
        joinRoom.interactable = false;
        roomId.gameObject.SetActive(true);
    }
    public void SetLogMessage(string message)
    {
        logMessage.text = message;
    }
    public void OnMessageSend()
    {
        firebaseManager.instance.SendMessageGame(messageInput.text);
    }
    public void OnMessageGet(string message)
    {
        opponentMessage.text = message;
    }
}

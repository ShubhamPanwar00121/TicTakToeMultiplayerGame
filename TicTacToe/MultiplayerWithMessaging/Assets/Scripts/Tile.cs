using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [HideInInspector] public PLayerColor playerColor { get; private set; }
    [HideInInspector] public bool clicked = false;
    private Button Btn;
    private int i, j;

    private void Awake()
    {
        playerColor = PLayerColor.NONE;
        Btn = this.gameObject.GetComponent<Button>();
        Btn.onClick.AddListener(() => UiManager.UIinstance.OnBtnPress(this.gameObject));
    }
    public void SetPlayerColor(PLayerColor _playerColor)
    {
        playerColor = _playerColor;
        switch (_playerColor)
        {
            case PLayerColor.RED:
                break;
            case PLayerColor.BLUE:
                break;
            default:
                Debug.Log("Wrong value of playerColor assigned");
                break;
        }
    }
    public void SetTileIndexes(int i, int j)
    {
        this.i = i;
        this.j = j;
    }
    public string GetButtonIndexJSON()
    {
        TilePojo pojo = new TilePojo();
        pojo.i = i;
        pojo.j = j;
        string json = JsonUtility.ToJson(pojo, true);
        return json;
    }
}
public enum PLayerColor
{
    RED,
    BLUE,
    NONE
}

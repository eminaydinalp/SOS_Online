using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    public Text userNameBigText;
    public Text userNameText;
    public Text countryText;
    public Text cityText;
    public Text levelText;
    public Text totalWinText;
    public Text totalLostText;
    public Text totalPlayedGame;
    public Text totalSosedText;
    public Text totalGoldText;

    private UserData user;

    void Awake()
    {
        user = UserData.Instance;
        GetUserProfileInformation();
    }

    public void GetUserProfileInformation()
    {
        Debug.Log("Bilgiler alınıyor...");
        userNameBigText.text = user.username;
        Debug.Log(user.username);
        userNameText.text = user.username;
        countryText.text = user.country;
        cityText.text = user.city;
        levelText.text = user.level.ToString();
        totalWinText.text = user.numberofgameswin.ToString();
        totalLostText.text = user.numberofgameslost.ToString();
        totalPlayedGame.text = user.totalnumberofgames.ToString();
        totalSosedText.text = user.totalnumberofsos.ToString();
        totalGoldText.text = user.totalgoldcount.ToString();
        

    }
 
}

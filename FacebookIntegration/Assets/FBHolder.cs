using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using Facebook.MiniJSON;
using UnityEngine.UI;

public class FBHolder : MonoBehaviour
{
    public Button loginButton;
    public Image profileAvatar;
    public Text profileUsername;
    //public Text profileUserID;
    private Dictionary<string, object> profile = null;
    private Dictionary<string, object> scoreResult = null;
    private Dictionary<string, object> userList = null;
    private List<object> dataList = null;
    int playerScore;
    void Awake()
    {
        FB.Init(SetInit, OnHideUnity);
    }

    void SetInit()
    {
        Debug.Log("FB Init done");
        if (!FB.IsLoggedIn)
        {
            Debug.Log("Login");
        }
        else
        {
            //FBLogin();
        }
    }
    void OnHideUnity(bool isGameShown)
    {

    }
    public void FBLogInLogOut()
    {
        if (!FB.IsLoggedIn)
        {
            var permission = new List<string>() { "email", "public_profile,public_actions" };
            //FB.LogInWithReadPermissions(permission, FBLoggedIn);
            FB.LogInWithPublishPermissions(permission, FBLoggedIn);
        }
        else
        {
            FB.LogOut();
            loginButton.GetComponentInChildren<Text>().text = "Login To Facebook";
            Debug.Log("LogOut");
        }
    }
    void FBLoggedIn(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("Logged In");
            loginButton.GetComponentInChildren<Text>().text = "LogOut";
            FB.API("/me/picture", HttpMethod.GET, GetPicture);
            //FB.API("/me?fields=id,name", HttpMethod.GET, GetUserName); work same as /me
            FB.API("/me", HttpMethod.GET, GetUserName);
        }
    }
    void GetPicture(IGraphResult result)
    {
        if (string.IsNullOrEmpty(result.Error) && result.Texture != null)
        {
            //profileAvatar.GetComponent<Material>().mainTexture = result.Texture;
            //profileAvatar.GetComponent<RawImage>().texture = result.Texture;
            profileAvatar.sprite = Sprite.Create(result.Texture, new Rect(0, 0, result.Texture.width, result.Texture.height), new Vector2(0, 0));
        }
    }
    void GetUserName(IResult result)
    {
        if (result.Error != null)
        {
            Debug.Log("Problems getting in username");
        }
        profile = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
        profileUsername.text = "Hello " + profile["name"];
        //profileUserID.text = "ID," + profile["id"];
    }

    public void ShareButton()
    {
        FB.ShareLink(
            contentTitle : "Awesome Game",
            contentDescription : "Testing game"
            );
    }

    //Getting and setting scores
    public void GetScores()
    {
        FB.API("/app/scores?fields=score,user.limit(10)", HttpMethod.GET, ScoresCallback);
    }
    void ScoresCallback(IResult result)
    {
        Debug.Log("Score " + result.RawResult);
        scoreResult = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
        dataList = scoreResult["data"] as List<object>;
        //Dictionary<string, object> scoreList = ((Dictionary<string, object>)data[0]) as Dictionary<string, object>;
        //userList = ((Dictionary<string, object>)dataList[0])["user"] as Dictionary<string, object>;
        //playerScore = int.Parse(((Dictionary<string, object>)dataList[0])["score"].ToString());

        foreach (var dataObject in dataList)
        {
            userList = ((Dictionary<string, object>)dataObject)["user"] as Dictionary<string, object>;
            playerScore = int.Parse(((Dictionary<string, object>)dataObject)["score"].ToString());
            Debug.Log("User Name and Score " + userList["name"] + " " + playerScore);
        }

    }
    public void SetScores()
    {
        var scoreData = new Dictionary<string, string>();
        scoreData["score"] = Random.Range(10, 200).ToString();
        FB.API("/me/scores", HttpMethod.POST, delegate(IGraphResult result)
        {
            Debug.Log("Score submit result" + result.RawResult);

        }, scoreData);
    }
        
}

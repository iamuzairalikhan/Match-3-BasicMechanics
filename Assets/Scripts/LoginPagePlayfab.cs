using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class LoginPagePlayfab : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI topText;
    [SerializeField] TextMeshProUGUI messageText;

    [Header("Login")]
    [SerializeField] TMP_InputField emailLoginInput;
    [SerializeField] TMP_InputField passwordLoginInput;
    [SerializeField] GameObject loginPage;

    [Header("Register")]
    [SerializeField] TMP_InputField usernameRegisterInput;
    [SerializeField] TMP_InputField emailRegisterInput;
    [SerializeField] TMP_InputField passwordRegisterInput;
    [SerializeField] GameObject registerPage;

    [Header("Recovery")]
    [SerializeField] TMP_InputField emailRecoveryInput;
    [SerializeField] GameObject recoverPage;

    [SerializeField]
    private GameObject welcomeText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    #region Button Functions

    public void RegisterUser()
    {
        //password check here if long short or character check etc  
        var request  = new RegisterPlayFabUserRequest
        {
            DisplayName = usernameRegisterInput.text,
            Email = emailRegisterInput.text,
            Password = passwordRegisterInput.text,

            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    public void Login()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailLoginInput.text,
            Password = passwordLoginInput.text,

            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        string name = null;
        if(result.InfoResultPayload != null)
        {
            name = result.InfoResultPayload.PlayerProfile.DisplayName;
        }
        welcomeText.SetActive(true);
        //Code for getting username
        welcomeText.GetComponent<TextMeshProUGUI>().text = "Welcome " + name;
        StartCoroutine(LoadNextScene());
    }

    public void RecoverUser()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = emailRecoveryInput.text,
            TitleId = "9E418"
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnRecoverySuccess, OnErrorRecovery);
    }

    private void OnRecoverySuccess(SendAccountRecoveryEmailResult result)
    {
        OpenLoginPage();
        messageText.text = "Recovery mail sent!";
    }

    private void OnError(PlayFabError error)
    {
        messageText.text = "No Email Found!";
    }

    private void OnErrorRecovery(PlayFabError error)
    {
        messageText.text = error.ErrorMessage;
        Debug.Log(error.GenerateErrorReport());
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        messageText.text = "New Account is created!";
        OpenLoginPage();
    }

    public void OpenLoginPage()
    {
        loginPage.SetActive(true);
        registerPage.SetActive(false);
        recoverPage.SetActive(false);
        topText.text = "LOGIN";
    }

    public void OpenRegisterPage()
    {
        loginPage.SetActive(false);
        registerPage.SetActive(true);
        recoverPage.SetActive(false);
        topText.text = "REGISTER";
    }

    public void OpenRecoveryPage()
    {
        loginPage.SetActive(false);
        registerPage.SetActive(false);
        recoverPage.SetActive(true);
        topText.text = "RECOVER";
    }
    #endregion

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(2);
        messageText.text = "Logging in";
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private bool IsValidEmail(string email)
    {
        Regex emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.IgnoreCase);
        return emailRegex.IsMatch(email);
    }

    private bool CheckPassword(string password, int min)
    {
        bool hasNum = false;
        bool hasCap = false;
        bool hasLow = false;
        bool hasSpec = false;
        char currentCharacter; 
        if(!(password.Length >= min))
        {
            return false;
        }

        for (int i = 0; i < password.Length; i++)
        {
            currentCharacter = password[i];
            if (char.IsDigit(currentCharacter))
            {
                hasNum = true;
            }
            else if (char.IsUpper (currentCharacter))
            {
                hasCap = true;
            }
            else if (char. IsLower (currentCharacter))
            {
                hasLow = true;
            }
            else if (!char.IsLetterOrDigit(currentCharacter))
            {
                hasSpec = true;
            }
            if (hasNum && hasCap && hasLow && hasSpec)
            {
                return true;
            }
        }
        return false;
    } 
    
}

using UnityEngine;
using Firebase.Auth;
using TMPro;
using System;
using System.Text.RegularExpressions;
using System.Globalization;
using Firebase.Extensions;

public class AuthController : MonoBehaviour
{
    [SerializeField]
    public TMP_Text email;
    [SerializeField]
    public TMP_Text password1;
    [SerializeField]
    public TMP_Text password2;
    [SerializeField]
    public TMP_Text responseTextblock;

    [SerializeField] LoginUIController loginUIController;

    public Color successColor = new(0.2039216f, 0.6588235f, 0.3254902f);

    private void Awake()
    {
        loginUIController = FindObjectOfType<LoginUIController>();
    }

    public void Login()
    {
        if (!VerifyFields()) return;

        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email.text, password1.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
            }
            if (task.IsFaulted)
            {
                Firebase.FirebaseException e =
                task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                GetErrorMessage((AuthError)e.ErrorCode);

                return;
            }
            if (task.IsCompleted)
            {
                UpdateResponse("Login Successful", successColor);
                print("Login Successful");
                loginUIController.OnSuccess();
            }
        });
    }
    public void LoginWithoutFeedback()
    {
        if (!VerifyFields()) return;

        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email.text, password1.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
            }
            if (task.IsFaulted)
            {
                Firebase.FirebaseException e =
                task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                GetErrorMessage((AuthError)e.ErrorCode);

                return;
            }
            if (task.IsCompleted)
            {
                loginUIController.OnSuccess();
            }
        });
    }

    public void LoginAndKeepCurrentScene()
    {
        if (!VerifyFields()) return;

        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email.text, password1.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
            }
            if (task.IsFaulted)
            {
                Firebase.FirebaseException e =
                task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                GetErrorMessage((AuthError)e.ErrorCode);

                return;
            }
            if (task.IsCompleted)
            {
                UpdateResponse("Login Successful", successColor);
                print("Login Successful");
            }
        });
    }


    public void LoginAnonymous()
    {
        FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
            }
            if (task.IsFaulted)
            {
                Firebase.FirebaseException e =
                task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                GetErrorMessage((AuthError)e.ErrorCode);

                return;
            }
            if (task.IsCompleted)
            {
                print("Anonymous Login Successful");
            }
        });
    }

    public void ResgisterUser()
    {
        if (!VerifyFields()) return;

        FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email.text, password1.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Firebase.FirebaseException e =
                task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                GetErrorMessage((AuthError)e.ErrorCode);

                return;
            }
            if (task.IsFaulted)
            {
                Firebase.FirebaseException e =
                task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                GetErrorMessage((AuthError)e.ErrorCode);

                return;
            }
            if (task.IsCompleted)
            {
                UpdateResponse("Account Created Successfully", successColor);
                Debug.Log("ResgisterUser success");
                LoginWithoutFeedback();
            }
        });

    }

    public void Logout()
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            FirebaseAuth.DefaultInstance.SignOut();
        }
    }

    public void ResetPassword()
    {
        if (!IsValidEmail(email.text))
        {
            Debug.Log("Invalid Email");
            UpdateResponse("Invalid Email", Color.red);
            return;
        }

        FirebaseAuth.DefaultInstance.SendPasswordResetEmailAsync(email.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Firebase.FirebaseException e =
                task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                GetErrorMessage((AuthError)e.ErrorCode);

                return;
            }
            if (task.IsFaulted)
            {
                Firebase.FirebaseException e =
                task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                GetErrorMessage((AuthError)e.ErrorCode);

                return;
            }
            if (task.IsCompleted)
            {
                UpdateResponse("Password Reset Sent Successfully", successColor);
                Debug.Log("Password Reset Sent Successfully");

                if (loginUIController == null)
                {
                    throw new MissingComponentException("LoginUIController not found");
                }

                //loginUIController.DisablePasswordResetButton();

            }
        });

    }

    public bool VerifyFields()
    {
        if (!IsValidEmail(email.text))
        {
            Debug.Log("Invalid Email");
            UpdateResponse("Invalid Email", Color.red);
            return false;
        }
        if (!IsValidPassword())
        {
            return false;
        }
        return true;
    }

    private bool IsValidPassword()
    {
        //if not null, passwords are matching and length < 7
        if (string.IsNullOrEmpty(password1.text) || string.IsNullOrEmpty(password2.text))
        {
            Debug.Log("One or More Password Field is empty");
            UpdateResponse("One or More Password Field is empty", Color.red);
            return false;
        }
        if (!String.Equals(password1.text, password2.text))
        {
            Debug.Log("Passwords do not match");
            UpdateResponse("Passwords do not match", Color.red);
            return false;
        }
        if (password1.text.Length < 8 || password1.text.Length > 24)
        {
            Debug.Log("Password must be between 8 to 24 characters");
            UpdateResponse("Password must be between 8 to 24 characters", Color.red);
            return false;
        }
        return true;
    }



    // IsValidEmail courtesy of Microsoft https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                  RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                string domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException e)
        {
            Debug.LogException(e);
            return false;
        }
        catch (ArgumentException e)
        {
            Debug.LogException(e);
            return false;
        }

        try
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    private void GetErrorMessage(AuthError errorCode)
    {
        string message = errorCode.ToString();

        UpdateResponse(message, Color.red);
        Debug.Log(message);
    }

    public void UpdateResponse(string msg, Color color)
    {
        if (color.Compare(Color.green))
        {
            color = new Color(0, 190, 0);
        }
        responseTextblock.color = color;
        responseTextblock.text = msg;
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MenuGridSize : MonoBehaviour
{
    // Tags are made and assigned for both x and y input field.

    // Inputfields are made static in order to access the class variable in the other script.
    public static InputField xField;
    public static InputField yField;

    // Creates a text UI with a warning, that tell us that an integer input in either box is required in order to press play. Assigns the text UI to this public variable in the inspector.
    public GameObject textWarning;

    private void Awake()
    {
        // Look for game objects with these tags and stores it in the variable.
        xField = GameObject.FindWithTag("Field X").GetComponent<InputField>();
        yField = GameObject.FindWithTag("Field Y").GetComponent<InputField>();

        // This game object will still exist in the next scene
        DontDestroyOnLoad(this.gameObject);
    }

    // Instead of using the standard scene change script, we use this instead for the custom grid scene. Script is placed onto an empty game objectm and then assigned to the play button OnClick().
    public void EnterGame(string scene)
    {
        // If input fields are empty, it  will unhide the textWarning gameo bject where the player is told to put numbers in.
        if (xField.text == "" && yField.text == "" || xField.text != "" && yField.text == "" || xField.text == "" && yField.text != "")
        {
            textWarning.SetActive(true);
            Debug.Log("Must enter grid size");

        }
        else
        {
            // Hide warning and load the scene if numbers are placed inside the variable.
            textWarning.SetActive(false);
            SceneManager.LoadScene(scene);
        }
    }
}
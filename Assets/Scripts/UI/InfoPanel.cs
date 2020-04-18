using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Player;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    private LevelController levelController;
    public GameObject nameTextField;
    public GameObject actionPointsTextField;
    public GameObject specialMoveTextField;
    public Button actionButton;
    public Button nextRoundButton;
    private int selectedUnit;
    public GameObject portraitCamera;
    private AbstractUnit currentUnit;


    // Start is called before the first frame update
    void Start()
    {
        levelController = GameObject.Find("LevelController").GetComponent<LevelController>();
        currentUnit = levelController.playerUnits[this.selectedUnit];
        actionButton.onClick.AddListener(this.ActionButtonClicked);
        nextRoundButton.onClick.AddListener(this.NextRoundButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        if (levelController.playerUnits.Count > levelController.currentlySelectedUnit && levelController.currentlySelectedUnit != -1)
        {
            currentUnit = levelController.playerUnits[levelController.currentlySelectedUnit];
        }
        else
        {
            Debug.Log("All units dead. Can't show anything");

            actionPointsTextField.GetComponent<UnityEngine.UI.Text>().text = "";
            nameTextField.GetComponent<UnityEngine.UI.Text>().text = "";
            specialMoveTextField.GetComponent<UnityEngine.UI.Text>().text = "";

            return;
        }

        Transform unitTransform = currentUnit.ObjectTransform;
        portraitCamera.GetComponent<PortraitCameraController>().Show(unitTransform);
        actionPointsTextField.GetComponent<UnityEngine.UI.Text>().text = currentUnit.ActionPoints.ToString() + " / " + (int) currentUnit.UnitType;
        nameTextField.GetComponent<UnityEngine.UI.Text>().text = currentUnit.name.ToString();

        specialMoveTextField.GetComponent<UnityEngine.UI.Text>().text = string.Format("Cost: {0} Action Points\n\n{1}", currentUnit.UnitActions?[0].Item2, currentUnit.UnitActions?[0].Item3);
    }

    public void ActionButtonClicked()
    {
        levelController.ExecuteAction(currentUnit.UnitActions?[0], currentUnit.UnitType, currentUnit);
    }

    public void NextRoundButtonClicked()
    {
        levelController.UpdateTiles();
    }
}

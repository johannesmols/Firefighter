using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Player;

public class InfoPanel : MonoBehaviour
{
    private LevelController levelController;
    public GameObject nameTextField;
    public GameObject actionPointsTextField;
    public GameObject specialMoveTextField;
    private int selectedUnit;
    public GameObject portraitCamera;
    private AbstractUnit currentUnit;


    // Start is called before the first frame update
    void Start()
    {
        levelController = GameObject.Find("LevelController").GetComponent<LevelController>();
        selectedUnit = levelController.currentlySelectedUnit;
        currentUnit = levelController.playerUnits[this.selectedUnit];
    }

    // Update is called once per frame
    void Update()
    {
        if(this.selectedUnit != levelController.currentlySelectedUnit){
            this.selectedUnit = levelController.currentlySelectedUnit;
            currentUnit = levelController.playerUnits[this.selectedUnit];
        }

        Transform unitTransform = currentUnit.ObjectTransform;
        portraitCamera.GetComponent<PortraitCameraController>().Show(unitTransform);
        actionPointsTextField.GetComponent<UnityEngine.UI.Text>().text = currentUnit.ActionPoints.ToString() + " / " + (int) currentUnit.UnitType;
        nameTextField.GetComponent<UnityEngine.UI.Text>().text = currentUnit.name.ToString();

        specialMoveTextField.GetComponent<UnityEngine.UI.Text>().text = string.Format("Cost: {0} Action Points\n\n{1}", currentUnit.UnitActions?[0].Item2, currentUnit.UnitActions?[0].Item3);
    }
}

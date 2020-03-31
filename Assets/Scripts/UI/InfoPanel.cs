using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Player;

public class InfoPanel : MonoBehaviour
{
    private LevelController levelController;
    public GameObject nameTextField;
    public GameObject actionPointsTextField;
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
        actionPointsTextField.GetComponent<UnityEngine.UI.Text>().text = currentUnit.ActionPoints.ToString()+" / 4";
    }
}

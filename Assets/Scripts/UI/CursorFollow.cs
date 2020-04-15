using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFollow : MonoBehaviour
{
    public Canvas myCanvas;
    public RectTransform tooltip;
    private int horizontal; // -1 for left, 1 for right
    private int vertical; // -1 for down, 1 for up
    public int offsetX;
    public int offsetY;
    //public UnityEngine.UI.Text textBox;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        //TextGenerator textGen = new TextGenerator();
        //TextGenerationSettings generationSettings = textBox.GetGenerationSettings(textBox.rectTransform.rect.size); 
        //float width = textGen.GetPreferredWidth(textBox.text, generationSettings);
        //float height = textGen.GetPreferredHeight(textBox.text, generationSettings);
        //tooltip.sizeDelta = new Vector2(width+10, height+10);

        if(Input.mousePosition.x+tooltip.rect.width + offsetX> Screen.width ){
            horizontal = -1;
        }else{
            horizontal = 1;
        }
        if(Input.mousePosition.y+tooltip.rect.height + offsetY> Screen.height ){
            vertical = -1;
        }else{
            vertical = 1;
        }
        
        transform.position = Input.mousePosition + new Vector3(horizontal*(tooltip.rect.width/2 + offsetX), vertical*(tooltip.rect.height/2 + offsetY), 0 );
    }
}

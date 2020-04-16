using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class CornerMenu : MonoBehaviour
    {
        public Button menuButton;
        public Button settingsButton;
        public Button helpButton;

        public void Start()
        {
            menuButton.onClick.AddListener(this.MenuButtonClicked);
            settingsButton.onClick.AddListener(this.SettingsButtonClicked);
            helpButton.onClick.AddListener(this.HelpButtonClicked);
        }

        public void Update()
        {

        }

        public void MenuButtonClicked()
        {
            SceneManager.LoadScene("Startmenu");
        }

        public void SettingsButtonClicked()
        {

        }

        public void HelpButtonClicked()
        {

        }
    }
}
